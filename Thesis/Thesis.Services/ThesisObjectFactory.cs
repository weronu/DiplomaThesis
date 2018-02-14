using System;
using Thesis.Services.Interfaces;

namespace Thesis.Services
{
    public class ThesisObjectFactory : IThesisObjectFactory
    {
        public T Get<T>(params object[] args)
        {
            return (T)Activator.CreateInstance(typeof(T), args);
        }
    }
}
