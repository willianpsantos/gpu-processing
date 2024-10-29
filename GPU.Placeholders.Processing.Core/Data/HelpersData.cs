namespace GPU.Placeholders.Processing.Core.Data
{
    public static class HelpersData
    {
        public static string ConnnectionString = "";
        public static string[]? Formula_USD_Placeholders;
        public static IDictionary<string, string>? AliasesTable;
        public static IDictionary<string, string>? LegendsForLookupCodes { get; set; }
        public static IDictionary<string, LookupTableValue[]>? LookupTables { get; set; }
    }
}
