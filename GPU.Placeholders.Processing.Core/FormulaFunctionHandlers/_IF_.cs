using GPU.Placeholders.Processing.Core.Attributes;
using GPU.Placeholders.Processing.Core.Builders;
using GPU.Placeholders.Processing.Core.FormulaProcessors;
using Soukoku.ExpressionParser;
using System.Text.RegularExpressions;

namespace GPU.Placeholders.Processing.Core.FormulaFunctionHandlers
{
    [FormulaFunctionHandlerFor(Pattern = ApplicationConstants.CHECK_IF_STARTSWITH_GENERAL_IF_REGEX)]
    public class _IF_ : FormulaFunctionHandler
    {
        public _IF_() : base()
        {
        }

        public _IF_(SoukokuFormulaProcessor? formulaProcessor) : base(formulaProcessor)
        {

        }

        private string? ResolveFormula_V1(string formula, DateTime rfiDate, out string replacebleFormula)
        {
            var ifExpressionMatch = Regex.Match(formula, ApplicationConstants.CHECK_IF_STARTSWITH_GENERAL_IF_REGEX);
            string? result = "";

            if (ifExpressionMatch.Success)
            {
                replacebleFormula = ifExpressionMatch.Value;

                var split = ifExpressionMatch.Value.Split(",");
                var conditionExpression = Regex.Replace(split[0], @"\s?(IF|if)\(\s?(trim|TRIM)?", "").TrimStart().TrimEnd();
                var trueExpression = split[1];
                var falseExpression = Regex.Replace(string.Join(", ", split[2..]), @"\){1}$", "", RegexOptions.RightToLeft);
                var evaluator = _formulaProcessor?.GetUnderlyingFormulaEvaluator() ?? new Evaluator(EvaluatorContextBuilder.BuildContext());
                var value = _formulaProcessor?.ProcessFormula(conditionExpression, rfiDate);

                result =
                    value == "1"
                      ? _formulaProcessor?.ProcessFormula(trueExpression, rfiDate)
                      : _formulaProcessor?.ProcessFormula(falseExpression, rfiDate);
            }
            else
            {
                replacebleFormula = formula;
            }

            return result ?? ApplicationConstants.PLACEHOLDER_DEFAULT_VALUE;
        }


        public override string? ProcessFunction(
            string formula,
            DateTime rfiDate,
            out string replacebleFormula
        )
        {
            replacebleFormula = formula;

            var (condition, trueExpression, falseExpression) = FormulaHelper.Split_IF_Formula(formula);
            var evaluator = _formulaProcessor?.GetUnderlyingFormulaEvaluator() ?? new Evaluator(EvaluatorContextBuilder.BuildContext());
            var value = _formulaProcessor?.ProcessFormula(condition, rfiDate);

            var result =
                value == "1"
                      ? _formulaProcessor?.ProcessFormula(trueExpression, rfiDate)
                      : _formulaProcessor?.ProcessFormula(falseExpression, rfiDate);


            return result ?? ApplicationConstants.PLACEHOLDER_DEFAULT_VALUE;
        }
    }
}
