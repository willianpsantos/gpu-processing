namespace GPU.Placeholders.Processing.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class FormulaEvaluatorCustomFunctionForAttribute : Attribute
    {
        public string? Name { get; private set; }
        public int ArgsCount {  get; private set; }

        public FormulaEvaluatorCustomFunctionForAttribute(string name, int argsCount)
        {
            Name = name;
            ArgsCount = argsCount;
        }
    }
}
