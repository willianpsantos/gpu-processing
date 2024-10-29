using GPU.Placeholders.Processing.Core.Attributes;
using GPU.Placeholders.Processing.Core.Data;
using GPU.Placeholders.Processing.Core.FormulaProcessors;
using GPU.Placeholders.Processing.Core.Providers;

namespace GPU.Placeholders.Processing.Core.FormulaPlaceholderHandlers
{
    [FormulaPlaceholderHandlerFor(Name = "NGN_CPI")]
    public class NGN_CPI : FormulaPlaceholderHandler
    {
        public NGN_CPI(LookupTableValue[] lookuptableValues) : base(lookuptableValues)
        {
        }

        public NGN_CPI(LookupTableValue[] lookuptableValues, SoukokuFormulaProcessor formulaProcessor) : base(lookuptableValues, formulaProcessor)
        {

        }

        public override string ProcessPlaceholder(string placeholder, DateTime rfiDate)
        {
            var splited = placeholder.Split(ApplicationConstants.PLACEHOLDER_SEPARATOR);
            var function = PlaceholderAliasProvider.GetPlaceholderByAlias(splited[0]) ?? splited[0];
            var year = int.Parse(splited[1]);
            var cpi = double.Parse(splited[2]);

            var placeholderValue = FormulaHelper.GetPlaceholderValue(_lookuptableValues, function, rfiDate, year);
            double result = 1;

            if (double.TryParse(placeholderValue, out double placeholderNumber))
            {
                result = placeholderNumber - cpi;
            }

            return result.ToString();
        }
    }
}
