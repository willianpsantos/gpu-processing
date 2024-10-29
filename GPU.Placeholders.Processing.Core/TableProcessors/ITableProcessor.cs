using GPU.Placeholders.Processing.Core.Data;

namespace GPU.Placeholders.Processing.Core.TableProcessors
{
    public interface ITableProcessor : IDisposable
    {
        IDictionary<string, string>? LegendsForLookupCodes { get; set; }
        IDictionary<string, LookupTableValue[]>? LookupTables { get; set; }

        public ITableProcessor AddLookupTableValues(string tablename, LookupTableValue[] values)
        {
            if (LookupTables is null)
            {
                LookupTables = new Dictionary<string, LookupTableValue[]>()
                {
                    {tablename, values}
                };
            }
            else if (LookupTables.TryGetValue(tablename, out LookupTableValue[]? value))
                LookupTables[tablename] = values;
            else
                LookupTables.Add(tablename, values);

            return this;
        }

        public ITableProcessor AddLookupTableValues(IDictionary<string, LookupTableValue[]> values)
        {
            LookupTables = values;
            return this;
        }

        public ITableProcessor AddLegendsForLookupCodes(IDictionary<string, string> legends)
        {
            LegendsForLookupCodes = legends;
            return this;
        }

        public IEnumerable<MainTableProcessedResult> ProcessTable(IEnumerable<MainTableToProcess> data);
    }
}
