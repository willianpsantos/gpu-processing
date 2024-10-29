using GPU.Placeholders.Processing.Core.Attributes;
using GPU.Placeholders.Processing.Core.Data;
using GPU.Placeholders.Processing.Core.FormulaPlaceholderHandlers;
using GPU.Placeholders.Processing.Core.FormulaProcessors;

namespace GPU.Placeholders.Processing.Core.FormulaPlaceholders
{
    [FormulaPlaceholderHandlerFor(Name = "@RFIDATE")]
    public class RFIDATE : FormulaPlaceholderHandler
    {
        public RFIDATE(LookupTableValue[] lookuptableValues) : base(lookuptableValues)
        {
        }

        public RFIDATE(LookupTableValue[] lookuptableValues, SoukokuFormulaProcessor formulaProcessor) : base(lookuptableValues, formulaProcessor)
        {

        }

        public override string ProcessPlaceholder(string placeholder, DateTime rfiDate) =>
            rfiDate.ToString("yyyy-MM-dd");        
    }
}
