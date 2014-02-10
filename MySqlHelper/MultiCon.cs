using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace MySql.MysqlHelper
{
    /// <summary>
    /// Opens and closes a connection for each query. When not doing transactions or working with memory tables
    /// </summary>
    public class MultiCon : Default
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options">Connection string helper instance</param>
        public MultiCon(ConnectionString options)
        {
            base.SetConnectionString(options);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        public MultiCon(string connectionString)
        {
            base.SetConnectionString(connectionString);
        }

        /// <summary>
        /// Inserts a row and returns last insertion id
        /// </summary>
        /// <param name="database">Destination database</param>
        /// <param name="table">Destination table</param>
        /// <param name="colData">Columns and their data</param>
        /// <param name="onDuplicateUpdate">If duplicate, update duplicate with new values</param>
        /// <returns>Returns last insertion ID</returns>
        public override long InsertRow(string database, string table, bool onDuplicateUpdate, params ColumnData[] colData)
        {
            using (MySqlConnection mysqlConnection = GetMysqlConnection())
            using (MySqlCommand mysqlCommand = mysqlConnection.CreateCommand())
                return base.InsertRow(mysqlCommand, database, table, onDuplicateUpdate, colData);
        }

        /// <summary>
        /// Inserts or updates a row and returns last insertion id. Generic data properties and data type must correspond to column names and column type
        /// </summary>
        /// <param name="database">Destination database</param>
        /// <param name="table">Destination table</param>
        /// <param name="onDuplicateUpdate">If duplicate, update duplicate with new values</param>
        /// <param name="data">Instance where properties and type match database structure</param>
        /// <returns>Returns last insertion ID</returns>
        public override long InsertRowGeneric<T>(string database, string table, bool onDuplicateUpdate, T data)
        {
            using (MySqlConnection mysqlConnection = GetMysqlConnection())
            using (MySqlCommand mysqlCommand = mysqlConnection.CreateCommand())
                return base.InsertRowGeneric<T>(mysqlCommand, database, table, onDuplicateUpdate, data);

        }

        /// <summary>
        /// Updates a row or rows
        /// </summary>
        /// <param name="database">Destination database</param>
        /// <param name="table">Destination table</param>
        /// <param name="where">Which row(s) to update, null = all</param>
        /// <param name="limit">amount of rows to update. 0 = all</param>
        /// <param name="colData">Columns and their data</param>
        /// <returns>Returns update count</returns>
        public override long UpdateRow(string database, string table, string where, int limit, params ColumnData[] colData)
        {
            using (MySqlConnection mysqlConnection = GetMysqlConnection())
            using (MySqlCommand mysqlCommand = mysqlConnection.CreateCommand())
                return base.UpdateRow(mysqlCommand, database, table, where, limit, colData);
        }

        /// <summary>
        /// Sends an entire collection to specified column
        /// </summary>
        public override long BulkSend(string database, string table, string column, IEnumerable<object> listData, bool onDuplicateUpdate)
        {
            using (MySqlConnection mysqlConnection = GetMysqlConnection())
            using (MySqlCommand mysqlCommand = mysqlConnection.CreateCommand())
                return base.BulkSend(mysqlCommand, database, table, column, listData, onDuplicateUpdate);
        }

        /// <summary>
        /// Sends an entire datatable to specified table. Make sure that column names of table correspond to database
        /// </summary>
        public override long BulkSend(string database, string table, DataTable dataTable, bool onDuplicateUpdate, int updateBatchSize = 100)
        {
            using (MySqlConnection mysqlConnection = GetMysqlConnection())
            using (MySqlCommand mysqlCommand = mysqlConnection.CreateCommand())
                return base.BulkSend(mysqlCommand, database, table, dataTable, onDuplicateUpdate, updateBatchSize);

        }

        /// <summary>
        /// Sends generic IEnumerable to specified table and database. Make sure that the Generic properties and data type correspond
        /// to database column name and column type
        /// </summary>
        /// <param name="database">Destination database</param>
        /// <param name="table">Destination table</param>
        /// <param name="listData"></param>
        /// <param name="onDuplicateUpdate"></param>
        public override long BulkSendGeneric<T>(string database, string table, IEnumerable<T> listData, bool onDuplicateUpdate)
        {
            using (MySqlConnection mysqlConnection = GetMysqlConnection())
            using (MySqlCommand mysqlCommand = mysqlConnection.CreateCommand())
                return base.BulkSendGeneric(mysqlCommand, database, table, listData, onDuplicateUpdate);
        }

        /// <summary>
        /// Returns a field from the server as a object
        /// </summary>
        public override object GetObject(string query, params ColumnData[] colData)
        {
            using (MySqlConnection mysqlConnection = GetMysqlConnection())
            using (MySqlCommand mysqlCommand = mysqlConnection.CreateCommand())
                return base.GetObject(mysqlCommand, query, colData);
        }

        /// <summary>
        /// Returns a field from the server as specified type using explicit type conversion.
        /// Will throw exception if type is wrong
        /// </summary>
        public T GetObject<T>(string query, bool parse = false, params ColumnData[] colData)
        {
            if (parse)
                return base.ParseObject<T>(GetObject(query, colData));
            else
                return (T)GetObject(query, colData);
        }

        /// <summary>
        /// Sends query to server
        /// </summary>
        public override int SendQuery(string query, params ColumnData[] colData)
        {
            using (MySqlConnection mysqlConnection = GetMysqlConnection())
            using (MySqlCommand mysqlCommand = mysqlConnection.CreateCommand())
                return base.SendQuery(mysqlCommand, query, colData);
        }

        /// <summary>
        /// Returns all selected data as a datatable
        /// </summary>
        public override DataTable GetDataTable(string query, params ColumnData[] colData)
        {
            using (MySqlConnection mysqlConnection = GetMysqlConnection())
            using (MySqlCommand mysqlCommand = mysqlConnection.CreateCommand())
                return base.GetDataTable(mysqlCommand, query, colData);
        }

        /// <summary>
        /// Returns two dimensional object array with data
        /// </summary>
        /// <returns>Two dimensional object array</returns>
        public object[,] GetDataTableAsObjectArray2d(string query, params ColumnData[] colData)
        {
            using (MySqlConnection mysqlConnection = GetMysqlConnection())
            using (MySqlCommand mysqlCommand = mysqlConnection.CreateCommand())
            {
                using (DataTable dt = base.GetDataTable(mysqlCommand, query, colData))
                {
                    object[,] returnData = new object[dt.Rows.Count, dt.Columns.Count];

                    for (int row = 0; row < dt.Rows.Count; row++)
                        for (int col = 0; col < dt.Columns.Count; col++)
                            returnData[row, col] = dt.Rows[row][col];

                    return returnData;
                }
            }
        }

        /// <summary>
        /// Returns a ienumerable of instances.  Instance property name and type must reflect table column name and type
        /// </summary>
        /// <typeparam name="T">Instance type</typeparam>
        public override IEnumerable<T> GetIEnumerable<T>(string query, params ColumnData[] colData)
        {
            using (MySqlConnection mysqlConnection = GetMysqlConnection())
            using (MySqlCommand mysqlCommand = mysqlConnection.CreateCommand())
                return base.GetIEnumerable<T>(mysqlCommand, query, colData);
        }

        /// <summary>
        /// Returns a idictionary of instances. Instance property name and type must reflect table column name and type
        /// </summary>
        /// <typeparam name="Y">Key type</typeparam>
        /// <typeparam name="T">Instance type</typeparam>
        public override IDictionary<Y, T> GetIDictionary<Y, T>(string keyColumn, string query, bool parseKey, params ColumnData[] colData)
        {
            using (MySqlConnection mysqlConnection = GetMysqlConnection())
            using (MySqlCommand mysqlCommand = mysqlConnection.CreateCommand())
                return base.GetIDictionary<Y, T>(mysqlCommand, keyColumn, query, parseKey, colData);
        }

        /// <summary>
        /// Returns rows of selected column
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="query">Select query</param>
        /// <param name="column">Return column number</param>
        /// <param name="parse">Parses the object of explicit conversion</param>
        /// <param name="colData">Parameters</param>
        /// <returns>Selected data</returns>
        public override IEnumerable<T> GetColumn<T>(string query, int column, bool parse, params ColumnData[] colData)
        {
            using (MySqlConnection mysqlConnection = GetMysqlConnection())
            using (MySqlCommand mysqlCommand = mysqlConnection.CreateCommand())
                return base.GetColumn<T>(mysqlCommand, query, column, parse, colData);
        }

        /// <summary>
        /// Returns rows of selected column
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="query">Select query</param>
        /// <param name="column">Return column name</param>
        /// <param name="parse">Parses the object of explicit conversion</param>
        /// <param name="colData">Parameters</param>
        /// <returns>Selected data</returns>
        public override IEnumerable<T> GetColumn<T>(string query, string column, bool parse, params ColumnData[] colData)
        {
            using (MySqlConnection mysqlConnection = GetMysqlConnection())
            using (MySqlCommand mysqlCommand = mysqlConnection.CreateCommand())
                return base.GetColumn<T>(mysqlCommand, query, column, parse, colData);
        }

        /// <summary>
        /// Returns the default connecition data
        /// </summary>
        private MySqlConnection GetMysqlConnection()
        {
            MySqlConnection mysqlConnection = new MySqlConnection(base.connectionString);
            if (!base.OpenConnection(mysqlConnection, 10)) throw new Exception("Unable to connect");
            return mysqlConnection;
        }
    }
}