using System.Collections.Generic;
using System.Linq;
using Domain.DomainClasses;
using Domain.DTOs;
using Domain.GraphClasses;
using Repository.MSSQL.Interfaces;
using User = Domain.DomainClasses.User;

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
        public HashSet<Node<User>> ExtractVerticesFromDatabase()
        {
            return new HashSet<Node<User>>(from user in _context.Users
                                             select new Node<User>()
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
                                                    Node1 = new Node<User>()
                                                    {
                                                        Id = g.Key.Sender.Id
                                                    },
                                                    Node2 = new Node<User>()
                                                    {
                                                        Id = g.Key.Recipient.Id
                                                    }
                                                });

            List<Edge<User>> edges = queryable.ToList();

            return new HashSet<Edge<User>>(edges);
        }

        public HashSet<Node<User>> ExtractVerticesFromEdges(HashSet<Edge<User>> edges)
        {
            HashSet<Node<User>> vertices = new HashSet<Node<User>>();

            foreach (Edge<User> edge in edges)
            {
                if (vertices.All(x => x.Id != edge.Node1.Id))
                {
                    vertices.Add(edge.Node1);
                }
                else if (vertices.All(y => y.Id != edge.Node2.Id))
                {
                    vertices.Add(edge.Node2);
                }
            }
            HashSet<Node<User>> hashSet = new HashSet<Node<User>>(vertices.Distinct().ToList());
            return new HashSet<Node<User>>(vertices.Distinct().ToList());

        }

        public HashSet<Node<User>> ExtractVerticesFromConversations()
        {
            // extracting conversations from database
            HashSet<ConversationEmails> conversationEmails = new HashSet<ConversationEmails>(from conversation in _context.Conversations
                                                                                             group conversation.EmailMessage by conversation.ConversationId into grp
                                                                                             select new ConversationEmails()
                                                                                             {
                                                                                                 ConverationId = grp.Key,
                                                                                                 Emails = grp.ToList()
                                                                                             });
            HashSet<Node<User>> vertices = new HashSet<Node<User>>(from conversationEmail in conversationEmails.SelectMany(x => x.Emails)
                                                                       group conversationEmail by conversationEmail.Sender
                into senders
                                                                       select new Node<User>()
                                                                       {
                                                                           Id = senders.Key.Id
                                                                       });

            return vertices;
        }

        public HashSet<Edge<Domain.DTOs.UserDto>> ExtractEdgesFromConversation()
        {
            HashSet<ConversationEmails> conversationEmails =
                new HashSet<ConversationEmails>(from conversation in _context.Conversations
                    group conversation.EmailMessage by conversation.ConversationId
                    into grp
                    select new ConversationEmails()
                    {
                        ConverationId = grp.Key,
                        Emails = grp.ToList()
                    });

            HashSet<Edge<Domain.DTOs.UserDto>> edgesWithDuplicates = new HashSet<Edge<Domain.DTOs.UserDto>>();

            foreach (ConversationEmails email in conversationEmails)
            {
                HashSet<Edge<Domain.DTOs.UserDto>> enumerable = new HashSet<Edge<Domain.DTOs.UserDto>>(from conversationEmailSet1 in conversationEmails.Where(x => x.ConverationId == email.ConverationId).SelectMany(x => x.Emails)
                    from conversationEmailSet2 in conversationEmails.Where(x => x.ConverationId == email.ConverationId).SelectMany(x => x.Emails)
                    where conversationEmailSet1.Sender.Id < conversationEmailSet2.Sender.Id // only one direction
                    select new Edge<Domain.DTOs.UserDto>()
                    {
                        Node1 = new Node<Domain.DTOs.UserDto>()
                        {
                            Id = conversationEmailSet1.Sender.Id,
                            NodeElement = new Domain.DTOs.UserDto()
                            {
                                Name = conversationEmailSet1.Sender.Name
                            } 
                           
                        },
                        Node2 = new Node<Domain.DTOs.UserDto>()
                        {
                            Id = conversationEmailSet2.Sender.Id,
                            NodeElement = new Domain.DTOs.UserDto()
                            {
                                Name = conversationEmailSet2.Sender.Name
                            }
                        },
                        Weight = 2
                    });

                edgesWithDuplicates.UnionWith(enumerable);
            }
            

            HashSet<Edge<Domain.DTOs.UserDto>> edges = new HashSet<Edge<Domain.DTOs.UserDto>> (edgesWithDuplicates.Distinct(new DistinctItemComparer()).ToList());

            return edges;
        }

        public List<int> GetConversations()
        {
            List<int> conversationIds = (from conversation in _context.Conversations
                select conversation.ConversationId).Distinct().ToList();

            return conversationIds;
        }
    }

    public class DistinctItemComparer : IEqualityComparer<Edge<Domain.DTOs.UserDto>>
    {

        public bool Equals(Edge<UserDto> x, Edge<UserDto> y)
        {
            return y != null && (x != null && (x.Node1.Id == y.Node1.Id && x.Node2.Id == y.Node2.Id));
        }

        public int GetHashCode(Edge<UserDto> obj)
        {
            return obj.Node1.Id.GetHashCode() ^
                   obj.Node2.Id.GetHashCode();
        }
    }

}
