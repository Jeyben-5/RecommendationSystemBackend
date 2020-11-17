using DataAccess.Interfaces;
using Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Repositories
{
    public class DocumentRepository : IRepository<Document>
    {
        private readonly IdentityDbContext _context;

        public DocumentRepository(IdentityDbContext context)
        {
            this._context = context;
        }
        public IQueryable<Document> List
        {
            get
            {
                return _context.Set<Document>()
                    .Include(s => s.Subject);
            }
        }

        public Document Add(Document entity)
        {
            if (entity.Subject != null)
            {
                var subject = _context.Set<Subject>().Find(entity.Subject.Id);

                if (subject != null)
                {
                    entity.Subject = subject;
                }
            }

            _context.Set<Document>().Add(entity);
            _context.SaveChanges();

            return entity;
        }

        public void Delete(Document entity)
        {
            throw new NotImplementedException();
        }

        public Document FindById(int id)
        {
            return _context.Set<Document>().Find(id);
        }

        public Document FindByIdWithIncludeArray<TInclude>(int id, System.Linq.Expressions.Expression<Func<Document, ICollection<TInclude>>> includeFunc)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Document> ListWithInclude<TInclude>(System.Linq.Expressions.Expression<Func<Document, TInclude>> includeFunc) where TInclude : Entity
        {
            throw new NotImplementedException();
        }

        public IQueryable<Document> ListWithIncludeArray<TInclude>(System.Linq.Expressions.Expression<Func<Document, ICollection<TInclude>>> includeFunc) where TInclude : Entity
        {
            throw new NotImplementedException();
        }

        public void Update(Document entity)
        {
            throw new NotImplementedException();
        }
    }
}
