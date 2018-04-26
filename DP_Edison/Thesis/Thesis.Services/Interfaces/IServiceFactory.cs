namespace Thesis.Services.Interfaces
{
    public interface IServiceFactory
    {
        T GetService<T>(params object[] args) where T : class;
    }
}
