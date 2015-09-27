namespace NetworkTables.Tables
{
    public interface ITableListener
    {
        void ValueChanged(ITable source, string key, object value, NotifyFlags flags);
    }
}
