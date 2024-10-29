using GPU.Placeholders.Processing.Core.Data;
using GPU.Placeholders.Processing.Core.FormulaProcessors;

namespace GPU.Placeholders.Processing.Core.FormulaFunctionHandlers
{
    public class FormulaFunctionHandler
    {
        protected readonly SoukokuFormulaProcessor? _formulaProcessor;
        public IReadOnlyDictionary<string, string>? LegendsForLookupCodes { get; private set; }
        public IDictionary<string, LookupTableValue[]>? LookupTables { get; private set; }


        public FormulaFunctionHandler() { }

        public FormulaFunctionHandler(SoukokuFormulaProcessor? formulaProcessor)
        {
            _formulaProcessor = formulaProcessor;
        }


        public virtual FormulaFunctionHandler AddLookupTableValues(string tablename, LookupTableValue[] values)
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

        public virtual FormulaFunctionHandler AddLookupTableValues(IDictionary<string, LookupTableValue[]>? table)
        {
            LookupTables = table;
            return this;
        }

        public virtual FormulaFunctionHandler AddLegendsForLookupCodes(IDictionary<string, string>? legends)
        {
            LegendsForLookupCodes = legends?.AsReadOnly();
            return this;
        }

        public virtual FormulaFunctionHandler AddLegendsForLookupCodes(IReadOnlyDictionary<string, string>? legends)
        {
            LegendsForLookupCodes = legends;
            return this;
        }


        public virtual string? ProcessFunction(string formula, DateTime rfiDate, out string replacebleFormula)
        {
            replacebleFormula = "";
            return null;
        }
    }
}
