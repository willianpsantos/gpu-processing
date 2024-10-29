using ILGPU;
using ILGPU.Runtime;
using ILGPU.Runtime.CPU;
using ILGPU.Runtime.Cuda;
using ILGPU.Runtime.OpenCL;
using Microsoft.Extensions.Logging;

namespace GPU.Placeholders.Processing.UnitsOfWork
{
    public class ListGPUDevices
    {
        private readonly ILogger _logger;

        public ListGPUDevices(ILogger logger)
        {
            _logger = logger;
        }

        private string GetInfoString(Accelerator a)
        {
            StringWriter infoString = new StringWriter();
            a.PrintInformation(infoString);
            return infoString.ToString();
        }

        public void ListDevices()
        {
            using var context = Context.Create(builder =>
            {
                builder.OpenCL().Cuda().CPU();
            });

            foreach (Device d in context)
            {
                using Accelerator accelerator = d.CreateAccelerator(context);                
                _logger.LogInformation(GetInfoString(accelerator));
            }

            foreach (CPUDevice d in context.GetCPUDevices())
            {
                using CPUAccelerator accelerator = (CPUAccelerator)d.CreateAccelerator(context);
                _logger.LogInformation(GetInfoString(accelerator));
            }

            foreach (Device d in context.GetCudaDevices())
            {
                using Accelerator accelerator = d.CreateAccelerator(context);
                _logger.LogInformation(GetInfoString(accelerator));
            }

            foreach (Device d in context.GetCLDevices())
            {
                using Accelerator accelerator = d.CreateAccelerator(context);
                _logger.LogInformation(GetInfoString(accelerator));
            }
        }
    }
}
