using GPU.Placeholders.Processing.Core.Data;
using GPU.Placeholders.Processing.Core.Providers;
using Soukoku.ExpressionParser;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace GPU.Placeholders.Processing.Core.FormulaProcessors
{
    public class SoukokuFormulaProcessor : FormulaProcessor
    {
        private readonly EvaluationContext _formulaEvaluationContext;
        private readonly Evaluator _evaluator;        

        protected SoukokuFormulaProcessor(EvaluationContext formulaEvaluationContext)
        {
            _formulaEvaluationContext = formulaEvaluationContext;
            _evaluator = new Evaluator(_formulaEvaluationContext);
        }

    
        private bool FormulaContainsSomeSpecialFunction(string formula) =>
            //FormulaHelper.StartsWithFunction_IF_RFI_DATE(formula) ||
            //FormulaHelper.StartsWithFunction_IF_TRIM_ApplyDate(formula) ||
            FormulaHelper.ContainsFunction_IF_General(formula);

        public virtual Evaluator GetUnderlyingFormulaEvaluator() => _evaluator;

        private string ProcessWithSpecialFormulas(string formula, DateTime rfiDate, Evaluator? evaluator = null)
        {
            var formulaPieces = FormulaHelper.GetFormulaPieces(formula);
            var alreadyProcessed = new Dictionary<string, string>();
            var lastResult = "";

            while (formulaPieces.Count > 0)
            {
                var replacebleFormula = "";
                var functionResult = "";
                var originalPiece = formulaPieces.Pop().Replace(" ", "");
                var sanitizedPiece = FormulaHelper.SanitizeFormula(originalPiece);
                var piece = FormulaHelper.ReplaceFormulaPiecesByResults(sanitizedPiece, alreadyProcessed);
                var replacedPiece = GetPlaceholdersAndReplaceByTheirValues(piece, rfiDate);
                var functionHandlers = FormulaFunctionHandlerProvider.GetFormulaFunctionHandlers(replacedPiece, this);

                if (functionHandlers is { Length: > 0 })
                {
                    foreach (var functionHandler in functionHandlers)
                    {
                        functionResult =
                            functionHandler?
                                .AddLegendsForLookupCodes(LegendsForLookupCodes)?
                                .AddLookupTableValues(LookupTables)?
                                .ProcessFunction(replacedPiece, rfiDate, out replacebleFormula) ?? "";
                    }
                }
                else
                {
                    functionResult = ProcessFormula(replacedPiece, rfiDate, evaluator) ?? "";
                }

                if (!alreadyProcessed.ContainsKey(originalPiece))
                    alreadyProcessed.Add(originalPiece, functionResult ?? ApplicationConstants.PLACEHOLDER_DEFAULT_VALUE);

                lastResult = functionResult;
            }

            alreadyProcessed.Clear();

            return lastResult ?? ApplicationConstants.PLACEHOLDER_DEFAULT_VALUE;
        }

        public override string? ProcessFormula(string pformula, DateTime rfiDate, params object[]? extraArgs)
        {
            if (string.IsNullOrEmpty(pformula))
                return "";

            var formula = pformula;
            var result = ApplicationConstants.PLACEHOLDER_DEFAULT_VALUE;
            var evaluator = extraArgs is { Length: > 0 } ? extraArgs[0] as Evaluator : null;

            if (FormulaContainsSomeSpecialFunction(formula))
                formula = ProcessWithSpecialFormulas(formula, rfiDate, evaluator);

            var replacedFormula = GetPlaceholdersAndReplaceByTheirValues(formula, rfiDate);

            var expression = (evaluator ?? _evaluator).Evaluate(replacedFormula);

            if (expression.TokenType == ExpressionTokenType.SingleQuoted || expression.TokenType == ExpressionTokenType.DoubleQuoted)
                result = "'" + expression.Value + "'";
            else
                result = expression.Value;

            // switch is not supported by GPU
            //result = expression.TokenType switch
            //{
            //    ExpressionTokenType.SingleQuoted or ExpressionTokenType.DoubleQuoted => "'" + expression.Value + "'",
            //    _ => expression.Value
            //};            

            return result;
        }
    }
}
