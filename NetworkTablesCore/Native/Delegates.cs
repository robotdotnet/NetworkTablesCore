namespace NetworkTables.Native
{
    internal delegate void EntryListenerFunction(int uid, string key, object value, NotifyFlags flags);

    internal delegate void ConnectionListenerFunction(int uid, bool connected, ConnectionInfo conn);

    internal delegate byte[] RpcFunction(string name, byte[] paramsString);

    /// <summary>
    /// Delegate called from the native logger, which is set using <see cref="CoreMethods.SetLogger"/>.
    /// </summary>
    /// <param name="level">The level of the notification</param>
    /// <param name="file">The file that the notification came from</param>
    /// <param name="line">The line that the notification came from</param>
    /// <param name="message">The log message</param>
    public delegate void LoggerFunction(int level, string file, int line, string message);
}
