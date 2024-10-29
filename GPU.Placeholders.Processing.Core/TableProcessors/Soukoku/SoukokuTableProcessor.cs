using GPU.Placeholders.Processing.Core.Data;
using GPU.Placeholders.Processing.Core.FormulaProcessors;
using Microsoft.Extensions.Logging;
using Soukoku.ExpressionParser;

namespace GPU.Placeholders.Processing.Core.TableProcessors.Soukoku
{
    public abstract class SoukokuTableProcessor : SoukokuFormulaProcessor, ITableProcessor
    {
        protected readonly ILogger? _logger;

        protected SoukokuTableProcessor(
            EvaluationContext formulaEvaluationContext,
            ILogger? logger = null
        )
        : base(formulaEvaluationContext)
        {
            _logger = logger;
        }

        public virtual SoukokuTableProcessor AddLookupTableValues(string tablename, LookupTableValue[] values)
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

        public virtual SoukokuTableProcessor AddLookupTableValues(IDictionary<string, LookupTableValue[]> values)
        {
            LookupTables = values;
            return this;
        }

        public virtual SoukokuTableProcessor AddLegendsForLookupCodes(IDictionary<string, string> legends)
        {
            LegendsForLookupCodes = legends;
            return this;
        }

        public abstract IEnumerable<MainTableProcessedResult> ProcessTable(IEnumerable<MainTableToProcess> data);
    }
}
