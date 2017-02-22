namespace Coremero.Storage
{
    public interface ICredentialStorage
    {
        string GetKey(string keyName, string defaultKey);
    }
}