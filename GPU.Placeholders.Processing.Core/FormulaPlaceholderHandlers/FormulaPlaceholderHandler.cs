using GPU.Placeholders.Processing.Core.Data;
using GPU.Placeholders.Processing.Core.FormulaProcessors;

namespace GPU.Placeholders.Processing.Core.FormulaPlaceholderHandlers
{
    public class FormulaPlaceholderHandler
    {
        protected readonly LookupTableValue[] _lookuptableValues;
        protected readonly FormulaProcessor? _formulaProcessor;

        public FormulaPlaceholderHandler(LookupTableValue[] lookuptableValues) =>
            _lookuptableValues = lookuptableValues;

        public FormulaPlaceholderHandler(LookupTableValue[] lookuptableValues, FormulaProcessor? formulaProcessor)
        {
            _lookuptableValues = lookuptableValues;
            _formulaProcessor = formulaProcessor;
        }

        public virtual string GetPlaceholderValue(string placeholder, DateTime rfiDate) => 
            FormulaHelper.GetPlaceholderValue(_lookuptableValues, placeholder, rfiDate) ?? ApplicationConstants.PLACEHOLDER_DEFAULT_VALUE;

        public virtual string ProcessPlaceholder(string formula, DateTime rfiDate) =>
            GetPlaceholderValue(formula, rfiDate);
    }
}
