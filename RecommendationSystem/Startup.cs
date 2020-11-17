using Entities;
using Hypermedia.Metadata;
using Hypermedia.Configuration;
using Hypermedia.JsonApi.AspNetCore;
using Hypermedia.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using DataAccess.Context;
using DataAccess.Interfaces;
using DataAccess.Repositories;
using Services.Interfaces;
using Services;
using Microsoft.AspNetCore.Identity;
using Authentication.Entities;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using Authentication.DataAccess.Context;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Entities.Exceptions;

namespace RecomendationSystem
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        readonly string MyAllowSpecificOrigins = "MyPolicy";

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var contractResolver = CreateContractResolver();

            RegisterTypes(services);

            Authentication.StartupHelper.RegisterTypes(services);

            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
                });
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<RecommendationSystemContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("RecommendationDatabase"), b => b.MigrationsAssembly("RecomendationSystem"));
            });

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
            })
                .AddEntityFrameworkStores<RecommendationSystemContext>()
                .AddDefaultTokenProviders();

            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = Configuration["JwtIssuer"],
                        ValidAudience = Configuration["JwtIssuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtKey"])),
                        ClockSkew = TimeSpan.Zero // remove delay of token when expire
                    };
                });

            services.AddMvc();

            services.AddControllers()
                .AddHypermediaFormatters(
                    options =>
                    {
                        options.ContractResolver = contractResolver;
                        options.JsonApiSerializerOptions.FieldNamingStrategy = DasherizedFieldNamingStrategy.Instance;
                    });

            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            ContextInitializer.SeedData(userManager, roleManager, Configuration);
            app.UseMiddleware<ExceptionHandler>();
            app.UseCors(MyAllowSpecificOrigins);
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public static void RegisterTypes(IServiceCollection services)
        {
            services.AddTransient<IdentityDbContext, RecommendationSystemContext>();
            services.AddTransient<IRepository<Moderator>, ModeratorRepository>();
            services.AddTransient<IModeratorService, ModeratorService>();

            //Subject
            services.AddTransient<IRepository<Subject>, SubjectRepository>();
            services.AddTransient<ISubjectService, SubjectService>();

            //Document
            services.AddTransient<IRepository<Document>, DocumentRepository>();
            services.AddTransient<IDocumentService, DocumentService>();
        }

        public static IContractResolver CreateContractResolver()
        {
            var builder = new Builder();

            builder.With<Moderator>("moderator")
                    .Id(nameof(Moderator.Id))
                    .With<Student>("student")
                    .Id(nameof(Student.Id))   
                    .With<Subject>("subject")
                    .Id(nameof(Subject.Id))
                    .With<Document>("document")
                    .Id(nameof(Document.Id))
                    .BelongsTo<Subject>(nameof(Document.Subject))
                    .With<User>("users")
                    .Id(nameof(User.Id))
                    .HasMany<Role>(nameof(User.Roles))
                    .With<Role>("roles")
                    .Id(nameof(Role.Name));

            return builder.Build();
        }
    }
}
