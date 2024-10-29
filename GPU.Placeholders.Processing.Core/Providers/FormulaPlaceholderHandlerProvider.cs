using GPU.Placeholders.Processing.Core.Attributes;
using GPU.Placeholders.Processing.Core.Data;
using GPU.Placeholders.Processing.Core.FormulaPlaceholderHandlers;
using GPU.Placeholders.Processing.Core.FormulaProcessors;
using System.Reflection;
using System.Text.RegularExpressions;

namespace GPU.Placeholders.Processing.Core.Providers
{
    public static class FormulaPlaceholderHandlerProvider
    {
        private static IDictionary<string, Type>? _LoadedFormulaPlaceholdersHandlersByNameTypes;
        private static IDictionary<string, Type>? _LoadedFormulaPlaceholdersHandlersByPatternTypes;

        private static readonly Type ArrayOfLookupTableValueType = typeof(LookupTableValue[]);
        private static readonly Type SoukokuFormulaProcessorType = typeof(SoukokuFormulaProcessor);

        
        public static bool MustReloadPlaceholderTypes() =>
            (
                _LoadedFormulaPlaceholdersHandlersByNameTypes is null or { Count: 0 } || 
                _LoadedFormulaPlaceholdersHandlersByPatternTypes is null or { Count: 0 }
            );

        public static void LoadFormulaPlaceholderHandlerTypes()
        {
            var assembly = typeof(FormulaPlaceholderHandlerProvider).Assembly;

            var types = 
                assembly
                    .GetTypes()
                    .Select(t => new AttributeAndClassType<FormulaPlaceholderHandlerForAttribute> 
                    { 
                        Attribute = t.GetCustomAttribute<FormulaPlaceholderHandlerForAttribute>(), 
                        ClassType = t 
                    })
                    .Where(t => t is { Attribute: not null, ClassType: not null });

            _LoadedFormulaPlaceholdersHandlersByNameTypes = 
                types?
                    .Where(a => !string.IsNullOrEmpty(a.Attribute!.Name))
                    .ToDictionary(a => a.Attribute!.Name!, a => a.ClassType!);

            _LoadedFormulaPlaceholdersHandlersByPatternTypes =
                types?
                    .Where(a => !string.IsNullOrEmpty(a.Attribute!.Pattern))
                    .ToDictionary(a => a.Attribute!.Pattern!, a => a.ClassType!);
        }

        public static Type? GetFormulaPlaceholderHandlerType(string name)
        {
            if (MustReloadPlaceholderTypes())
                LoadFormulaPlaceholderHandlerTypes();

            if (_LoadedFormulaPlaceholdersHandlersByNameTypes!.TryGetValue(name, out Type? type))
            {
                foreach (var key in _LoadedFormulaPlaceholdersHandlersByPatternTypes.Keys)
                {
                    if (Regex.IsMatch(name, key))
                        type = _LoadedFormulaPlaceholdersHandlersByPatternTypes[key];
                }
            }

            return type;
        }


        public static FormulaPlaceholderHandler? GetFormulaPlaceholderHandler(string name, LookupTableValue[] lookuptableValues, FormulaProcessor? formulaProcessor)
        {
            var type = GetFormulaPlaceholderHandlerType(name);
            object? instance = null;

            if (type is not null)
            {
                var constructor = type.GetConstructor([ArrayOfLookupTableValueType, formulaProcessor?.GetType() ?? SoukokuFormulaProcessorType]);
                instance = constructor?.Invoke([lookuptableValues, formulaProcessor]);
            }
            else
            {
                instance = new FormulaPlaceholderHandler(lookuptableValues, formulaProcessor);
            }

            return (FormulaPlaceholderHandler?)instance;
        }

        public static FormulaPlaceholderHandler? GetFormulaPlaceholderHandler(string name, LookupTableValue[] lookuptableValues) =>
            GetFormulaPlaceholderHandler(name, lookuptableValues, null);        
    }
}
