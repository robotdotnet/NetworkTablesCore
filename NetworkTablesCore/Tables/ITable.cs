using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NetworkTablesCore.Tables
{
    public interface ITable
    {
        /// <summary>
        /// Determines whether the given key is in this table.
        /// </summary>
        /// <param name="key">The key to search for</param>
        /// <returns>If the table has a value assignend to the given key</returns>
        bool ContainsKey(string key);

        /// <summary>
        /// Determines whether there exists a non-empty subtable for this key in this table.
        /// </summary>
        /// <param name="key">The key to search for</param>
        /// <returns>If there is a subtable with the key which contains at least one key/subtable of its own</returns>
        bool ContainsSubTable(string key);

        /// <summary>
        /// Gets the subtable in this table for the given name.
        /// </summary>
        /// <param name="key">The name of the table relative to this one.</param>
        /// <returns>A sub table relative to this one</returns>
        ITable GetSubTable(string key);

        HashSet<string> GetKeys(int types);

        HashSet<string> GetKeys();

        HashSet<string> GetSubTables();  

        void SetPersistent(string key);

        void ClearPersistent(string key);

        bool IsPersistent(string key);

        void SetFlags(string key, int flags);

        void ClearFlags(string key, int flags);

        int GetFlags(string key);

        void Delete(string key);


        /// <summary>
        /// Gets the value associated with a key as an object
        /// </summary>
        /// <param name="key">The key of the value to look up</param>
        /// <returns>The value associated with the given key, or null if the key does not exist.</returns>
        object GetValue(string key);

        /// <summary>
        /// Put a value in the table.
        /// </summary>
        /// <param name="key">The key to be assigned to</param>
        /// <param name="value">The value that will be assigned</param>
        /// <returns>False if the table key already exists with a different type</returns>
        bool PutValue(string key, object value);

        /// <summary>
        /// Put a number in the table.
        /// </summary>
        /// <param name="key">The key to be assigned to</param>
        /// <param name="value">The value that will be assigned</param>
        /// <returns>False if the table key already exists with a different type</returns>
        bool PutNumber(string key, double value);

        /// <summary>
        /// Gets the number associated with the given name.
        /// </summary>
        /// <param name="key">The key to look up</param>
        /// <param name="defaultValue">The value to be returned if no value is found</param>
        /// <returns>The value associated with the given key, or the given default value if there is no value associated with the key</returns>
        double GetNumber(string key, double defaultValue);

        double GetNumber(string key);

        /// <summary>
        /// Put a string in the table.
        /// </summary>
        /// <param name="key">The key to be assigned to</param>
        /// <param name="value">The value that will be assigned</param>
        /// <returns>False if the table key already exists with a different type</returns>
        bool PutString(string key, string value);

        /// <summary>
        /// Gets the string associated with the given name.
        /// </summary>
        /// <param name="key">The key to look up</param>
        /// <param name="defaultValue">The value to be returned if no value is found</param>
        /// <returns>The value associated with the given key, or the given default value if there is no value associated with the key</returns>
        string GetString(string key, string defaultValue);

        string GetString(string key);

        /// <summary>
        /// Put a boolean in the table.
        /// </summary>
        /// <param name="key">The key to be assigned to</param>
        /// <param name="value">The value that will be assigned</param>
        /// <returns>False if the table key already exists with a different type</returns>
        bool PutBoolean(string key, bool value);

        /// <summary>
        /// Gets the boolean associated with the given name.
        /// </summary>
        /// <param name="key">The key to look up</param>
        /// <param name="defaultValue">The value to be returned if no value is found</param>
        /// <returns>The value associated with the given key, or the given default value if there is no value associated with the key</returns>
        bool GetBoolean(string key, bool defaultValue);

        bool GetBoolean(string key);

        //TODO: Add Commenting to Array Types

        bool PutBooleanArray(string key, bool[] value);
        bool[] GetBooleanArray(string key);
        bool[] GetBooleanArray(string key, bool[] defaultValue);

        bool PutNumberArray(string key, double[] value);
        double[] GetNumberArray(string key);
        double[] GetNumberArray(string key, double[] defaultValue);

        bool PutStringArray(string key, string[] value);
        string[] GetStringArray(string key);
        string[] GetStringArray(string key, string[] defaultValue);

        /// <summary>
        /// Add a listener to changes to the table.
        /// </summary>
        /// <param name="listener">The listener to add</param>
        /// <param name="immediateNotify">If true then this listener will be notified of all current entries (marked as new)</param>
        void AddTableListener(ITableListener listener, bool immediateNotify = false);

        /// <summary>
        /// Add a listener for changes to a specific key in the table.
        /// </summary>
        /// <param name="key">The key to listen for</param>
        /// <param name="listener">The listener to add</param>
        /// <param name="immediateNotify">If true then this listener will be notified of all current entries (marked as new)</param>
        void AddTableListener(string key, ITableListener listener, bool immediateNotify);

        void AddSubTableListener(ITableListener listener);

        /// <summary>
        /// Remove a listener from receiving table events.
        /// </summary>
        /// <param name="listener">The listener to be removed.</param>
        void RemoveTableListener(ITableListener listener);
    }
}
