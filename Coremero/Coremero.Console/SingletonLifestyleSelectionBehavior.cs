using System;
using System.Collections.Generic;
using System.Text;
using SimpleInjector;
using SimpleInjector.Advanced;

namespace Coremero.Console
{
    public class SingletonLifestyleSelectionBehavior : ILifestyleSelectionBehavior
    {
        public Lifestyle SelectLifestyle(Type serviceType, Type implementationType)
        {
            return Lifestyle.Singleton;
        }

        public Lifestyle SelectLifestyle(Type implementationType)
        {
            return Lifestyle.Singleton;
        }
    }
}
