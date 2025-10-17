namespace Engine.Api.Internal
{
    public interface IClientLockService
    {
        bool IsLocked(string ipAddress);
        ClientLockStatus Success(string ipAddress);
        ClientLockStatus Fail(string ipAddress);
    }
}