using System;


namespace Coremero.Commands
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        public readonly string Name;
        public string Help { get; set; }

        public CommandAttribute(string name)
        {
            this.Name = name;
        }
    }
}
