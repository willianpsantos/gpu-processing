using GPU.Placeholders.Processing.Core.Data;
using Microsoft.Extensions.Logging;

namespace GPU.Placeholders.Processing.Core.TableProcessors
{
    public class TableProcessorProxy<TUnderlyingTableProcessor> : IDisposable where TUnderlyingTableProcessor : ITableProcessor
    {
        protected readonly TUnderlyingTableProcessor? _processor;

        public IDictionary<string, string>? LegendsForLookupCodes { get => _processor?.LegendsForLookupCodes; set => _processor!.LegendsForLookupCodes = value; }
        public IDictionary<string, LookupTableValue[]>? LookupTables { get => _processor?.LookupTables; set => _processor!.LookupTables = value; }

        public TableProcessorProxy(TUnderlyingTableProcessor processor, ILogger? logger = null)
        {
            _processor = processor;

            _processor?
                .AddLegendsForLookupCodes(HelpersData.LegendsForLookupCodes!)
                .AddLookupTableValues(HelpersData.LookupTables!);
        }

        public void Dispose()
        {
            _processor?.Dispose();
        }

        public IEnumerable<MainTableProcessedResult> ProcessTable(IEnumerable<MainTableToProcess> data) =>
            _processor?.ProcessTable(data) ?? [];
    }
}
