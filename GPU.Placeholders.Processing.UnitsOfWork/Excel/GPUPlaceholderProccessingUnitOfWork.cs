using GPU.Placeholders.Processing.Core;
using GPU.Placeholders.Processing.Core.Data;
using GPU.Placeholders.Processing.Core.Repositories;
using GPU.Placeholders.Processing.Core.TableProcessors;
using GPU.Placeholders.Processing.Core.TableProcessors.Excel;
using ILGPU;
using ILGPU.Runtime;
using ILGPU.Runtime.OpenCL;
using Microsoft.Extensions.Logging;

namespace GPU.Placeholders.Processing.UnitsOfWork.Excel
{
    public unsafe class GPUPlaceholderProccessingUnitOfWork : PlaceholderProcessingUnitOfWork<ExcelTableProcessor>
    {
        public GPUPlaceholderProccessingUnitOfWork(string connectionString, ILogger? logger = null)
            : base(
                  connectionString,
                  new TableProcessorProxy<ExcelTableProcessor>(new ExcelCPUTableProcessor(logger)),
                  logger
              )
        {

        }

        public static void ProcessTableKernel(
            Index1D index,
            ArrayView1D<char, Stride1D.Dense> rfidatebuffer,
            ArrayView1D<char, Stride1D.Dense> formulausdbuffer,
            ArrayView1D<char, Stride1D.Dense> formulabuffer
        )
        {
            
        }

        public override unsafe void DoWork()
        {
            using var repository = new MainTableToProcessRepository(_database);
            var tableCount = repository.GetMainProcessToProcessTotalCount();

            _database.CloseConnection();

            var take = 1000;
            var pages = tableCount > take ? tableCount / take : 1;

            using var context = Context.Create(builder => builder.Default().EnableAlgorithms());
            using var accelerator = context.CreateCLAccelerator(0);

            //Parallel.For(1, pages + 1, (page) =>
            for (var page = 1; page <= pages; page++)
            {
                using var database = new Database(HelpersData.ConnnectionString);
                using var localRepository = new MainTableToProcessRepository(_database);
                using var processor = new ExcelCPUTableProcessor(_logger);

                database.OpenConnection();
                var table = localRepository.GetMainTableToProcess(page, take).ToArray();
                database.CloseConnection();

                var kernel =
                    accelerator.LoadAutoGroupedStreamKernel<
                        Index1D,
                        ArrayView1D<char, Stride1D.Dense>,
                        ArrayView1D<char, Stride1D.Dense>,
                        ArrayView1D<char, Stride1D.Dense>
                    >(ProcessTableKernel);

                for (var i = 0; i < tableCount; i++)
                {
                    if (table?[i] is null)
                        continue;

                    if (!string.IsNullOrEmpty(table[i].Formula_USD) && !string.IsNullOrEmpty(table[i].Formula))
                        continue;

                    char[] rfiDateCharArray =
                        table[i]
                            .RFI_Date
                            .ToString("yyyy-MM-dd")
                            .ToCharArray();

                    char[] formulaUSDCharArray = table[i].Formula_USD.ToCharArray();
                    char[] formulaCharArray = table[i].Formula.ToCharArray();

                    var rfidateArrayBuffer = accelerator.Allocate1D<char>(rfiDateCharArray.LongLength);
                    var formulausdBuffer = accelerator.Allocate1D<char>(formulaUSDCharArray.LongLength);
                    var formulaBuffer = accelerator.Allocate1D<char>(formulaCharArray.LongLength);

                    rfidateArrayBuffer.CopyFromCPU(rfiDateCharArray);
                    formulausdBuffer.CopyFromCPU(formulaUSDCharArray);
                    formulaBuffer.CopyFromCPU(formulaCharArray);

                    kernel(new Index1D(i), rfidateArrayBuffer.View, formulausdBuffer.View, formulaBuffer.View);
                }
            }

            accelerator.Synchronize();
        }
    }
}
