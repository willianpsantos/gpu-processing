using GPU.Placeholders.Processing.Core.Attributes;
using GPU.Placeholders.Processing.Core.Data;
using GPU.Placeholders.Processing.Core.FormulaPlaceholderHandlers;
using GPU.Placeholders.Processing.Core.FormulaProcessors;
using GPU.Placeholders.Processing.Core.Providers;

namespace GPU.Placeholders.Processing.Core.FormulaPlaceholders
{
    [FormulaPlaceholderHandlerFor(Pattern = $"^(RFI\\{ApplicationConstants.PLACEHOLDER_SEPARATOR}).*")]
    public class RFI : FormulaPlaceholderHandler
    {
        public RFI(LookupTableValue[] lookuptableValues) : base(lookuptableValues)
        {
        }

        public RFI(LookupTableValue[] lookuptableValues, SoukokuFormulaProcessor formulaProcessor) : base(lookuptableValues, formulaProcessor)
        {

        }

        public override string ProcessPlaceholder(string formula, DateTime rfiDate)
        {
            var removedRFIFormula = formula.Replace("RFI" + ApplicationConstants.PLACEHOLDER_SEPARATOR, "");
            var splited = formula.Split(ApplicationConstants.PLACEHOLDER_SEPARATOR);
            var function = PlaceholderAliasProvider.GetPlaceholderByAlias(splited[0]) ?? splited[0];
            var formulaPlaceholder = FormulaPlaceholderHandlerProvider.GetFormulaPlaceholderHandler(function, _lookuptableValues);
            var value = formulaPlaceholder!.ProcessPlaceholder(function, rfiDate);

            return value;
        }
    }
}
