using GPU.Placeholders.Processing.Core.Data;
using GPU.Placeholders.Processing.Core.Providers;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace GPU.Placeholders.Processing.Core.FormulaProcessors
{
    public abstract class FormulaProcessor : IDisposable
    {
        public IDictionary<string, string>? LegendsForLookupCodes { get; set; }
        public IDictionary<string, LookupTableValue[]>? LookupTables { get; set; }


        protected FormulaProcessor()
        {

        }

        public virtual void Dispose()
        {
            LookupTables?.Clear();
            LegendsForLookupCodes?.Clear();
        }


        public virtual LookupTableValue[]? GetLookupTableValues(string placeholder, out string realPlaceholder)
        {
            realPlaceholder = PlaceholderAliasProvider.GetPlaceholderByAlias(placeholder) ?? "";
            var lookupcodeName = realPlaceholder;

            if (!LegendsForLookupCodes.TryGetValue(lookupcodeName, out string? lookuptableName))
                return null;

            if (string.IsNullOrEmpty(lookuptableName) || lookuptableName == ApplicationConstants.IGNORE_THIS_LOOKUP)
                return null;

            if (!LookupTables.TryGetValue(lookuptableName, out LookupTableValue[]? lookupvalues))
                return null;

            return lookupvalues;
        }        

        public virtual string GetPlaceholdersAndReplaceByTheirValues(string formula, DateTime rfiDate)
        {
            var codes = FormulaHelper.GetLookupCodes(formula);
            var lookupValues = codes.Select(c => new KeyValuePair<string, string>(c, GetPlaceholderValue(c, rfiDate))).ToImmutableDictionary();
            var replacedFormula = formula;

            foreach (var keyPair in lookupValues)
                replacedFormula = Regex.Replace(replacedFormula, $"{ApplicationConstants.PLACEHOLDER_TAG_IDENTIFIER}{keyPair.Key}(?![_])", keyPair.Value);

            return replacedFormula;
        }

        public virtual string GetPlaceholderValue(string placeholder, DateTime rfiDate)
        {
            var lookupvalues = GetLookupTableValues(placeholder, out string realPlaceholder);
            string placeholderValue = ApplicationConstants.PLACEHOLDER_DEFAULT_VALUE;

            if (lookupvalues is { Length: > 0 })
            {
                var formulaPlaceholderHandler = FormulaPlaceholderHandlerProvider.GetFormulaPlaceholderHandler(realPlaceholder, lookupvalues, this);
                placeholderValue = formulaPlaceholderHandler?.ProcessPlaceholder(realPlaceholder, rfiDate);
            }

            return placeholderValue ?? ApplicationConstants.PLACEHOLDER_DEFAULT_VALUE;
        }

        public abstract string? ProcessFormula(string pformula, DateTime rfiDate, params object[]? extraArgs);
    }
}
