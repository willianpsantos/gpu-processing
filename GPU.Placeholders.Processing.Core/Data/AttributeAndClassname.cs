namespace GPU.Placeholders.Processing.Core.Data
{
    public record AttributeAndClassname<TAttribute> where TAttribute : Attribute
    {
        public TAttribute? Attribute { get; set; }
        public Type? ClassType { get; set; }
    }
}
