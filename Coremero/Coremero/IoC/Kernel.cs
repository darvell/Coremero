using System;
using System.Collections.Generic;
using System.Linq;
using SimpleInjector;

namespace Coremero
{
    /// <summary>
    /// IoC wrapper. (Currently wraps SimpleInjector.)
    /// TODO: Consider making the kernel itself NOT global but have a singleton instance.
    /// Right now for our needs this is good but unit testability will get funky.
    /// </summary>

    public static class Kernel
    {
        private static Container _internalContainer = new Container();


        public static Func<Type, object> GetInstance = (service) => { return _internalContainer.GetInstance(service); };

        public static Func<Type, IEnumerable<object>> GetAllInstances = service => { return _internalContainer.GetAllInstances(service); };

        public static T Get<T>()
        {
            return (T)GetInstance(typeof(T));
        }

        public static IEnumerable<T> GetAll<T>()
        {
            return GetAllInstances(typeof(T)).Cast<T>();
        }

        public static void Register<T>(T registrationItem)
        {
            // todo: allow different lifecycles
        }

        public static void RegisterSingleton<T>(T registrationItem) where T : class
        {
            _internalContainer.RegisterSingleton(typeof(T), registrationItem);
        }

        public static void RegisterFactory<T>(Func<T> factory) where T : class
        {
//            var producer = Lifestyle.Singleton.CreateProducer(typeof(T), factory, _internalContainer);
        }
    }
}
