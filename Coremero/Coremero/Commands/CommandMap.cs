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
    public class CommandMap
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

                    // Check if the parameters are right.
                    ParameterInfo[] methodParams = methodInfo.GetParameters();
                    if (methodParams.Length != 1)
                    {
                        Debug.Fail($"Command {attribute.Name} has an invalid parameter count of {methodParams.Length}.");
                        continue;
                    }

                    if (methodParams[0].ParameterType != typeof(IMessageContext))
                    {
                        Debug.Fail($"Command {attribute.Name} has an invalid parameter argument. You should only use MethodName(IMessageContext context).");
                        continue;
                    }

                    // Check if it's async.
                    // TODO: Handle properly
                    bool isAsync = methodInfo.GetCustomAttribute<AsyncStateMachineAttribute>() != null;

                    // 


                }
            }
        }

        public Task GetTaskForCommand(string commandName)
        {
            // TODO: One iteration.

            List<CommandAttribute> potentialCommands = _commandMap.Keys.Where(x => x.Name.StartsWith(commandName)).ToList();
            if (potentialCommands.Count != 1)
            {
                // TODO: Custom exception
                return null;
            }

            return Task.FromResult(true);
            //return _commandMap[potentialCommands[0]];
        }
    }
}
