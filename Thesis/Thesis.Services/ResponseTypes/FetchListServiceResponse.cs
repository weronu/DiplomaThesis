using System.Collections.Generic;

namespace Thesis.Services.ResponseTypes
{
    public class FetchListServiceResponse<T> : ServiceResponse
    {
        public HashSet<T> Items { get; internal set; }

        public FetchListServiceResponse()
        {
            Items = new HashSet<T>();
        }
    }
}
