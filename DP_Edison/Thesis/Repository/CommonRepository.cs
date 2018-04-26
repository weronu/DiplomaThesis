using System;
using System.Data.Entity;
using System.Linq.Expressions;
using Domain.DomainClasses;
using Repository.MSSQL.Interfaces;

namespace Repository.MSSQL
{
    public class CommonRepository : RepositoryBase, ICommonRepository
    {
        private readonly ThesisDbContext _context;

        public CommonRepository(ThesisDbContext context) : base(context)
        {
            _context = context;
        }        

        public new TEntity GetById<TEntity>(int id) where TEntity : DomainBase
        {
            return base.GetById<TEntity>(id);
        }

        public new TEntity GetById<TEntity>(int id, params Expression<Func<TEntity, object>>[] includes) where TEntity : DomainBase
        {
            return base.GetById(id,includes);
        }

        public new void Add<TEntity>(TEntity e) where TEntity : DomainBase
        {
            base.Add(e);
        }
        public new void Update<TEntity>(TEntity e) where TEntity : class
        {
            base.Update(e);
        }

        public new void Delete<TEntity>(int entityId) where TEntity : DomainBase
        {
            base.Delete<TEntity>(entityId); 
        }

        public void Detach(object o)
        {
            var entry = _context.Entry(o);
            entry.State = EntityState.Detached;
        }

        
    }
}
