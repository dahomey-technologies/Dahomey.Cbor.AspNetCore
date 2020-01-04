using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System.IO.Pipelines;
using System.Threading.Tasks;

namespace Dahomey.Cbor.AspNetCore
{
    public class CborOutputFormatter : OutputFormatter
    {
        private readonly CborOptions _cborOptions;

        public CborOutputFormatter(CborOptions cborOptions)
        {
            _cborOptions = cborOptions ?? CborOptions.Default;

            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/cbor"));
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
#if (NETCOREAPP2_1 || NETCOREAPP2_2)
            return Cbor.SerializeAsync(context.Object, context.ObjectType, context.HttpContext.Response.Body, _cborOptions);
#else
            PipeWriter writer = context.HttpContext.Response.BodyWriter;
            Cbor.Serialize(context.Object, context.ObjectType, writer, _cborOptions);

            ValueTask<FlushResult> flushTask = writer.FlushAsync();

            if (flushTask.IsCompleted)
            {
                ValueTask completeTask = writer.CompleteAsync();

                if (completeTask.IsCompleted)
                {
                    return Task.CompletedTask;
                }

                return FinishCompleteAsync(completeTask);
            }

            return FinishFlushAsync(flushTask);

            async Task FinishFlushAsync(ValueTask<FlushResult> localFlushTask)
            {
                await localFlushTask.ConfigureAwait(false);
                await writer.CompleteAsync();
            }

            async Task FinishCompleteAsync(ValueTask localCompleteTask)
            {
                await localCompleteTask.ConfigureAwait(false);
            }
#endif
        }
    }
}
