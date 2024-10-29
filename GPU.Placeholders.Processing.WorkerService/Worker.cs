using GPU.Placeholders.Processing.Core;
using GPU.Placeholders.Processing.Core.Data;
using GPU.Placeholders.Processing.Core.TableProcessors.Soukoku;
using GPU.Placeholders.Processing.UnitsOfWork;
using SoukokuUnitsOfWork = GPU.Placeholders.Processing.UnitsOfWork.Soukoku;
using ExcelUnitsOfWork = GPU.Placeholders.Processing.UnitsOfWork.Excel;
using System.Text.RegularExpressions;
using GPU.Placeholders.Processing.Core.TableProcessors.Excel;
using GPU.Placeholders.Processing.Core.Repositories;

namespace GPU.Placeholders.Processing.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private Regex _TheresOnlyPlaceholderInFormulaPiece = new Regex(@"^\({1}""{0,1}@{1}.*""{0,1}\){1}$");

        public Worker(
            ILogger<Worker> logger,
            IConfiguration configuration
        )
        {
            _logger = logger;
            _configuration = configuration;
        }

        private PlaceholderProcessingUnitOfWork<SoukokuTableProcessor>? GetProperUnitOfWork_Soukoku()
        {
            var connectioString = _configuration.GetConnectionString("Database") ?? "";
            var processingType = _configuration.GetValue<string>("ProcessingType");

            return processingType switch
            {
                "cpu" => new SoukokuUnitsOfWork.CpuPlaceholderProcessingUnitOfWork(connectioString, _logger),
                "parallel_cpu" => new SoukokuUnitsOfWork.ParallelCpuPlaceholderProcessingUnitOfWork(connectioString, _logger),
                "ilgpu" => null,
                _ => new SoukokuUnitsOfWork.CpuPlaceholderProcessingUnitOfWork(connectioString, _logger)
            };
        }

        private PlaceholderProcessingUnitOfWork<ExcelTableProcessor>? GetProperUnitOfWork_Excel()
        {
            var connectioString = _configuration.GetConnectionString("Database") ?? "";
            var processingType = _configuration.GetValue<string>("ProcessingType");

            return processingType switch
            {
                "cpu" => new ExcelUnitsOfWork.CpuPlaceholderProcessingUnitOfWork(connectioString, _logger),
                "parallel_cpu" => new ExcelUnitsOfWork.ParallelCpuPlaceholderProcessingUnitOfWork(connectioString, _logger),
                "ilgpu" => new ExcelUnitsOfWork.GPUPlaceholderProccessingUnitOfWork(connectioString, _logger),
                _ => new ExcelUnitsOfWork.CpuPlaceholderProcessingUnitOfWork(connectioString, _logger)
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //var listDevices = new ListGPUDevices(_logger);
            //listDevices.ListDevices();

            var connectioString = _configuration.GetConnectionString("Database") ?? "";
            var placeholderAliases = _configuration.GetSection(ApplicationConstants.PLACEHOLDER_ALIASES_SETTINGS_SECTION)?.Get<IDictionary<string, string>>();
            var formulaUSDPlaceholders = _configuration.GetSection(ApplicationConstants.FORMULA_USD_PALCEHOLDERS_SETTINGS_SECTION)?.Get<string[]>();

            using var voloader = new ValueObjectsRepository(connectioString);

            HelpersData.AliasesTable = placeholderAliases;
            HelpersData.Formula_USD_Placeholders = formulaUSDPlaceholders;
            HelpersData.LegendsForLookupCodes = voloader.LoadAllLegendForLookupCodes();
            HelpersData.LookupTables = voloader.LoadAllLookupTablesValues();
            HelpersData.ConnnectionString = connectioString;

            using var cpuUoW = GetProperUnitOfWork_Excel();

            cpuUoW?.DoWork();

            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    if (_logger.IsEnabled(LogLevel.Information))
            //    {
            //        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            //    }
            //    await Task.Delay(1000, stoppingToken);
            //}
        }
    }
}
