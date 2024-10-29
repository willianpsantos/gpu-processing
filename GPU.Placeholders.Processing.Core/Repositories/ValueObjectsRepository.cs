using GPU.Placeholders.Processing.Core.Data;

namespace GPU.Placeholders.Processing.Core.Repositories
{
    public class ValueObjectsRepository : Repository
    {
        public ValueObjectsRepository(string connectionString) : base(connectionString)
        {

        }

        public ValueObjectsRepository(Database database) : base(database)
        {

        }


        public IDictionary<string, LookupTableValue[]> LoadAllLookupTablesValues()
        {
            var additionalMeasures = _database.GetData<LookupTableValue>("SELECT MEASURE_TYPE as Code, MEASURE_VALUE as Value, APPLY_DATE as ApplyDate FROM ADDITIONAL_MEASURES ORDER BY APPLY_DATE DESC");
            var additionalTowerCoMeasures = _database.GetData<LookupTableValue>("SELECT MEASURE_TYPE as Code, MEASURE_VALUE as Value, APPLY_DATE as ApplyDate FROM ADDITIONAL_TOWERCO_MEASURES ORDER BY APPLY_DATE DESC");
            var generalMeasures = _database.GetData<LookupTableValue>("SELECT MEASURE_TYPE as Code, MEASURE_VALUE as Value, APPLY_DATE as ApplyDate FROM GENERAL_MEASURES ORDER BY APPLY_DATE DESC");
            var towerCoMeasures = _database.GetData<LookupTableValue>("SELECT MEASURE_TYPE as Code, MEASURE_VALUE as Value, APPLY_DATE as ApplyDate FROM TOWERCO_MEASURES ORDER BY APPLY_DATE DESC");
            var formattedToday = DateTime.Today.ToString("yyyy/MM/dd", System.Globalization.CultureInfo.InvariantCulture);

            var applyDateLookup = new LookupTableValue[]
            {
                new LookupTableValue
                {
                    Code = "APPLYDATE",
                    Value = formattedToday,
                    ApplyDate = DateTime.MinValue
                },

                new LookupTableValue
                {
                    Code = "APPLY_DATE",
                    Value = formattedToday,
                    ApplyDate = DateTime.MinValue
                },

                new LookupTableValue
                {
                    Code = "RFIDATE",
                    Value = formattedToday,
                    ApplyDate = DateTime.MinValue
                },

                new LookupTableValue
                {
                    Code = "RFI_DATE",
                    Value = formattedToday,
                    ApplyDate = DateTime.MinValue
                }
            };

            return new Dictionary<string, LookupTableValue[]>
            {
                { "ADDITIONAL_MEASURES", additionalMeasures.Concat(applyDateLookup).ToArray() },
                { "ADDITIONAL_TOWERCO_MEASURES", additionalTowerCoMeasures.Concat(applyDateLookup).ToArray() },
                { "GENERAL_MEASURES", generalMeasures.Concat(applyDateLookup).ToArray() },
                { "TOWERCO_MEASURES", towerCoMeasures.Concat(applyDateLookup).ToArray() }
            };
        }

        public IDictionary<string, string> LoadAllLegendForLookupCodes()
        {
            var values = _database.GetDataAsDictionary("SELECT CODE, ItemType FROM LEGEND_FOR_LOOKUP_CODES");

            values.Add("APPLYDATE", "GENERAL_MEASURES");
            values.Add("APPLY_DATE", "GENERAL_MEASURES");
            values.Add("RFIDATE", "GENERAL_MEASURES");

            return values;
        }

    }
}
