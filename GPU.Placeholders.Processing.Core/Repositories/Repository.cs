namespace GPU.Placeholders.Processing.Core.Repositories
{
    public abstract class Repository : IDisposable
    {
        protected readonly string _connectionString;
        protected readonly Database _database;

        protected Repository(string connectionString)
        {
            _connectionString = connectionString;
            _database = new Database(connectionString);
        }

        protected Repository(Database database)
        {
            _connectionString = database.ConnectionString;
            _database = database;
        }

        public void Dispose()
        {
            
        }
    }
}
