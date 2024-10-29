namespace GPU.Placeholders.Processing.Core
{
    public static class ApplicationConstants
    {
        public const string IGNORE_THIS_LOOKUP = "IGNORE_THIS_LOOKUP";
        public const string PLACEHOLDER_SEPARATOR = ":";
        public const string PLACEHOLDER_DEFAULT_VALUE = "1";
        public const string PLACEHOLDER_TAG_IDENTIFIER = "@";

        public const string PLACEHOLDER_ALIASES_SETTINGS_SECTION = "PlaceholderAliases";
        public const string FORMULA_USD_PALCEHOLDERS_SETTINGS_SECTION = "Formula_USD_Placeholders";

        //public const string CHECK_IF_RFI_DATE_PRESENT_REGEX = @"\s?(IF|if)\s?\(\s?(TRIM|trim)?\s?\(\s?("")?\s?(@RFI_DATE|@rfi_date|@Rfi_Date|@RFIDATE|@rfidate|@RfiDate)\s?""\s?\)";
        //public const string CHECK_IF_TRIM_APPLY_DATE_PRESENT_REGEX = @"\s?(IF|if)\s?\(\s?(TRIM|trim)?\s?\(\s?("")?\s?((@APPLYDATE|@applydate|@ApplyDate|@APPLY_DATE|@apply_date|@Apply_Date))\s?""\s?\)";
        public const string CHECK_IF_GENERAL_IS_PRESENT_REGEX = @"\s?(IF|if)\s?\(\s?";
        public const string CHECK_IF_STARTSWITH_GENERAL_IF_REGEX = @"^\s?\({0,1}(IF|if)\s?\({1}.*\){0,2}$";

        public const string REPLACE_TRIM_APPLYDATE_REGEX = @"\s?\(?\s?(trim|TRIM)+\({1}""\s?(@APPLYDATE|@applydate|@ApplyDate|@APPLY_DATE|@apply_date|@Apply_Date)\s?""\){1}";
        public const string REPLACE_TRIM_RFIDATE_REGEX = @"\s?\(?\s?(trim|TRIM)+\({1}""\s?(@RFI_DATE|@rfi_date|@Rfi_Date|@RFIDATE|@rfidate|@RfiDate)\s?""\){1}";
    }
}
