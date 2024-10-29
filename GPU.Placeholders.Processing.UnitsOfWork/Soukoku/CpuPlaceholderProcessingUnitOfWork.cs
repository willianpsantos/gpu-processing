using GPU.Placeholders.Processing.Core.Builders;
using GPU.Placeholders.Processing.Core.Repositories;
using GPU.Placeholders.Processing.Core.TableProcessors;
using GPU.Placeholders.Processing.Core.TableProcessors.Soukoku;
using Microsoft.Extensions.Logging;

namespace GPU.Placeholders.Processing.UnitsOfWork.Soukoku
{
    public class CpuPlaceholderProcessingUnitOfWork : PlaceholderProcessingUnitOfWork<SoukokuTableProcessor>
    {
        public CpuPlaceholderProcessingUnitOfWork(string connectionString, ILogger? logger = null)
            : base(
                  connectionString,
                  new TableProcessorProxy<SoukokuTableProcessor>(new SoukokuCPUTableProcessor(EvaluatorContextBuilder.BuildContext(), logger)),
                  logger
              )
        {

        }

        public override void DoWork()
        {
            var repository = new MainTableToProcessRepository(_database);
            var tableCount = repository.GetMainProcessToProcessTotalCount();

            ushort currentTake = 100;
            var pages = tableCount > currentTake ? tableCount / currentTake : 1;

            for (var page = 1; page <= pages; page++)
            {
                var table = repository.GetMainTableToProcess(page, currentTake);
                var result = _processor.ProcessTable(table);
            }

            _database.CloseConnection();
        }
    }
}
