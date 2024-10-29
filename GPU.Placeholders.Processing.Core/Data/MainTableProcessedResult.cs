namespace GPU.Placeholders.Processing.Core.Data
{
    public record MainTableProcessedResult
    {
        public string? SiteID;
        public string? CompanyCode;

        public DateTime RFI_Date { get; set; }
        
        public string? Formula_USD;
        public string? Formula;

        public string? FormulaResult;
        public string? Formula_USD_Result;

        public string FormatFormulaAndFormulaResult() =>
            $"\"{Formula}\" = {FormulaResult}";

        public string FormatFormulaAndFormulaResult_USD() =>
            $"\"{Formula_USD}\" = {Formula_USD_Result}";

        public override string ToString() =>
            $"\n * Site ID: {SiteID} | CompanyCode: {CompanyCode} \n " +
            $"\r\t a) Formula_USD: {FormatFormulaAndFormulaResult_USD()} \n " +
            $"\r\t b) Formula: {FormatFormulaAndFormulaResult()} \n ";
    }
}
