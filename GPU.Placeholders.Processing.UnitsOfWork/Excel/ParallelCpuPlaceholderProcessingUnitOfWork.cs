using GPU.Placeholders.Processing.Core;
using GPU.Placeholders.Processing.Core.Data;
using GPU.Placeholders.Processing.Core.Repositories;
using GPU.Placeholders.Processing.Core.TableProcessors;
using GPU.Placeholders.Processing.Core.TableProcessors.Excel;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace GPU.Placeholders.Processing.UnitsOfWork.Excel
{
    public class ParallelCpuPlaceholderProcessingUnitOfWork(string connectionString, ILogger? logger = null) : 
        PlaceholderProcessingUnitOfWork<ExcelTableProcessor>(
            connectionString,
            new TableProcessorProxy<ExcelTableProcessor>(new ExcelParallelCPUTableProcessor(logger)),
            logger
        )
    {
        public override void DoWork()
        {
            var repository = new MainTableToProcessRepository(_database);
            var tableCount = repository.GetMainProcessToProcessTotalCount();

            _database.CloseConnection();

            repository.Dispose();

            var take = 1000;
            var pages = tableCount > take ? tableCount / take : 1;

            Parallel.For(1, pages + 1, (page) =>
            {
                using var database = new Database(HelpersData.ConnnectionString);
                using var localRepository = new MainTableToProcessRepository(_database);
                using var processor = new ExcelCPUTableProcessor(_logger);

                var loadedLegendsForLookupCodes = new ConcurrentDictionary<string, string>(HelpersData.LegendsForLookupCodes!);
                var loadedLookupTables = new ConcurrentDictionary<string, LookupTableValue[]>(HelpersData.LookupTables!);
                
                database.OpenConnection();

                var table = localRepository.GetMainTableToProcess(page, take);

                database.CloseConnection();

                var result =
                    processor
                        .AddLegendsForLookupCodes(loadedLegendsForLookupCodes)
                        .AddLookupTableValues(loadedLookupTables)
                        .ProcessTable(table);

                loadedLegendsForLookupCodes.Clear();
                loadedLookupTables.Clear();
            });
        }
    }
}
