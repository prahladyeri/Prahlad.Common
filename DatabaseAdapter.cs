/**
 * DatabaseAdapter.cs
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using System.Collections.Generic;
using System.Data;

namespace Prahlad.Common
{
    public interface IDatabaseAdapter
    {
        void Connect(string connectionString);
        void Disconnect();
        List<string> GetDatabases();
        List<string> GetTables(string database);
        DataTable ExecuteQuery(string sql, Dictionary<string, object> parameters = null);
        int ExecuteNonQuery(string sql, Dictionary<string, object> parameters = null);

        string BuildInsert(string table, Dictionary<string, object> fields);
        string BuildUpdate(string table, int idValue, Dictionary<string, object> fields);

    }

    public class DatabaseAdapter
    {

    }
}
