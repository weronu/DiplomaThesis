using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Domain.DomainClasses;
using Repository.MSSQL.Interfaces;

namespace Repository.MSSQL
{
    public class UserRepository : RepositoryBase, IUserRepository
    {
        private readonly ThesisDbContext _dbContext;

        public UserRepository(ThesisDbContext _dbContext) : base(_dbContext)
        {
            this._dbContext = _dbContext;   
        }

        public List<DataPoint> GetTenMostUsedEmailDomains(DateTime fromDate, DateTime toDate)
        {
            string sql = @"SELECT TOP 10 substring(ue.Email, CHARINDEX(char(64), ue.Email) + 1, len(ue.Email) - CHARINDEX(char(64), ue.Email)) AS label, CAST(COUNT(*) AS FLOAT) as y
                           FROM EmailMessages em
                           INNER JOIN Users u on u.Id = em.SenderId
                           INNER JOIN UserEmails ue on ue.UserId = u.Id
                           WHERE em.Id IN(SELECT EmailMessageId FROM Conversations) AND em.Sent BETWEEN @fromDate and @toDate
                           GROUP BY substring(ue.Email, CHARINDEX(char(64), ue.Email) + 1, len(ue.Email) - CHARINDEX(char(64), ue.Email))
                           ORDER BY COUNT(*) DESC";


            List<DataPoint> dataPoints = _dbContext.Database.SqlQuery<DataPoint>(sql, new SqlParameter("@fromDate", fromDate), new SqlParameter("@toDate", toDate)).ToList();

            double sum = dataPoints.Sum(x => x.y);

            foreach (DataPoint dataPoint in dataPoints)
            {
                dataPoint.y = Math.Round(((dataPoint.y * 100) / sum), 1);
            }

            return dataPoints;
        }
    }
}