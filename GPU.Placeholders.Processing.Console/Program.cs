using GPU.Placeholders.Processing.Core;
using GPU.Placeholders.Processing.Core.Repositories;
using ILGPU;
using ILGPU.Runtime;
using System.Text;

class Program
{
    static void ProcessTableKernel(
        Index1D index,
        ArrayView<byte> rfidatebuffer,
        ArrayView<byte> formulausdbuffer,
        ArrayView<byte> formulabuffer
    )
    {
        String rfidate = Encoding.UTF8.GetString(rfidatebuffer.GetAsArray());
        String formulausd = Encoding.UTF8.GetString(formulausdbuffer.GetAsArray());
        String formula = Encoding.UTF8.GetString(formulabuffer.GetAsArray());
        String message = "RFI DATE: " + rfidate + " | Formula USD: " + formulausd + " | Formula: " + formula;

        Interop.WriteLine(message);
    }

    static void Main(string[] args)
    {
        var take = 1000;
        var connectionString = "Server=127.0.0.1\\mcr.microsoft.com/mssql/server,1433; Database=ILGPU; User ID=sa; Password=@P455w0rd$; Integrated Security=False; MultipleActiveResultSets=True; TrustServerCertificate=True; Min Pool Size=5; Max Pool Size=1000";

        using var database = new Database(connectionString);
        using var repository = new MainTableToProcessRepository(database);

        var tableCount = repository.GetMainProcessToProcessTotalCount();
        var pages = tableCount > take ? tableCount / take : 1;

        using var context = Context.CreateDefault();
        Accelerator? accelerator = null;

        foreach (var devide in context.Devices)
        {
            if (devide.AcceleratorType == AcceleratorType.CPU)
                accelerator = devide.CreateAccelerator(context);
        }

        for (var page = 1; page <= pages; page++)
        {
            using var localRepository = new MainTableToProcessRepository(database);
            var table = localRepository.GetMainTableToProcess(page, take).ToArray();

            var kernel =
                accelerator.LoadAutoGroupedStreamKernel<
                    Index1D,
                    ArrayView<byte>,
                    ArrayView<byte>,
                    ArrayView<byte>
                >(ProcessTableKernel);

            for (var i = 0; i < take; i++)
            {
                if (table?[i] is null)
                    continue;

                if (string.IsNullOrEmpty(table[i].Formula_USD) && string.IsNullOrEmpty(table[i].Formula))
                    continue;

                var rfidate = table[i].RFI_Date.ToString("yyyy-MM-dd");
                var formulausd = table[i].Formula_USD;
                var formula = table[i].Formula;

                byte[] rfiDateByteArray = Encoding.UTF8.GetBytes(rfidate);
                byte[] formulaUSDByteArray = Encoding.UTF8.GetBytes(formulausd);
                byte[] formulaByteArray = Encoding.UTF8.GetBytes(formula);

                using var rfidateArrayBuffer = accelerator.Allocate1D(rfiDateByteArray);
                using var formulausdBuffer = accelerator.Allocate1D(formulaUSDByteArray);
                using var formulaBuffer = accelerator.Allocate1D(formulaByteArray);

                //rfidateArrayBuffer.CopyFromCPU(rfiDateCharArray);
                //formulausdBuffer.CopyFromCPU(formulaUSDCharArray);
                //formulaBuffer.CopyFromCPU(formulaCharArray);

                kernel(
                    new Index1D(i),
                    rfidateArrayBuffer.View,
                    formulausdBuffer.View,
                    formulaBuffer.View
                );
            }
        }

        accelerator?.Dispose();
    }
}