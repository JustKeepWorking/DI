using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI
{
    public class DIContainer
    {
        private static readonly Dictionary<Type, Object> RegisteredModules = new Dictionary<Type, object>();

        public static void SetModule<IInterface, TModule>()
        {
            SetModule(typeof(IInterface), typeof(TModule));
        }

        public static T GetModule<T>()
        {
            return (T)GetModule(typeof(T));
        }

        public static void SetModule(Type interfaceType, Type moduleType)
        {
            if (!interfaceType.IsAssignableFrom(moduleType))
            {
                throw new Exception("Wrong module type");
            }
            //find first constructor
            var firstConstructor = moduleType.GetConstructors()[0];
            Object module = null;
            if (!firstConstructor.GetParameters().Any())
            {
                module = firstConstructor.Invoke(null); //new Database, new Logger
            }
            else
            {
                //get parameter of constructor
                var constructorParameters = firstConstructor.GetParameters(); //IDatabase, ILogger
                var moduleDependencies = new List<Object>();

                foreach (var parameter in constructorParameters)
                {
                    var dependency = GetModule(parameter.ParameterType);
                    moduleDependencies.Add(dependency);
                }
                module = firstConstructor.Invoke(moduleDependencies.ToArray());
            }
            RegisteredModules.Add(interfaceType, module);
        }

        public static Object GetModule(Type interfaceType)
        {
            if (RegisteredModules.ContainsKey(interfaceType))
            {
                return RegisteredModules[interfaceType];
            }
            throw new Exception("Module not register");
        }
    }
}
