using DataAccess.Interfaces;
using Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Repositories
{
    public class ModeratorRepository : IRepository<Moderator>
    {
        private readonly IdentityDbContext _context;

        public ModeratorRepository(IdentityDbContext context)
        {
            this._context = context;
        }

        public IQueryable<Moderator> List
        {
            get
            {
                return _context.Set<Moderator>();
            }
        }

        public Moderator Add(Moderator entity)
        {
            _context.Set<Moderator>().Add(entity);
            _context.SaveChanges();

            return entity;
        }

        public void Delete(Moderator entity)
        {
            throw new NotImplementedException();
        }

        public void DeleteList(ICollection<Moderator> list)
        {
            throw new NotImplementedException();
        }

        public Moderator FindById(int id)
        {
            return _context.Set<Moderator>().Find(id);
        }

        public Moderator FindByIdWithIncludeArray<TInclude>(int id, System.Linq.Expressions.Expression<Func<Moderator, ICollection<TInclude>>> includeFunc)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Moderator> ListWithInclude<TInclude>(System.Linq.Expressions.Expression<Func<Moderator, TInclude>> includeFunc) where TInclude : Entity
        {
            throw new NotImplementedException();
        }

        public IQueryable<Moderator> ListWithIncludeArray<TInclude>(System.Linq.Expressions.Expression<Func<Moderator, ICollection<TInclude>>> includeFunc) where TInclude : Entity
        {
            throw new NotImplementedException();
        }

        public void Update(Moderator entity)
        {
            var director = _context.Set<Moderator>().Find(entity.Id);

            director.Name = entity.Name;
            director.LastName = entity.LastName;
            director.PhoneNumber = entity.PhoneNumber;
            director.CellPhone = entity.CellPhone;
            director.Email = entity.Email;
            director.Address = entity.Address;
            _context.SaveChanges();
        }
    }
}
