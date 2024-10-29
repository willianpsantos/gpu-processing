using GPU.Placeholders.Processing.Core.FormulaFunctionHandlers;
using GPU.Placeholders.Processing.Core.FormulaProcessors;

namespace GPU.Placeholders.Processing.Core.Providers
{
    public static class FormulaFunctionHandlerProvider
    {
        public static FormulaFunctionHandler[] GetFormulaFunctionHandlers(string formula, SoukokuFormulaProcessor? formulaProcessor)
        {
            var handlers = new HashSet<FormulaFunctionHandler>();

            //if (FormulaHelper.StartsWithFunction_IF_TRIM_ApplyDate(formula))
            //    handlers.Add(new _IF_TRIM_APPLYDATE(formulaProcessor));

            //if (FormulaHelper.StartsWithFunction_IF_RFI_DATE(formula))
            //    handlers.Add(new _IF_RFI_DATE(formulaProcessor));

            if (FormulaHelper.StartsWithFunction_IF_General(formula))
                handlers.Add(new _IF_(formulaProcessor));

            return handlers.ToArray();
        }

        public static FormulaFunctionHandler[] GetFormulaFunctionHandlers(string formula) =>
            GetFormulaFunctionHandlers(formula, null);
    }
}
