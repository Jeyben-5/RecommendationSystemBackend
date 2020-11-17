using DataAccess.Interfaces;
using Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Linq;

namespace DataAccess.Repositories
{
    public class SubjectRepository : IRepository<Subject>
    {
        private readonly IdentityDbContext _context;

        public SubjectRepository(IdentityDbContext context) 
        {
            this._context = context;
        }
        public IQueryable<Subject> List
        {
            get
            {
                return _context.Set<Subject>();
            }
        }

        public Subject Add(Subject entity)
        {
            _context.Set<Subject>().Add(entity);
            _context.SaveChanges();

            return entity;
        }

        public void Delete(Subject entity)
        {
            throw new System.NotImplementedException();
        }

        public Subject FindById(int id)
        {
            return _context.Set<Subject>().Find(id);
        }

        public Subject FindByIdWithIncludeArray<TInclude>(int id, System.Linq.Expressions.Expression<System.Func<Subject, System.Collections.Generic.ICollection<TInclude>>> includeFunc)
        {
            throw new System.NotImplementedException();
        }

        public IQueryable<Subject> ListWithInclude<TInclude>(System.Linq.Expressions.Expression<System.Func<Subject, TInclude>> includeFunc) where TInclude : Entity
        {
            throw new System.NotImplementedException();
        }

        public IQueryable<Subject> ListWithIncludeArray<TInclude>(System.Linq.Expressions.Expression<System.Func<Subject, System.Collections.Generic.ICollection<TInclude>>> includeFunc) where TInclude : Entity
        {
            throw new System.NotImplementedException();
        }

        public void Update(Subject entity)
        {
            var matter = _context.Set<Subject>().Find(entity.Id);

            matter.Name = entity.Name;
            matter.Docente = entity.Docente;

            _context.SaveChanges();
        }
    }
}
