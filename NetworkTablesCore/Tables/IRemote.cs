namespace NetworkTables.Tables
{
    /// <summary>
    /// Represents an object that has a remote connection.
    /// </summary>
    public interface IRemote
    {
        /// <summary>
        /// Register an object to listen for connection and disconnection events.
        /// </summary>
        /// <param name="listener">The listener to be registered</param>
        /// <param name="immediateNotify">True if the listener object should be notified of the current
        /// connection state immediately.</param>
        void AddConnectionListener(IRemoteConnectionListener listener, bool immediateNotify);

        /// <summary>
        /// Unregister a listener from connection events.
        /// </summary>
        /// <param name="listener">The listener to be unregistered.</param>
        void RemoveConnectionListener(IRemoteConnectionListener listener);

        /// <summary>
        /// Gets if the current object is connected.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Gets if the object is a server.
        /// </summary>
        bool IsServer { get; }
    }
}
