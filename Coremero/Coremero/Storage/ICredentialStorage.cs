using System;
using System.Collections.Generic;
using System.Text;

namespace Coremero.Storage
{
    public interface ICredentialStorage
    {
        string GetKey(string keyName, string defaultKey);
    }
}
