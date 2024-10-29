using Soukoku.ExpressionParser;

namespace GPU.Placeholders.Processing.Core.Extensions
{
    public static class EvaluatorContextExtensions
    {
        public static void RegisterCustomFunction_YEAR(this EvaluationContext context)
        {
            var routine = new FunctionRoutine(1, (localContext, args) =>
            {
                var arg = args[0].Value;
                ExpressionToken? token = null;

                // if - return is not supported by GPU
                //if (!DateTime.TryParse(arg, out DateTime date))
                //    return new ExpressionToken(DateTime.Today.Year.ToString());

                //return new ExpressionToken(date.Year.ToString());

                if (DateTime.TryParse(arg, out DateTime date))
                    token = new ExpressionToken(date.Year.ToString());
                else
                    token = new ExpressionToken(DateTime.Today.Year.ToString());

                return token;
            });
            
            context.RegisterFunction("YEAR", routine);
            context.RegisterFunction("year", routine);
            context.RegisterFunction("Year", routine);
            context.RegisterFunction("Yaer", routine);
        }

        public static void RegisterCustomFunction_MONTH(this EvaluationContext context)
        {
            var routine = new FunctionRoutine(1, (localContext, args) =>
            {
                var arg = args[0].Value;
                ExpressionToken? token = null;

                // if - return is not supported by GPU
                //if (!DateTime.TryParse(arg, out DateTime date))
                //    return new ExpressionToken(DateTime.Today.Month.ToString());

                //return new ExpressionToken(date.Month.ToString());

                if (DateTime.TryParse(arg, out DateTime date))
                    token = new ExpressionToken(date.Month.ToString());
                else
                    token = new ExpressionToken(DateTime.Today.Month.ToString());

                return token;
            });

            context.RegisterFunction("MONTH", routine);
            context.RegisterFunction("month", routine);
            context.RegisterFunction("Month", routine);
        }

        public static void RegisterCustomFunction_DAY(this EvaluationContext context)
        {
            var routine = new FunctionRoutine(1, (localContext, args) =>
            {
                var arg = args[0].Value;
                ExpressionToken? token = null;

                // if - return is not supported by GPU
                //if (!DateTime.TryParse(arg, out DateTime date))
                //    return new ExpressionToken(DateTime.Today.Day.ToString());

                //return new ExpressionToken(date.Day.ToString());

                if (DateTime.TryParse(arg, out DateTime date))
                    token = new ExpressionToken(date.Day.ToString());
                else
                    token = new ExpressionToken(DateTime.Today.Day.ToString());

                return token;
            });

            context.RegisterFunction("DAY", routine);
            context.RegisterFunction("day", routine);
            context.RegisterFunction("Day", routine);
        }

        public static void RegisterCustomFunction_TRIM(this EvaluationContext context)
        {
            var routine = new FunctionRoutine(1, (localContext, args) =>
            {
                var arg = args[0]?.Value?.TrimStart()?.TrimEnd() ?? "";
                return new ExpressionToken(arg);
            });

            context.RegisterFunction("TRIM", routine);
            context.RegisterFunction("Trim", routine);
            context.RegisterFunction("trim", routine);
        }

        public static void RegisterCustomFunction_IF(this EvaluationContext context)
        {
            var routine = new FunctionRoutine(3, (localContext, args) =>
            {
                var arg = args[0]?.Value?.TrimStart()?.TrimEnd() ?? "";
                return new ExpressionToken(arg);
            });

            context.RegisterFunction("IF", routine);
            context.RegisterFunction("if", routine);
            context.RegisterFunction("If", routine);
        }
    }
}
