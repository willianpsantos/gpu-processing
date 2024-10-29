using GPU.Placeholders.Processing.Core.Data;
using System.Text;

namespace GPU.Placeholders.Processing.Core.Providers
{
    public static class PlaceholderAliasProvider
    {
        // switch is not supported by GPU
        //private static string? ProcessAlias(string? alias, string? realPlaceholder) =>
        //    realPlaceholder switch 
        //    { 
        //        "RemoveAlias" => "",
        //        _ => realPlaceholder
        //    };

        private static string? ProcessAlias(string? alias, string? realPlaceholder)
        {
            string? result;

            if(realPlaceholder == "RemoveAlias")
                result = alias;
            else
                result = realPlaceholder;

            return result;
        }

        private static string? ProcessChainedAliases(string alias)
        {
            var parts = alias.Split(ApplicationConstants.PLACEHOLDER_SEPARATOR);
            var newAlias = alias;
            var builder = new StringBuilder();

            foreach (var part in parts)
            {
                if (FormulaHelper.IsPlaceholderName(part))
                {
                    var aux = GetPlaceholderByAlias(part);
                    newAlias = newAlias.Replace(string.IsNullOrEmpty(aux) ? part + ApplicationConstants.PLACEHOLDER_SEPARATOR : part, aux);
                }
            }

            return newAlias;
        }


        public static string? GetPlaceholderByAlias(string alias)
        {
            string? result = null;

            if (alias.Contains(ApplicationConstants.PLACEHOLDER_SEPARATOR))
                result = ProcessChainedAliases(alias);
            else
            {
                HelpersData.AliasesTable?.TryGetValue(alias, out result);
                result = ProcessAlias(alias, result);
            }

            return result ?? alias;
        }
    }
}
