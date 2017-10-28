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
    }
}