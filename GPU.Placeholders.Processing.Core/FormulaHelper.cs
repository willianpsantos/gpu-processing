using GPU.Placeholders.Processing.Core.Data;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace GPU.Placeholders.Processing.Core
{
    public static class FormulaHelper
    {
        private static Regex _LookupCodeRegex = new Regex(@$"{ApplicationConstants.PLACEHOLDER_TAG_IDENTIFIER}\s?[^'""\*\-\+\/\(\)\>\<\=\s\[\]\{{\}},\.]+\s?");
        private static Regex _LookupCodeNoSpaceRegex = new Regex(@$"{ApplicationConstants.PLACEHOLDER_TAG_IDENTIFIER}[^'""\*\-\+\/\(\)\>\<\=\s\[\]\{{\}}]+");
        private static Regex _IsAPlaceholderRegex = new Regex(@$"^[A-Z_{ApplicationConstants.PLACEHOLDER_TAG_IDENTIFIER}]+$");
        private static Regex _TheresOtherCharsThanNumbers = new Regex(@"[^0-9]");
        private static Regex _IsAPlaceholderWithNumbersRegex = new Regex(@$"^[A-Z_0-9{ApplicationConstants.PLACEHOLDER_TAG_IDENTIFIER}]+$");
        private static Regex _IsAPlaceholderWithNumbersSpacesAndSpecialCharsRegex = new Regex(@$"^[A-Z_0-9\s\-_{ApplicationConstants.PLACEHOLDER_TAG_IDENTIFIER}]+$");

        //private static Regex _Check_IF_Trim_ApplyDate_FunctionRegex = new Regex(ApplicationConstants.CHECK_IF_TRIM_APPLY_DATE_PRESENT_REGEX);
        //private static Regex _CheckIF_RFI_Date_FunctionRegex = new Regex(ApplicationConstants.CHECK_IF_RFI_DATE_PRESENT_REGEX);
        private static Regex _Check_If_StartsWith_IF_General_FunctionRegex = new Regex(ApplicationConstants.CHECK_IF_STARTSWITH_GENERAL_IF_REGEX);
        private static Regex _Check_If_Contains_IF_General_FunctionRegex = new Regex(ApplicationConstants.CHECK_IF_GENERAL_IS_PRESENT_REGEX);

        private static Regex _CheckIfFormulaStartsAndEndsWithParenthesis = new Regex(@"^\s?\({1}.*?\){1}\s?$");
        private static Regex _TheresOnlyPlaceholderInFormulaPiece = new Regex(@"^\({1}""{0,1}@{1}.*""{0,1}\){1}$");
        private static Regex _TheresOnlyRawValuesInFormulaPiece = new Regex(@"^\s?\({1}\s?[0-9\-\+\.\\\/""']+\s?\){1}\s?$");


        //public static bool StartsWithFunction_IF_TRIM_ApplyDate(string formula) => 
        //    _Check_IF_Trim_ApplyDate_FunctionRegex.IsMatch(formula);

        //public static bool StartsWithFunction_IF_RFI_DATE(string formula) => 
        //    _CheckIF_RFI_Date_FunctionRegex.IsMatch(formula);

        public static bool ContainsFunction_IF_General(string formula) =>
            _Check_If_Contains_IF_General_FunctionRegex.IsMatch(formula);

        public static bool StartsWithFunction_IF_General(string formula) => 
            _Check_If_StartsWith_IF_General_FunctionRegex.IsMatch(formula);

        public static (string, string, string) Split_IF_Formula(string formula)
        {
            var sanitizedFormula = Regex.Replace(formula, @"(^[\s\,]+)|([\s\,]+$)", "");
            var split = sanitizedFormula.Split(',');

            var condition = split[0].TrimStart().TrimEnd();
            var @true = split[1].TrimStart().TrimEnd();
            var @false = split.Length > 3 ? string.Join(",", split[2..(split.Length)]).TrimStart().TrimEnd() : split[2];

            var conditionExpression = Regex.Replace(condition, @"\s?\s?(IF|if)\s?\(\s?(trim|TRIM)?", "").TrimStart().TrimEnd();
            var trueExpression = Regex.Replace(@true, @"(^[\(]{0,1}[\s\,]+)|([\s\,]+[\)]{0,1}$)", "");
            var falseExpression = Regex.Replace(@false, @"(^[\(]{0,1}[\s\,]+)|([\s\,]+[\)]{0,1}$)", "");

            conditionExpression = Regex.Replace(conditionExpression, @"(^[\(]{0,1}[\s\,]+)|([\s\,]+[\)]{0,1}$)", "");


            if (trueExpression.StartsWith("(") && !trueExpression.EndsWith(")"))
                trueExpression += ")";

            if (!trueExpression.StartsWith("(") && trueExpression.EndsWith(")"))
                trueExpression = "(" + trueExpression;

            
            if (falseExpression.StartsWith("(") && !falseExpression.EndsWith(")"))
                falseExpression += ")";

            if (!falseExpression.StartsWith("(") && falseExpression.EndsWith(")"))
                falseExpression = "(" + falseExpression;


            return (conditionExpression, trueExpression, falseExpression);
        }

        private static void SplitFormula(string formula, ref Stack<string> formulaPieces)
        {
            for (var i = 0; i < formula.Length; i++)
            {
                if (formula[i] == '(')
                {
                    var shouldContinue = true;
                    var parenthesisCloseNeededCount = 1;
                    var parenthesisCloseFoundCount = 0;
                    var j = i + 1;

                    do
                    {
                        if (j >= formula.Length)
                        {
                            shouldContinue = false;
                            parenthesisCloseFoundCount = parenthesisCloseNeededCount;

                            if (formula.EndsWith(")"))
                                SplitFormula(formula + ")", ref formulaPieces);
                        }
                        else
                        {
                            if (formula[j] == '(')
                                ++parenthesisCloseNeededCount;
                            else if (formula[j] == ')')
                                ++parenthesisCloseFoundCount;
                        }

                        j++;
                    }
                    while (parenthesisCloseNeededCount > parenthesisCloseFoundCount);

                    if (shouldContinue)
                    {
                        var @from = i;
                        var @to = j;

                        if (@from >= 2 && formula[(@from - 2)..@from].StartsWith("IF"))
                            @from -= 2;

                        var piece = formula[@from..@to];
                        var splited = piece.Split(",");

                        if (piece != formula && splited.Length > 3)
                        {
                            SplitFormula(piece, ref formulaPieces);
                        }
                        else
                        {
                            if (
                                !_TheresOnlyPlaceholderInFormulaPiece.IsMatch(piece) &&
                                !_TheresOnlyRawValuesInFormulaPiece.IsMatch(piece)
                            )
                            {
                                if (!formulaPieces.Contains(piece))
                                    formulaPieces.Push(piece);
                            }
                        }
                    }
                }
            }
        }

        public static Stack<string> GetFormulaPieces(string formula)
        {
            var formulaPieces = new Stack<string>();

            SplitFormula(formula, ref formulaPieces);

            var copyFormula = formula;
            var copyFormulaPieces = formulaPieces.Reverse().ToHashSet();

            foreach (var piece in copyFormulaPieces)
                copyFormula = ReplaceInFormula(copyFormula, piece, "");

            if (!string.IsNullOrEmpty(copyFormula) && copyFormula != formula)
            {
                //var sanitized = SanitizeFormula(copyFormula);
                //copyFormulaPieces.Add(sanitized);
                //copyFormulaPieces.Add(formula);

                formulaPieces.Clear();

                foreach (var item in copyFormulaPieces)
                    formulaPieces.Push(item);

                copyFormulaPieces.Clear();
            }

            return formulaPieces;
        }

        public static string ReplaceInFormula(string formula, string subject, string replacement)
        {
            var pattern =
                subject
                    .Replace("(", "\\(")
                    .Replace(")", "\\)")
                    .Replace(".", "\\.")
                    .Replace(",", "\\,")
                    .Replace(">", "\\>")
                    .Replace("<", "\\<")
                    .Replace("*", "\\*")
                    .Replace("-", "\\-")
                    .Replace("\\", "\\")
                    .Replace("/", "\\/")
                    .Replace("+", "\\+");

            var replacedFormula =
                Regex.Replace(
                    formula,
                    pattern,
                    replacement,
                    RegexOptions.ECMAScript
                );

            return replacedFormula;
        }

        public static bool IsPlaceholderName(string value)
        {
            var itis = true;

            if (!_TheresOtherCharsThanNumbers.IsMatch(value))
                itis = false;

            if (_IsAPlaceholderRegex.IsMatch(value))
                itis = false;

            if (_IsAPlaceholderWithNumbersRegex.IsMatch(value))
                itis = true;

            return itis && _IsAPlaceholderWithNumbersSpacesAndSpecialCharsRegex.IsMatch(value);
        }

        public static string[] GetLookupCodes(string formula)
        {
            var matches = _LookupCodeRegex.Matches(formula);
            var codes = new HashSet<string>();

            foreach (Match match in matches)
            {
                if (match.Success)
                    codes.Add(match.Value.Trim().Replace(ApplicationConstants.PLACEHOLDER_TAG_IDENTIFIER, ""));
            }

            return codes.ToArray();
        }

        public static string GetFirstPlaceholderOccurence(string placeholder)
        {
            var split = placeholder.Split(ApplicationConstants.PLACEHOLDER_SEPARATOR);
            var first = split.FirstOrDefault(s => IsPlaceholderName(s));

            if (string.IsNullOrEmpty(first))
                first = placeholder;

            return first;
        }

        public static bool IsLookupCode(string code) => _LookupCodeNoSpaceRegex.IsMatch(code);

        public static bool FormulaHasPlaceholdersWhichRepresentFormulaUSDResult(string formula) =>
            HelpersData.Formula_USD_Placeholders?.Any(f => formula.Contains($"{ApplicationConstants.PLACEHOLDER_TAG_IDENTIFIER}{f}")) ?? false;

        public static string ReplaceFormulaUSDPlaceholdersByFormulaUSDResult(string formula, string result)
        {
            var newFormula = formula;

            if (HelpersData.Formula_USD_Placeholders is { Length: > 0 })
            {
                foreach (var placeholder in HelpersData.Formula_USD_Placeholders)
                    newFormula = newFormula.Replace($"{ApplicationConstants.PLACEHOLDER_TAG_IDENTIFIER}{placeholder}", result);
            }

            return newFormula;
        }
        
        public static bool HasPlaceholderSeparator(string value) => 
            value.Contains(ApplicationConstants.PLACEHOLDER_SEPARATOR);        

        public static string ReplaceFormulaPiecesByResults(string formula, IDictionary<string, string>? results)
        {
            var replacedFormula = formula;

            if (results is { Count: > 0 })
            {
                foreach (var key in results.Keys.Reverse())
                    replacedFormula = ReplaceInFormula(replacedFormula, key, results[key]);
            }

            return replacedFormula;
        }

        public static string SanitizeFormula(string formula)
        {
            var sanitizedFormula = Regex.Replace(formula, "([\\r\\n\\t])|([\\*\\.\\,\\+\\-\\/\\\\\\(\\{]+$)", "");

            if (_CheckIfFormulaStartsAndEndsWithParenthesis.IsMatch(formula))
                sanitizedFormula = Regex.Replace(formula, "(^\\s?\\({1})|(\\){1}\\s?$)|([\\r\\n\\t])", "");

            return sanitizedFormula;
        }

        public static string TreatNotSupportException(NotSupportedException ex, string formula, out bool shouldReprocess)
        {
            shouldReprocess = false;

            if (ex.Message.StartsWith("Function") && ex.Message.EndsWith("is not supported"))
            {
                shouldReprocess = true;

                var remaining =
                    ex.Message
                        .Replace("Function", "")
                        .Replace("is not supported.", "")
                        .Replace("\"", "")
                        .Replace(" ", "");

                formula = formula.Replace(remaining, "(" + remaining + ")");
            }

            return formula;
        }

        public static string GetPlaceholderValue(
            LookupTableValue[]? lookupvalues, 
            string placeholder, 
            DateTime rfiDate,
            int year = 0
        )
        {           
            var placeholderValue = ApplicationConstants.PLACEHOLDER_DEFAULT_VALUE;

            if (lookupvalues is { Length: > 0 })
            {
                placeholderValue =
                    lookupvalues?
                        .OrderByDescending(_ => _.ApplyDate)
                        .FirstOrDefault(
                            _ => _.Code == placeholder &&
                                 _.ApplyDate <= rfiDate &&
                                 (year == 0 || _.ApplyDate.Year == year)
                         )?
                        .Value;

                if (!string.IsNullOrEmpty(placeholderValue) && IsLookupCode(placeholderValue))
                    placeholderValue = GetPlaceholderValue(lookupvalues, placeholderValue, rfiDate);
            }

            return placeholderValue ?? ApplicationConstants.PLACEHOLDER_DEFAULT_VALUE;
        }
    }
}
