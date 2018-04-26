using System;
using System.Collections.Generic;
using Domain.DomainClasses;

namespace Repository.MSSQL.Interfaces
{
    public interface IUserRepository
    {
        List<DataPoint> GetTenMostUsedEmailDomains(DateTime fromDate, DateTime toDate);
    }
}
