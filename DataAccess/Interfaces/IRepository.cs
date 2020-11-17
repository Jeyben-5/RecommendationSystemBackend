using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DataAccess.Interfaces
{
    public interface IRepository<T> where T : Entity
    {
        IQueryable<T> List { get; }
        T Add(T entity);
        void Delete(T entity);
        T FindById(int id);
        T FindByIdWithIncludeArray<TInclude>(int id, Expression<Func<T, ICollection<TInclude>>> includeFunc);
        IQueryable<T> ListWithInclude<TInclude>(Expression<Func<T, TInclude>> includeFunc) where TInclude : Entity;
        IQueryable<T> ListWithIncludeArray<TInclude>(Expression<Func<T, ICollection<TInclude>>> includeFunc) where TInclude : Entity;
        void Update(T entity);
    }
}