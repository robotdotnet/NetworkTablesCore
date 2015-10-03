namespace NetworkTables.Tables
{
    /// <summary>
    /// A listener that listens for connection changes in an <see cref="IRemote"/> object.
    /// </summary>
    public interface IRemoteConnectionListener
    {
        /// <summary>
        /// Called when an <see cref="IRemote"/> is connected
        /// </summary>
        /// <param name="remote">The object that connected.</param>
        void Connected(IRemote remote);

        /// <summary>
        /// Called when an <see cref="IRemote"/> is disconnected.
        /// </summary>
        /// <param name="remote">The object that disconnected.</param>
        void Disconnected(IRemote remote);
    }
}
