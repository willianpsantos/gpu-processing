using GPU.Placeholders.Processing.Core.Extensions;
using Soukoku.ExpressionParser;

namespace GPU.Placeholders.Processing.Core.Builders
{
    public static class EvaluatorContextBuilder
    {
        public static EvaluationContext BuildContext()
        {
            var context = new EvaluationContext();

            context.RegisterCustomFunction_DAY();
            context.RegisterCustomFunction_MONTH();
            context.RegisterCustomFunction_YEAR();
            context.RegisterCustomFunction_IF();

            return context;
        }
    }
}
