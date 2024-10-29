using GPU.Placeholders.Processing.Core.Attributes;
using GPU.Placeholders.Processing.Core.Data;
using GPU.Placeholders.Processing.Core.FormulaPlaceholderHandlers;
using GPU.Placeholders.Processing.Core.FormulaProcessors;
using GPU.Placeholders.Processing.Core.Providers;

namespace GPU.Placeholders.Processing.Core.FormulaPlaceholders
{
    [FormulaPlaceholderHandlerFor(Pattern = ".*\\:[0-9]{4}")]
    public class PlaceholderWithYearParameters : FormulaPlaceholderHandler
    {
        public PlaceholderWithYearParameters(LookupTableValue[] lookuptableValues) : base(lookuptableValues)
        {
        }

        public PlaceholderWithYearParameters(LookupTableValue[] lookuptableValues, SoukokuFormulaProcessor formulaProcessor) : base(lookuptableValues, formulaProcessor)
        {

        }

        public override string ProcessPlaceholder(string formula, DateTime rfiDate)
        {
            var splited = formula.Split(ApplicationConstants.PLACEHOLDER_SEPARATOR);
            var function = PlaceholderAliasProvider.GetPlaceholderByAlias(splited[1]) ?? splited[1];
            var value = GetPlaceholderValue(function, rfiDate);

            return value;
        }
    }
}
