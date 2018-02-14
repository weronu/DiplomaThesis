using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;
using System.Web.Mvc;
using IDependencyResolver = System.Web.Mvc.IDependencyResolver;

namespace Thesis.Web
{
    public sealed class SimpleInjectorDependencyResolver : IDependencyResolver,
        System.Web.Http.Dependencies.IDependencyResolver, IDisposable
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SimpleInjectorDependencyResolver" /> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="container" /> is a null
        ///     reference.
        /// </exception>
        public SimpleInjectorDependencyResolver(Container container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            Container = container;
        }

        /// <summary>Gets the container.</summary>
        /// <value>The <see cref="Container" />.</value>
        public Container Container { get; }

        private IServiceProvider ServiceProvider => Container;

        /// <summary>Resolves singly registered services that support arbitrary object creation.</summary>
        /// <param name="serviceType">The type of the requested service or object.</param>
        /// <returns>The requested service or object.</returns>
        public object GetService(Type serviceType)
        {
            // By calling GetInstance instead of GetService when resolving a controller, we prevent the
            // container from returning null when the controller isn't registered explicitly and can't be
            // created because of an configuration error. GetInstance will throw a descriptive exception
            // instead. Not doing this will cause MVC to throw a non-descriptive "Make sure that the 
            // controller has a parameterless public constructor" exception.
            if (!serviceType.IsAbstract && typeof(IController).IsAssignableFrom(serviceType))
            {
                return Container.GetInstance(serviceType);
            }

            return ServiceProvider.GetService(serviceType);
        }

        ///// <summary>Resolves multiply registered services.</summary>
        ///// <param name="serviceType">The type of the requested services.</param>
        ///// <returns>The requested services.</returns>
        //public IEnumerable<object> GetServices(Type serviceType)
        //{
        //    return Container.GetAllInstances(serviceType);
        //}

        IDependencyScope System.Web.Http.Dependencies.IDependencyResolver.BeginScope()
        {
            return this;
        }
        /// <summary>Retrieves a service from the scope.</summary>
        /// <param name="serviceType">The service to be retrieved.</param>
        /// <returns>The retrieved service.</returns>
        object IDependencyScope.GetService(Type serviceType)
        {
            // By calling GetInstance instead of GetService when resolving a controller, we prevent the
            // container from returning null when the controller isn't registered explicitly and can't be
            // created because of an configuration error. GetInstance will throw a descriptive exception
            // instead. Not doing this will cause Web API to throw a non-descriptive "Make sure that the 
            // controller has a parameterless public constructor" exception.
            if (!serviceType.IsAbstract && typeof(IHttpController).IsAssignableFrom(serviceType))
            {
                return this.Container.GetInstance(serviceType);
            }

            return this.ServiceProvider.GetService(serviceType);
        }

        IEnumerable<object> IDependencyScope.GetServices(Type serviceType)
        {
            var collectionType = typeof(IEnumerable<>).MakeGenericType(serviceType);

            var services = (IEnumerable<object>)ServiceProvider.GetService(collectionType);

            return services ?? Enumerable.Empty<object>();
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            var collectionType = typeof(IEnumerable<>).MakeGenericType(serviceType);

            var services = (IEnumerable<object>)ServiceProvider.GetService(collectionType);

            return services ?? Enumerable.Empty<object>();
        }

        public void Dispose()
        {
        }
    }
}

