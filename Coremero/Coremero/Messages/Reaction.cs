using System.Collections.Generic;
using System.Linq;

namespace Coremero.Messages
{
    public class Reaction
    {
        public string Emoji { get; private set; }
        public List<IUser> Reactors { get; private set; }

        public Reaction(string emoji, params IUser[] users)
        {
            Emoji = emoji;
            Reactors = users.ToList();
        }
    }
}