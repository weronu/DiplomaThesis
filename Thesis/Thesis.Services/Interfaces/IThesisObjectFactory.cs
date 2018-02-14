namespace Thesis.Services.Interfaces
{
    public interface IThesisObjectFactory
    {
        T Get<T>(params object[] args);
    }
}
