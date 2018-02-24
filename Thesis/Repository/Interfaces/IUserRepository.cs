namespace Repository.MSSQL.Interfaces
{
    public interface IUserRepository
    {
        int GetNodeIdByUserName(string name);
    }
}
