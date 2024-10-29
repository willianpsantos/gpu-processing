using GPU.Placeholders.Processing.Core.Attributes;
using GPU.Placeholders.Processing.Core.Data;
using GPU.Placeholders.Processing.Core.FormulaPlaceholderHandlers;
using GPU.Placeholders.Processing.Core.FormulaProcessors;

namespace GPU.Placeholders.Processing.Core.FormulaPlaceholders
{
    [FormulaPlaceholderHandlerFor(Pattern = $"^(FXF\\{ApplicationConstants.PLACEHOLDER_SEPARATOR}).*")]
    public class FXF : FormulaPlaceholderHandler
    {
        public FXF(LookupTableValue[] lookuptableValues) : base(lookuptableValues)
        {
        }

        public FXF(LookupTableValue[] lookuptableValues, SoukokuFormulaProcessor formulaProcessor) : base(lookuptableValues, formulaProcessor)
        {

        }

        public override string ProcessPlaceholder(string formula, DateTime rfiDate) => 
            formula.Replace("FXF" + ApplicationConstants.PLACEHOLDER_SEPARATOR, "");
    }
}
