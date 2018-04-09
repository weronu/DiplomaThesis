using System;
using System.Linq.Expressions;
using Domain.DomainClasses;

namespace Repository.MSSQL.Interfaces
{
    public interface ICommonRepository
    {
        TEntity GetById<TEntity>(int id) where TEntity : DomainBase;
        TEntity GetById<TEntity>(int id, params Expression<Func<TEntity, object>>[] includes) where TEntity : DomainBase;

        void Add<TEntity>(TEntity e) where TEntity : DomainBase;
        void Update<TEntity>(TEntity e) where TEntity : class;
        void Delete<TEntity>(int entityId) where TEntity : DomainBase;
        void Detach(object o); 
    };
}