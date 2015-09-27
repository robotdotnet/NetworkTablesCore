namespace NetworkTables.Tables
{
    public interface IRemote
    {
        void AddConnectionListener(IRemoteConnectionListener listener, bool immediateNotify);

        void RemoveConnectionListener(IRemoteConnectionListener listener);

        bool IsConnected();
        bool IsServer();
    }
}
