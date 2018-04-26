using System;
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
                                Name = "User " + conversationEmailSet1.Sender.Id
                            } 
                           
                        },
                        Node2 = new Node<UserDto>()
                        {
                            Id = conversationEmailSet2.Sender.Id,
                            NodeElement = new UserDto()
                            {
                                Id = conversationEmailSet2.Sender.Id,
                                Name = "User " + conversationEmailSet2.Sender.Id
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
                                                                                        Name = "User " + conversationEmailSet1.Sender.Id
                                                                                    }

                                                                                },
                                                                                Node2 = new Node<UserDto>()
                                                                                {
                                                                                    Id = conversationEmailSet2.Sender.Id,
                                                                                    NodeElement = new UserDto()
                                                                                    {
                                                                                        Id = conversationEmailSet2.Sender.Id,
                                                                                        Name = "User " + conversationEmailSet2.Sender.Id
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
                    Name = "USER " + user.Id,
                    Coordinator = node.Brokerage.Coordinator,
                    Gatepeeker = node.Brokerage.Gatepeeker,
                    Itinerant = node.Brokerage.Itinerant,
                    Liaison = node.Brokerage.Liaison,
                    Representative = node.Brokerage.Representative,
                    TotalBrokerageScore = node.Brokerage.TotalBrokerageScore
                }).OrderByDescending(x => x.TotalBrokerageScore).Take(10).ToList();
            return topTenBrokers;
        }

        public NetworkStatisticsDto GetEmailNetworkStatistics(DateTime fromDate, DateTime toDate)
        {
            const string biggestEmailSender = @"SELECT u.Id FROM
                                            (
                                            SELECT TOP 1 em.SenderId
                                            FROM EmailMessages em
											WHERE em.Sent BETWEEN @fromDate and @toDate
                                            GROUP BY SenderId
                                            ORDER BY COUNT(em.SenderId) DESC
                                            ) t
                                            INNER JOIN Users u on t.SenderId = u.Id
                                            ";

            const string peekHour = @"SELECT TOP 1 cast(DATEPART(HOUR, Sent) as VARCHAR(2)) + ':00-'  + cast(DATEPART(HOUR, Sent)+1 as VARCHAR(2)) + ':00'
                                        FROM EmailMessages em
                                        WHERE em.Sent BETWEEN @fromDate and @toDate
                                        GROUP BY DATEPART(HOUR, Sent)
                                        ORDER BY COUNT(*) DESC";

            const string biggestNumberOfEmailsInConversations = @"SELECT TOP 1 COUNT(*)  
                                                            FROM Conversations c
															INNER JOIN EmailMessages em ON c.EmailMessageId = em.Id
															WHERE em.Sent BETWEEN @fromDate and @toDate
                                                            GROUP BY ConversationId
                                                            ORDER BY COUNT(*) DESC";

            const string emailCount = @"SELECT COUNT(*)
                                        FROM EmailMessages
                                        WHERE Sent BETWEEN @fromDate and @toDate";

            const string userCount = @"SELECT count(distinct u.id)
                                        FROM Users u 
                                        INNER JOIN EmailMessages em on u.Id = em.SenderId
                                        WHERE em.Sent BETWEEN @fromDate and @toDate";

            const string conversationCount = @"SELECT COUNT(DISTINCT c.ConversationId)
                                                FROM Conversations c
                                                INNER JOIN EmailMessages em on c.EmailMessageId = em.Id
                                                WHERE em.Sent BETWEEN @fromDate and @toDate";

            int biggestEmailSenderId = _context.Database.SqlQuery<int>(biggestEmailSender, new SqlParameter("@fromDate", fromDate), new SqlParameter("@toDate", toDate)).FirstOrDefault();

            NetworkStatisticsDto statisticsDto = new NetworkStatisticsDto
            {
                NumberOfUsers = _context.Database.SqlQuery<int>(userCount, new SqlParameter("@fromDate", fromDate), new SqlParameter("@toDate", toDate)).FirstOrDefault(),

                NumberOfEmails = _context.Database.SqlQuery<int>(emailCount, new SqlParameter("@fromDate", fromDate), new SqlParameter("@toDate", toDate)).FirstOrDefault(),

                NumberOfConversations = _context.Database.SqlQuery<int>(conversationCount, new SqlParameter("@fromDate", fromDate), new SqlParameter("@toDate", toDate)).FirstOrDefault(),

                BiggestEmailSender = "User " + biggestEmailSenderId,

                PeekHour = _context.Database.SqlQuery<string>(peekHour, new SqlParameter("@fromDate", fromDate), new SqlParameter("@toDate", toDate)).FirstOrDefault(),

                TheBiggestNumberOfEmailsInConversation = _context.Database.SqlQuery<int>(biggestNumberOfEmailsInConversations, new SqlParameter("@fromDate", fromDate), new SqlParameter("@toDate", toDate)).FirstOrDefault(),
            };

            return statisticsDto;
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
