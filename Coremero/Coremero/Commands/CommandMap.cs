using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Coremero.Commands
{
    internal class CommandMap
    {
        private Dictionary<CommandAttribute, Func<Task<IResult>>> _commandMap = new Dictionary<CommandAttribute, Func<Task<IResult>>>();

        private List<Type> _validCommandReturnTypes = new List<Type>()
        {
            typeof(int),
            typeof(string),
        };

        public void RegisterPluginCommands(IPlugin plugin)
        {
            Type pluginType = plugin.GetType();
            foreach (var methodInfo in pluginType.GetRuntimeMethods())
            {
                CommandAttribute attribute = methodInfo.GetCustomAttribute<CommandAttribute>();
                if (attribute != null)
                {
                    // Check what it returns.
                    if (!_validCommandReturnTypes.Contains(methodInfo.ReturnType))
                    {
                        Debug.Fail($"Command {attribute.Name} has an invalid return type of {methodInfo.ReturnType.FullName}");
                        continue;
                    }

                    // Check if it's async.
                    bool isAsync = methodInfo.GetCustomAttribute<AsyncStateMachineAttribute>() != null;


                }
            }
            

        }
    }
}
