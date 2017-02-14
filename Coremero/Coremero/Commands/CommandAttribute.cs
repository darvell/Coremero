using System;
using System.Collections.Generic;
using System.Linq;


namespace Coremero.Commands
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        /// <summary>
        /// Name of the command.
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// Description for using the command.
        /// </summary>
        public string Help { get; set; }
        
        /// <summary>
        /// A list of arguments.
        /// </summary>
        public List<String> Arguments { get; set; }

        /// <summary>
        /// Mininum permission level required to execute the command.
        /// </summary>
        public UserPermission MinimumPermissionLevel { get; set; } = UserPermission.Normal;

        /// <summary>
        /// True if the command works with external state (e.g. direct client manipulation) https://en.wikipedia.org/wiki/Side_effect_(computer_science)
        /// </summary>
        public bool HasSideEffects { get; set; }

        public CommandAttribute(string name)
        {
            this.Name = name;
        }

        public CommandAttribute(string name, string arguments) : this(name)
        {
            this.Arguments = arguments?.Split('|').Select(x => x.Trim()).ToList();
        }

    }
}
