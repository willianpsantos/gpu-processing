using GPU.Placeholders.Processing.Core.Data;
using GPU.Placeholders.Processing.Core.FormulaProcessors;
using Microsoft.Extensions.Logging;

namespace GPU.Placeholders.Processing.Core.TableProcessors.Excel
{
    public abstract class ExcelTableProcessor : ExcelFormulaProcessor, ITableProcessor
    {
        protected readonly ILogger? _logger;

        protected ExcelTableProcessor(ILogger? logger = null) : base()
        {
            _logger = logger;
        }

        public virtual ExcelTableProcessor AddLookupTableValues(string tablename, LookupTableValue[] values)
        {
            if (LookupTables is null)
            {
                LookupTables = new Dictionary<string, LookupTableValue[]>()
                {
                    {tablename, values}
                };
            }
            else if (LookupTables.TryGetValue(tablename, out LookupTableValue[]? value))
                LookupTables[tablename] = values;
            else
                LookupTables.Add(tablename, values);

            return this;
        }

        public virtual ExcelTableProcessor AddLookupTableValues(IDictionary<string, LookupTableValue[]> values)
        {
            LookupTables = values;
            return this;
        }

        public virtual ExcelTableProcessor AddLegendsForLookupCodes(IDictionary<string, string> legends)
        {
            LegendsForLookupCodes = legends;
            return this;
        }

        public abstract IEnumerable<MainTableProcessedResult> ProcessTable(IEnumerable<MainTableToProcess> data);
    }
}
