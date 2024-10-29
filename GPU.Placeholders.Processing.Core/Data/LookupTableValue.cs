namespace GPU.Placeholders.Processing.Core.Data
{
    public record LookupTableValue
    {
        public string Code { get; init; }
        public string Value { get; set; }
        public DateTime ApplyDate { get; init; }
    }
}
