using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Domain.DomainClasses;
using Domain.DTOs;
using Domain.GraphClasses;
using Repository.MSSQL.Interfaces;

namespace Repository.MSSQL
{
    public class GraphRepository : RepositoryBase, IGraphRepository
    {
        private readonly ThesisDbContext _context;

        public GraphRepository(ThesisDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Extracts all vertices from database.
        /// </summary>
        public HashSet<Node<UserDto>> ExtractNodesFromDatabase()
        {
            return new HashSet<Node<UserDto>>(from user in _context.Users
                                             select new Node<UserDto>()
                                             {
                                                 Id = user.Id
                                             });
        }        

        public HashSet<Node<UserDto>> ExtractNodesFromEdges(HashSet<Edge<UserDto>> edges)
        {
            HashSet<Node<UserDto>> vertices = new HashSet<Node<UserDto>>();

            foreach (Edge<UserDto> edge in edges)
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

            return new HashSet<Node<UserDto>>(vertices.Distinct().ToList());

        }

        public HashSet<Node<UserDto>> ExtractNodesFromConversations()
        {
            // extracting conversations from database
            HashSet<ConversationEmails> conversationEmails = new HashSet<ConversationEmails>(from conversation in _context.Conversations
                                                                                             group conversation.EmailMessage by conversation.ConversationId into grp
                                                                                             select new ConversationEmails()
                                                                                             {
                                                                                                 ConverationId = grp.Key,
                                                                                                 Emails = grp.ToList()
                                                                                             });
            HashSet<Node<UserDto>> vertices = new HashSet<Node<UserDto>>(from conversationEmail in conversationEmails.SelectMany(x => x.Emails)
                                                                       group conversationEmail by conversationEmail.Sender
                into senders
                                                                       select new Node<UserDto>()
                                                                       {
                                                                           Id = senders.Key.Id
                                                                       });

            return vertices;
        }

        public HashSet<Edge<UserDto>> ExtractEdgesFromConversation()
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

            HashSet<Edge<UserDto>> edgesWithDuplicates = new HashSet<Edge<UserDto>>();

            foreach (ConversationEmails email in conversationEmails)
            {
                HashSet<Edge<UserDto>> enumerable = new HashSet<Edge<UserDto>>(
                                                                            from conversationEmailSet1 in conversationEmails
                                                                                .Where(x => x.ConverationId == email.ConverationId)
                                                                                .SelectMany(x => x.Emails)
                    from conversationEmailSet2 in conversationEmails.Where(x => x.ConverationId == email.ConverationId).SelectMany(x => x.Emails)
                    where conversationEmailSet1.Sender.Id < conversationEmailSet2.Sender.Id // only one direction
                    select new Edge<UserDto>()
                    {
                        Node1 = new Node<UserDto>()
                        {
                            Id = conversationEmailSet1.Sender.Id,
                            NodeElement = new UserDto()
                            {
                                Id = conversationEmailSet1.Sender.Id,
                                Name = conversationEmailSet1.Sender.Name
                            } 
                           
                        },
                        Node2 = new Node<UserDto>()
                        {
                            Id = conversationEmailSet2.Sender.Id,
                            NodeElement = new UserDto()
                            {
                                Id = conversationEmailSet2.Sender.Id,
                                Name = conversationEmailSet2.Sender.Name
                            }
                        },
                        Weight = 2
                    });

                edgesWithDuplicates.UnionWith(enumerable);
            }
            

            HashSet<Edge<UserDto>> edges = new HashSet<Edge<UserDto>> (edgesWithDuplicates.Distinct(new DistinctItemComparer()).ToList());

            return edges;
        }

        public HashSet<Edge<UserDto>> ExtractEdgesFromConversation(HashSet<ConversationEmails> conversationEmails)
        {
            HashSet<Edge<UserDto>> edgesWithDuplicates = new HashSet<Edge<UserDto>>();

            foreach (ConversationEmails email in conversationEmails)
            {
                HashSet<Edge<UserDto>> enumerable = new HashSet<Edge<UserDto>>(
                                                                            from conversationEmailSet1 in conversationEmails
                                                                                .Where(x => x.ConverationId == email.ConverationId)
                                                                                .SelectMany(x => x.Emails)
                                                                            from conversationEmailSet2 in conversationEmails.Where(x => x.ConverationId == email.ConverationId).SelectMany(x => x.Emails)
                                                                            where conversationEmailSet1.Sender.Id < conversationEmailSet2.Sender.Id // only one direction
                                                                            select new Edge<UserDto>()
                                                                            {
                                                                                Node1 = new Node<UserDto>()
                                                                                {
                                                                                    Id = conversationEmailSet1.Sender.Id,
                                                                                    NodeElement = new UserDto()
                                                                                    {
                                                                                        Id = conversationEmailSet1.Sender.Id,
                                                                                        Name = conversationEmailSet1.Sender.Name
                                                                                    }

                                                                                },
                                                                                Node2 = new Node<UserDto>()
                                                                                {
                                                                                    Id = conversationEmailSet2.Sender.Id,
                                                                                    NodeElement = new UserDto()
                                                                                    {
                                                                                        Id = conversationEmailSet2.Sender.Id,
                                                                                        Name = conversationEmailSet2.Sender.Name
                                                                                    }
                                                                                },
                                                                                Weight = 2
                                                                            });

                edgesWithDuplicates.UnionWith(enumerable);
            }


            HashSet<Edge<UserDto>> edges = new HashSet<Edge<UserDto>>(edgesWithDuplicates.Distinct(new DistinctItemComparer()).ToList());

            return edges;
        }

        public void ImportXmlFile(string pathToFile)
        {
            const string sql = "EXEC sp_ParseXML @fileName";
            _context.Database.ExecuteSqlCommand(sql, new SqlParameter("@fileName", pathToFile));
        }

        public void ExtractConversations()
        {
            const string sql = "EXEC sp_ExtractConversations";

            _context.Database.ExecuteSqlCommand(sql);
        }

        public void ClearDatabaseData()
        {
            const string sql = "EXEC sp_ClearDatabaseData";

            _context.Database.ExecuteSqlCommand(sql);
        }

        public List<BrokerageDto> GetTopTenBrokers(HashSet<Node<UserDto>> nodes)
        {
            List<User> users = _context.Users.ToList();

            List<BrokerageDto> topTenBrokers = (from user in users 
                join node in nodes on user.Id equals node.Id
                select new BrokerageDto()
                {
                    UserId = user.Id,
                    Name = user.RawSenderName.ToUpper(),
                    Coordinator = node.Brokerage.Coordinator,
                    Gatepeeker = node.Brokerage.Gatepeeker,
                    Itinerant = node.Brokerage.Itinerant,
                    Liaison = node.Brokerage.Liaison,
                    Representative = node.Brokerage.Representative,
                    TotalBrokerageScore = node.Brokerage.TotalBrokerageScore
                }).OrderByDescending(x => x.TotalBrokerageScore).Take(10).ToList();
            return topTenBrokers;
        }
    }

    public class DistinctItemComparer : IEqualityComparer<Edge<UserDto>>
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
