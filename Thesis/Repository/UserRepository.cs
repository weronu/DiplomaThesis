using System.Linq;
using Repository.MSSQL.Interfaces;

namespace Repository.MSSQL
{
    public class UserRepository : RepositoryBase, IUserRepository
    {
        private ThesisDbContext _dbContext;

        public UserRepository(ThesisDbContext _dbContext) : base(_dbContext)
        {
            this._dbContext = _dbContext;   
        }

        public int GetNodeIdByUserName(string name)
        {
            return (from user in _dbContext.Users
                where user.Name == name
                select user.Id).FirstOrDefault();
        }
    }
}