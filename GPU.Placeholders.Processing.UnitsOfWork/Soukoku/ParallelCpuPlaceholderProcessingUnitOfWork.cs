using GPU.Placeholders.Processing.Core;
using GPU.Placeholders.Processing.Core.Builders;
using GPU.Placeholders.Processing.Core.Data;
using GPU.Placeholders.Processing.Core.Repositories;
using GPU.Placeholders.Processing.Core.TableProcessors;
using GPU.Placeholders.Processing.Core.TableProcessors.Soukoku;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace GPU.Placeholders.Processing.UnitsOfWork.Soukoku
{
    public class ParallelCpuPlaceholderProcessingUnitOfWork : PlaceholderProcessingUnitOfWork<SoukokuTableProcessor>
    {
        public ParallelCpuPlaceholderProcessingUnitOfWork(string connectionString, ILogger? logger = null)
            : base(
                  connectionString,
                  new TableProcessorProxy<SoukokuTableProcessor>(new SoukokuParallelCPUTableProcessor( EvaluatorContextBuilder.BuildContext(), logger)),
                  logger
              )
        {

        }

        public override void DoWork()
        {
            using var repository = new MainTableToProcessRepository(_database);
            var tableCount = repository.GetMainProcessToProcessTotalCount();

            _database.CloseConnection();

            var take = 100;
            var pages = tableCount > take ? tableCount / take : 1;

            Parallel.For(1, pages + 1, (page) =>
            {
                var context = EvaluatorContextBuilder.BuildContext();

                using var database = new Database(HelpersData.ConnnectionString);
                using var repository = new MainTableToProcessRepository(_database);                
                using var processor = new SoukokuCPUTableProcessor(context);

                var loadedLegendsForLookupCodes = new ConcurrentDictionary<string, string>(HelpersData.LegendsForLookupCodes!);
                var loadedLookupTables = new ConcurrentDictionary<string, LookupTableValue[]>(HelpersData.LookupTables!);
                var table = repository.GetMainTableToProcess(page, take);

                database.CloseConnection();

                var result =
                    processor
                        .AddLegendsForLookupCodes(loadedLegendsForLookupCodes)
                        .AddLookupTableValues(loadedLookupTables)
                        .ProcessTable(table);
            });
        }
    }
}
