using Dapper;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace GPU.Placeholders.Processing.Core
{
    public class Database : IDisposable
    {
        private readonly string _connectionString;
        private IDbConnection? _dbConnection;

        public string ConnectionString { get => _connectionString; }


        public Database(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Dispose()
        {
            CloseConnection();

            _dbConnection?.Dispose();
        }


        public virtual IDbConnection CreateOrGetCurrentConnection()
        {
            if (_dbConnection is null)
                _dbConnection = new SqlConnection(_connectionString);

            return _dbConnection;
        }

        public virtual Database OpenConnection()
        {
            var conn = CreateOrGetCurrentConnection();

            try
            {
                if (conn.State == ConnectionState.Closed || conn.State == ConnectionState.Broken)
                    conn.Open();
            }
            catch
            {
                Thread.Sleep(300);

                if (conn.State == ConnectionState.Closed || conn.State == ConnectionState.Broken)
                    conn.Open();
            }

            return this;
        }

        public virtual Database CloseConnection()
        {
            var conn = CreateOrGetCurrentConnection();

            if (conn?.State != ConnectionState.Closed)
                conn?.Close();

            return this;
        }


        public DataTable ToDataTable<T>(IEnumerable<T> data)
        {
            var properties = typeof(T).GetProperties();
            var table = new DataTable();

            foreach (var property in properties)
                table.Columns.Add(property.Name, property.PropertyType);

            foreach (var item in data)
            {
                var row = table.NewRow();

                foreach (var property in properties)
                {
                    var value = property.GetValue(item);
                    row[property.Name] = value;
                }

                table.Rows.Add(row);  
            }

            return table;
        }

        public IDictionary<string, string> GetDataAsDictionary(string sql, object? sqlParams = null)
        {
            var conn = CreateOrGetCurrentConnection();
            var result = new Dictionary<string, string>();

            OpenConnection();

            var reader = conn.ExecuteReader(sql, sqlParams);

            while (reader.Read())
                result.Add(reader.GetString(0), reader.GetString(1));

            reader.Close();
            reader.Dispose();

            return result;
        }

        public IEnumerable<T> GetData<T>(string sql, object? sqlParams = null) where T : new()
        {            
            var conn = CreateOrGetCurrentConnection();

            OpenConnection();
            IEnumerable<T> result = Enumerable.Empty<T>();

            try
            {
                result = conn.Query<T>(sql, sqlParams);
            }
            catch
            {
                Thread.Sleep(300);
                OpenConnection();
                result = conn.Query<T>(sql, sqlParams);
            }

            return result;
        }

        public int GetCount(string sql, object? sqlParams = null)
        {
            var conn = CreateOrGetCurrentConnection();

            OpenConnection();

            var result = conn.ExecuteScalar<int>(sql, sqlParams);

            return result;
        }

        public void BulkInsertWithStoreProcedure<T>(string procedurename, string parametername, string typename, IEnumerable<T> data)
        {
            var conn = CreateOrGetCurrentConnection();
            var datatable = ToDataTable(data);
            var param = new DynamicParameters();

            param.Add(parametername, datatable.AsTableValuedParameter(typename));

            OpenConnection();

            conn.Execute(procedurename, param, commandTimeout: 0);
        }
    }
}