using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Coremero.Commands;
using Coremero.Context;
using Coremero.Messages;
using Coremero.Plugin;
using Coremero.Utilities;

namespace Coremero.Registry
{
    public class CommandRegistry
    {
        private readonly Dictionary<CommandAttribute, Func<IInvocationContext, IMessage, object>> _commandMap =
            new Dictionary<CommandAttribute, Func<IInvocationContext, IMessage, object>>();

        /// <summary>
        /// Registers all methods with the [Command] attribute in to the command map.
        /// </summary>
        /// <param name="plugin">The instance of the plugin to register in the command map.</param>
        public void Register(IPlugin plugin)
        {
            Type pluginType = plugin.GetType();
            foreach (var methodInfo in pluginType.GetRuntimeMethods())
            {
                CommandAttribute attribute = methodInfo.GetCustomAttribute<CommandAttribute>();
                if (attribute != null)
                {
                    // Don't trust the developer to remember to set HasSideEffects. Sorry.
                    if (methodInfo.ReturnType == typeof(void) || methodInfo.ReturnType == typeof(Task))
                    {
                        attribute.HasSideEffects = true;
                    }

                    // Check if the parameters are right.
                    ParameterInfo[] methodParams = methodInfo.GetParameters();
                    if (methodParams.Length > 0)
                    {
                        if (methodParams.Any(x =>
                        {
                            Type paramType = x.ParameterType;
                            return
                                !(paramType == (typeof(IInvocationContext)) || paramType == typeof(IMessage) ||
                                  paramType == typeof(string));
                        }))
                        {
                            Debug.Fail("Invalid parameter in command.");
                            continue;
                        }
                    }

                    // If attribute exists, clear.
                    // TODO: Check if no leaks after due to delegates being delegates.
                    if (_commandMap.ContainsKey(attribute))
                    {
                        _commandMap.Remove(attribute);
                    }

                    // Register command
                    _commandMap[attribute] = delegate (IInvocationContext context, IMessage message)
                    {
                        List<object> paramList = new List<object>();
                        foreach (var param in methodInfo.GetParameters())
                        {
                            if (param.ParameterType == typeof(IInvocationContext))
                            {
                                paramList.Add(context);
                            }
                            else if (param.ParameterType == typeof(IMessage))
                            {
                                paramList.Add(message);
                            }
                            else if (param.ParameterType == typeof(string))
                            {
                                paramList.Add(message.Text.TrimCommand());
                            }
                            else if (param.ParameterType == typeof(int))
                            {
                                int value = int.MinValue;
                                int.TryParse(message.Text.TrimCommand(), out value);
                                paramList.Add(value);
                            }
                            else if (param.ParameterType == typeof(int?))
                            {
                                if (int.TryParse(message.Text.TrimCommand(), out int value))
                                {
                                    paramList.Add(value);
                                }
                                else
                                {
                                    paramList.Add(null);
                                }
                            }
                        }
                        return methodInfo.Invoke(plugin, paramList.Count == 0 ? null : paramList.ToArray());
                    };
                }
            }
        }

        /// <summary>
        /// Retrieves the command attribute for a command string.
        /// </summary>
        /// <param name="commandName">The string to search.</param>
        /// <returns>CommandAttribute if found else null.</returns>
        private CommandAttribute GetCommand(string commandName)
        {
            CommandAttribute selectedCommand = null;
            int nearestEditDistance = int.MaxValue;
            foreach (var cmd in _commandMap.Keys)
            {
                if (cmd.Name.Equals(commandName))
                {
                    selectedCommand = cmd;
                    break;
                }
                else if (cmd.Name.StartsWith(commandName))
                {
                    int edit = cmd.Name.DamerauLevenshteinDistance(commandName, 10);
                    if (nearestEditDistance > edit)
                    {
                        nearestEditDistance = edit;
                        selectedCommand = cmd;
                    }
                }
            }
            return selectedCommand;
        }

        public async Task<IMessage> ExecuteCommandAsync(string commandName, IInvocationContext context, IMessage message)
        {
            var selectedCommand = GetCommand(commandName);

            if (selectedCommand == null)
            {
                return null;
            }

            // Validate context allows execution.
            if (context?.User?.Permissions < selectedCommand.MinimumPermissionLevel)
            {
                return null;
            }

            return await Task.Run(async () =>
            {
                var result = _commandMap[selectedCommand](context, message);

                // Check if the command is actually a task, if so, start that bad boy up and return result.
                if (result is Task task)
                {
                    await task.ConfigureAwait(false);
                    result = result.GetType().GetRuntimeProperty("Result")?.GetValue(result);

                    // The downside of using reflection to get the result is that we have to check this way if it was a void task.
                    if (result?.GetType().Name.Contains("VoidTaskResult") == true)
                    {
                        result = null;
                    }
                }

                if (result == null)
                {
                    return Message.Create(string.Empty);
                }

                if (result is IMessage resultMessage)
                {
                    return resultMessage;
                }

                return Message.Create(result.ToString());
            });
        }

        public IMessage ExecuteCommand(string commandName, IInvocationContext context, IMessage message)
        {
            return ExecuteCommandAsync(commandName, context, message).Result;
        }

        public bool IsCommandComplexOrNull(string commandName)
        {
            var cmd = GetCommand(commandName);
            return cmd?.HasSideEffects ?? true;
        }

        public bool Exists(string commandName)
        {
            return GetCommand(commandName) != null;
        }

        public string GetHelp(string commandName)
        {
            return GetCommand(commandName)?.Help;
        }

        public List<CommandAttribute> CommandAttributes
        {
            get { return _commandMap.Keys.ToList(); }
        }
    }
}