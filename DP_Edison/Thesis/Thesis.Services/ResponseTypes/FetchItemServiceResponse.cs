namespace Thesis.Services.ResponseTypes
{
    public class FetchItemServiceResponse<T> : ServiceResponse
    {
        public T Item { get; internal set; }
    }
}
