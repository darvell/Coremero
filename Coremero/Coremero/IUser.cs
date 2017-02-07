namespace Coremero
{
    public interface IUser : ISendable
    {
        string Name { get; }
        string Mention { get; }
        UserPermission Permissions { get; }
    }
}