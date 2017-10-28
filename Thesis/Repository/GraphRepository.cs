using System.Collections.Generic;
using System.Linq;
using Domain.DomainClasses;
using Domain.GraphClasses;
using Repository.MSSQL.Interfaces;

namespace Repository.MSSQL
{
    public class GraphRepository : RepositoryBase, IGraphRepository
    {
        private readonly ThesisDbContext _dbContext;

        public GraphRepository(ThesisDbContext _dbContext) : base(_dbContext)
        {
            this._dbContext = _dbContext;
        }

        /// <summary>
        /// Extracts all vertices from database.
        /// </summary>
        public HashSet<Vertex<User>> GetVertices()
        {
            return new HashSet<Vertex<User>>(from user in _dbContext.Users
                select new Vertex<User>()
                {
                    Id = user.Id
                });
        }

        /// <summary>
        /// Extracts all edges from database.
        /// Edge is created only when a User changes an email with another User at least 11 times.
        /// </summary>
        public HashSet<Edge<User>> GetEdges()
        {
            //var qry = @"SELECT  em.SenderId, er.RecipientId, COUNT(*) AS [Count]
            //            FROM EmailMessages em
            //            INNER JOIN EmailRecipients er on em.Id = er.EmailMessageId
            //            GROUP BY  em.SenderId, er.RecipientId
            //            HAVING Count(*) > 11";


            IQueryable<Edge<User>> queryable = (from emailMessage in _dbContext.EmailMessagess
                join recipient in _dbContext.Recipients on emailMessage.Id equals recipient.EmailMessageId
                group emailMessage by new {emailMessage.Sender, recipient.Recipient}
                into g
                where g.Count() > 11
                select new Edge<User>()
                {
                    Vertex1 = new Vertex<User>()
                    {
                        Id = g.Key.Sender.Id
                    },
                    Vertex2 = new Vertex<User>()
                    {
                        Id = g.Key.Recipient.Id
                    }
                });

            List<Edge<User>> edges = queryable.ToList();

            return new HashSet<Edge<User>>(edges);
        }
    }
}
