namespace NetworkTables
{
    /// <summary>
    /// This class contains all info needed for a given connection.
    /// </summary>
    public class ConnectionInfo
    {
        /// Gets the Remote Id of the Connection.
        public string RemoteId { get; }
        /// Gets the Remote Name of the Connection.
        public string RemoteName { get; }
        /// Gets the Remote Port of the Connection.
        public int RemotePort { get; }
        /// Gets the last update time of the Connection.
        public long LastUpdate { get; }
        /// Gets the Protocol Version of the Connection.
        public int ProtocolVersion { get; }

        internal ConnectionInfo(string rId, string rName, int rPort, long lastUpdate, int protocolVersion)
        {
            RemoteId = rId;
            RemoteName = rName;
            RemotePort = rPort;
            LastUpdate = lastUpdate;
            ProtocolVersion = protocolVersion;
        }
    }
}
