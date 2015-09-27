namespace NetworkTables.Tables
{
    public interface IRemoteConnectionListener
    {
        void Connected(IRemote remote);
        void Disconnected(IRemote remote);
    }
}
