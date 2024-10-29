using GemBox.Spreadsheet;

namespace GPU.Placeholders.Processing.Core.FormulaProcessors
{
    public class ExcelFormulaProcessor : FormulaProcessor
    {
        private readonly ExcelWorksheet? _worksheet;

        public ExcelFormulaProcessor()
        {
            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
            var file = new ExcelFile();
            _worksheet = file.Worksheets.Add("Formula Evaluation");
        }

        public override void Dispose()
        {
            base.Dispose();
            _worksheet?.Clear();
        }

        public override string? ProcessFormula(string pformula, DateTime rfiDate, params object[]? extraArgs)
        {
            string result = ApplicationConstants.PLACEHOLDER_DEFAULT_VALUE;

            if (!string.IsNullOrEmpty(pformula))
            {
                string replacedFormula = pformula;

                try
                {
                    var formula = pformula;
                    replacedFormula = GetPlaceholdersAndReplaceByTheirValues(formula, rfiDate);

                    var expression = _worksheet?.CalculateFormula("=" + replacedFormula);
                    result = expression?[0, 0]?.ToString() ?? ApplicationConstants.PLACEHOLDER_DEFAULT_VALUE;
                }
                catch (GemBox.Spreadsheet.CalculationEngine.SpreadsheetParserException ex)
                {
                    result = replacedFormula;
                }
            }

            return result;
        }
    }
}
