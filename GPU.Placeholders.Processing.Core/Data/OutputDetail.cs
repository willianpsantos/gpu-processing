namespace GPU.Placeholders.Processing.Core.Data
{
    public class OutputDetail
    {
        public string COMPANYCODE { get; set; }
        public string BILL_DESC { get; set; }
        public string BILL_NO { get; set; }
        public string SITEID { get; set; }
        public string TYPE { get; set; }
        public string unit_AMT { get; set; }
        public string unit_AMT_NGN { get; set; }
        public float Qty { get; set; }
        public float AMT { get; set; }
        public float AMT_NGN { get; set; }
        public float AMT_USD { get; set; }
        public float AMT_USD_PORTION { get; set; }
        public float AMT_USD_PORTION_NGN { get; set; }
        public string CURRENCY { get; set; }
        public DateTime BILL_DATE { get; set; }
        public int? T_ID { get; set; }
        public string RECUSER { get; set; }
        public DateTime? recdate { get; set; }
        public DateTime? RFI_DATE { get; set; }
        public DateTime? IFRS_RFI_DATE { get; set; }
        public DateTime? IFRS_END { get; set; }
    }
}
