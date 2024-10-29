using GPU.Placeholders.Processing.Core;
using GPU.Placeholders.Processing.Core.TableProcessors;
using Microsoft.Extensions.Logging;

namespace GPU.Placeholders.Processing.UnitsOfWork
{
    public abstract class PlaceholderProcessingUnitOfWork<TUnderlyingTableProcessor> : 
        IDisposable where TUnderlyingTableProcessor : ITableProcessor
    {
        protected readonly string _connectionString;
        protected readonly Database _database;
        protected readonly TableProcessorProxy<TUnderlyingTableProcessor> _processor;
        protected readonly ILogger? _logger;


        protected PlaceholderProcessingUnitOfWork(
            string connectionString, 
            TableProcessorProxy<TUnderlyingTableProcessor> tableProcessor, 
            ILogger? logger = null
        )
        {
            _connectionString = connectionString;
            _database = new Database(connectionString);
            _processor = tableProcessor;
            _logger = logger;
        }

        public void Dispose()
        {
            _database?.Dispose();
            _processor?.Dispose();

            GC.Collect(2, GCCollectionMode.Aggressive);
        }

        public abstract void DoWork();        
    }
}
