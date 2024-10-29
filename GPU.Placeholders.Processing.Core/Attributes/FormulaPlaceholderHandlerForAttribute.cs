﻿namespace GPU.Placeholders.Processing.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class FormulaPlaceholderHandlerForAttribute : Attribute
    {
        public string? Name { get; set; }
        public string? Pattern { get; set; }

        public FormulaPlaceholderHandlerForAttribute()
        {
            
        }
    }
}
