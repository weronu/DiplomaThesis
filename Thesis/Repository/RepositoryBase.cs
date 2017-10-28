using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Common;
using Domain.DomainClasses;

namespace Repository.MSSQL
{
    public class RepositoryBase
    {
        private ThesisDbContext _context;

        public RepositoryBase(ThesisDbContext context)
        {
            _context = context;
        }

        protected TEntity GetById<TEntity>(int id) where TEntity : DomainBase
        {
            TEntity e = _context.Set<TEntity>().FirstOrDefault(x => x.Id == id);
            return e;
        }

        protected TEntity GetById<TEntity>(int id, params Expression<Func<TEntity, object>>[] includes) where TEntity : DomainBase
        {
            IQueryable<TEntity> qry = _context.Set<TEntity>().Where(x => x.Id == id);
            foreach (Expression<Func<TEntity, object>> property in includes)
            {
                qry = qry.Include(property);
            }
            return qry.FirstOrDefault();
        }

        protected TEntity GetByIdNoTracking<TEntity>(int id) where TEntity : DomainBase
        {
            TEntity e = _context.Set<TEntity>().FirstOrDefault(x => x.Id == id);
            _context.Entry(e).State = EntityState.Detached;
            return e;
        }

        protected TEntity GetByIdNoTracking<TEntity>(int id, params Expression<Func<TEntity, object>>[] includes) where TEntity : DomainBase
        {
            IQueryable<TEntity> qry = _context.Set<TEntity>().Where(x => x.Id == id);
            foreach (Expression<Func<TEntity, object>> property in includes)
            {
                qry = qry.Include(property);
            }
            return qry.AsNoTracking().FirstOrDefault();
        }

        protected void Add<TEntity>(TEntity e) where TEntity : DomainBase
        {
            lock (_context)
            {
                _context.Set<TEntity>().Add(e);
            }
        }

        protected void Update<TEntity>(TEntity e) where TEntity : class
        {
            lock (_context)
            {
                if (_context.Entry(e).State == EntityState.Detached)
                {
                    _context.Set<TEntity>().Attach(e);
                    _context.Entry(e).State = EntityState.Modified;
                }
            }
        }

        protected void Delete<TEntity>(int id) where TEntity : DomainBase
        {
            lock (_context)
            {
                var e = _context.Set<TEntity>().FirstOrDefault(x => x.Id == id);

                if (e == null) throw new ThesisException(ErrorMessages.ObjectNotFound);

                var entry = _context.Entry(e);
                if (entry.State == EntityState.Detached)
                    _context.Set<TEntity>().Attach(e);

                _context.Set<TEntity>().Remove(e);
            }
        }

    }
}
