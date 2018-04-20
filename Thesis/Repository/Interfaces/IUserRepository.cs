using System.Collections.Generic;
using Domain.DomainClasses;

namespace Repository.MSSQL.Interfaces
{
    public interface IUserRepository
    {
        int GetNodeIdByUserName(string name);
        List<DataPoint> GetTenMostUsedEmailDomains();
    }
}
