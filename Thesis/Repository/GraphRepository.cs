using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Domain.DomainClasses;
using Domain.GraphClasses;
using Repository.MSSQL.Interfaces;

namespace Repository.MSSQL
{
    public class GraphRepository : RepositoryBase, IGraphRepository
    {
        private readonly ThesisDbContext _context;

        public GraphRepository(ThesisDbContext context) : base(context)
        {
            this._context = context;
        }

        /// <summary>
        /// Extracts all vertices from database.
        /// </summary>
        public HashSet<Vertex<User>> ExtractVerticesFromDatabase()
        {
            return new HashSet<Vertex<User>>(from user in _context.Users
                                             select new Vertex<User>()
                                             {
                                                 Id = user.Id
                                             });
        }

        /// <summary>
        /// Extracts all edges from database.
        /// Edge is created only when a User changes an email with another User at least 11 times.
        /// </summary>
        public HashSet<Edge<User>> ExtractEdgesFromDatabase()
        {
            //var qry = @"SELECT  em.SenderId, er.RecipientId, COUNT(*) AS [Count]
            //            FROM EmailMessages em
            //            INNER JOIN EmailRecipients er on em.Id = er.EmailMessageId
            //            GROUP BY  em.SenderId, er.RecipientId
            //            HAVING Count(*) > 11";


            IQueryable<Edge<User>> queryable = (from emailMessage in _context.EmailMessagess
                                                join recipient in _context.Recipients on emailMessage.Id equals recipient.EmailMessageId
                                                group emailMessage by new { emailMessage.Sender, recipient.Recipient }
                into g
                                                where g.Count() > 50
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

        public HashSet<Vertex<User>> ExtractVerticesFromEdges(HashSet<Edge<User>> edges)
        {
            HashSet<Vertex<User>> vertices = new HashSet<Vertex<User>>();

            foreach (Edge<User> edge in edges)
            {
                if (vertices.All(x => x.Id != edge.Vertex1.Id))
                {
                    vertices.Add(edge.Vertex1);
                }
                else if (vertices.All(y => y.Id != edge.Vertex2.Id))
                {
                    vertices.Add(edge.Vertex1);
                }
            }

            return vertices;
        }

        public HashSet<Vertex<User>> ExtractVerticesFromConversations()
        {
            // extracting conversations from database
            HashSet<ConversationEmails> conversationEmails = new HashSet<ConversationEmails>(from conversation in _context.Conversations
                                                                                             group conversation.EmailMessage by conversation.ConversationId into grp
                                                                                             select new ConversationEmails()
                                                                                             {
                                                                                                 ConverationId = grp.Key,
                                                                                                 Emails = grp.ToList()
                                                                                             });
            HashSet<Vertex<User>> vertices = new HashSet<Vertex<User>>(from conversationEmail in conversationEmails.SelectMany(x => x.Emails)
                                                                       group conversationEmail by conversationEmail.Sender
                into senders
                                                                       select new Vertex<User>()
                                                                       {
                                                                           Id = senders.Key.Id
                                                                       });

            return vertices;
        }

        public void ExtractEdgesFromConversation()
        {
            HashSet<Edge<User>> edges = new HashSet<Edge<User>>();

            HashSet<ConversationEmails> conversationEmails = new HashSet<ConversationEmails>(from conversation in _context.Conversations
                                                                                             group conversation.EmailMessage by conversation.ConversationId
                into grp
                                                                                             select new ConversationEmails()
                                                                                             {
                                                                                                 ConverationId = grp.Key,
                                                                                                 Emails = grp.ToList()
                                                                                             });

            List<Edge<User>> list = (from conversationEmailSet1 in conversationEmails.SelectMany(x => x.Emails)
                                     from conversationEmailSet2 in conversationEmails.SelectMany(x => x.Emails)
                                     where conversationEmailSet1.Sender.Id < conversationEmailSet2.Sender.Id // only one direction
                                     select new Edge<User>()
                                     {
                                         Vertex1 = new Vertex<User>()
                                         {
                                             Id = conversationEmailSet1.Sender.Id
                                         },
                                         Vertex2 = new Vertex<User>()
                                         {
                                             Id = conversationEmailSet2.Sender.Id
                                         }
                                     }).ToList();
        }
    }
}
