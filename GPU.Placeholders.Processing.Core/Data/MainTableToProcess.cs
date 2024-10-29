namespace GPU.Placeholders.Processing.Core.Data
{
    public record MainTableToProcess
    {
        public string SiteID { get; set; } = "";
        public string CompanyCode { get; set; } = "";
        public string Type { get; set; } = "";
        public DateTime RFI_Date { get; set; }
        public DateTime? End_Date { get; set; }
        public DateTime? FomDate { get; set; }
        public string Formula_USD { get; set; }
        public string Formula { get; set; }
    }
}
