using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Coremero
{
    public interface IReactableMessage : IMessage
    {
        Task React(string emoji);
    }
}