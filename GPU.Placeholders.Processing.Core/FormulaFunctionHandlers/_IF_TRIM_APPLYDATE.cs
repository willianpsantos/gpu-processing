using GPU.Placeholders.Processing.Core.Builders;
using GPU.Placeholders.Processing.Core.FormulaProcessors;
using Soukoku.ExpressionParser;
using System.Text.RegularExpressions;

namespace GPU.Placeholders.Processing.Core.FormulaFunctionHandlers
{
    //[FormulaFunctionHandlerFor(Pattern = ApplicationConstants.CHECK_IF_TRIM_APPLY_DATE_PRESENT_REGEX)]
    public class _IF_TRIM_APPLYDATE : FormulaFunctionHandler
    {
        public _IF_TRIM_APPLYDATE() : base()
        {
        }

        public _IF_TRIM_APPLYDATE(SoukokuFormulaProcessor? formulaProcessor) : base(formulaProcessor)
        {

        }

        private string? ResolveFormula_V1(string formula, DateTime rfiDate, out string replacebleFormula)
        {
            var ifExpressionMatch = Regex.Match(formula, @"(IF\s?\(.*\,.*\,.*\))");
            string? result = "";

            if (ifExpressionMatch.Success)
            {
                replacebleFormula = ifExpressionMatch.Value;

                var split = ifExpressionMatch.Value.Split(",");
                var conditionExpression = Regex.Replace(split[0], @"\s?(IF)\(\s?(trim|TRIM)", "").TrimStart().TrimEnd();

                conditionExpression = Regex.Replace(conditionExpression, @"\s?\(?""\s?@APPLYDATE\s?""\)?", "'" + DateTime.Now.ToString("yyyy-MM-dd") + "'").Replace("\"", "'");

                var trueExpression = split[1];
                var falseExpression = Regex.Replace(string.Join(", ", split[2..]), @"\){1}$", "", RegexOptions.RightToLeft);

                var evaluator = _formulaProcessor?.GetUnderlyingFormulaEvaluator() ?? new Evaluator(EvaluatorContextBuilder.BuildContext());
                var value = evaluator.Evaluate(conditionExpression, true).Value ?? "0";

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
            var conditionSanitized = Regex.Replace(condition, ApplicationConstants.REPLACE_TRIM_APPLYDATE_REGEX, "'@APPLYDATE'").Replace("\"", "'").Replace("-", "/");
            var value = _formulaProcessor?.ProcessFormula(conditionSanitized, rfiDate);

            var result =
                value == "1"
                      ? _formulaProcessor?.ProcessFormula(trueExpression, rfiDate)
                      : _formulaProcessor?.ProcessFormula(falseExpression, rfiDate);

            return result ?? ApplicationConstants.PLACEHOLDER_DEFAULT_VALUE;
        }
    }
}
