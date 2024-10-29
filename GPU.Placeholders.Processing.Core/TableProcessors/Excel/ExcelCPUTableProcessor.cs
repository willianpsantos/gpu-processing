using GPU.Placeholders.Processing.Core.Data;
using Microsoft.Extensions.Logging;

namespace GPU.Placeholders.Processing.Core.TableProcessors.Excel
{
    public class ExcelCPUTableProcessor(ILogger? logger = null) : ExcelTableProcessor(logger)
    {
        public override IEnumerable<MainTableProcessedResult> ProcessTable(IEnumerable<MainTableToProcess> data)
        {
            if (LegendsForLookupCodes is null or { Count: 0 })
                throw new InvalidOperationException("There's no legends loaded");

            if (LookupTables is null or { Count: 0 })
                throw new InvalidOperationException("There's no lookup tables loaded");

            var results = new HashSet<MainTableProcessedResult>();

            foreach (var item in data)
            {
                var result = new MainTableProcessedResult()
                {
                    SiteID = item.SiteID,
                    CompanyCode = item.CompanyCode,
                    RFI_Date = item.RFI_Date,
                    Formula = item.Formula,
                    Formula_USD = item.Formula_USD
                };

                LegendsForLookupCodes.Remove("RFI_DATE");
                LegendsForLookupCodes.Remove("RFIDATE");

                LegendsForLookupCodes.Add("RFI_DATE", "GENERAL_MEASURES");
                LegendsForLookupCodes.Add("RFIDATE", "GENERAL_MEASURES");

                if (LookupTables.TryGetValue("GENERAL_MEASURES", out LookupTableValue[]? values))
                {
                    var rfidate = item.RFI_Date.ToString("yyyy/MM/dd", System.Globalization.CultureInfo.InvariantCulture);

                    values.First(f => f.Code == "RFI_DATE").Value = "'" + rfidate + "'";
                    values.First(f => f.Code == "RFIDATE").Value = "'" + rfidate + "'";
                }

                if (!string.IsNullOrEmpty(item.Formula_USD) && item.Formula_USD != "0")
                    result.Formula_USD_Result = ProcessFormula(item.Formula_USD.Replace(" ", ""), item.RFI_Date);

                if (!string.IsNullOrEmpty(item.Formula) && item.Formula != "0")
                {
                    var formula = item.Formula;

                    if (FormulaHelper.FormulaHasPlaceholdersWhichRepresentFormulaUSDResult(formula))
                        formula = FormulaHelper.ReplaceFormulaUSDPlaceholdersByFormulaUSDResult(formula, result.Formula_USD_Result ?? ApplicationConstants.PLACEHOLDER_DEFAULT_VALUE);

                    result.FormulaResult = ProcessFormula(formula.Replace(" ", ""), item.RFI_Date);
                }

                results.Add(result);

                _logger?.LogInformation(result.ToString());
            }

            return results;
        }
    }
}
