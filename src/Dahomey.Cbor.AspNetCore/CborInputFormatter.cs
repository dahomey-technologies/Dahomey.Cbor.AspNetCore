using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Threading.Tasks;

namespace Dahomey.Cbor.AspNetCore
{
    public class CborInputFormatter : InputFormatter
    {
        private readonly static Task<InputFormatterResult> failureTask = Task.FromResult(InputFormatterResult.Failure());
        private readonly static Task<InputFormatterResult> noValueTask = Task.FromResult(InputFormatterResult.NoValue());

        private readonly CborOptions _cborOptions;

        public CborInputFormatter(CborOptions cborOptions)
        {
            _cborOptions = cborOptions ?? CborOptions.Default;

            SupportedMediaTypes.Add("application/cbor");
        }

        public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            try
            {
#if (NETCOREAPP2_1 || NETCOREAPP2_2)
                ValueTask<object> task = Cbor.DeserializeAsync(context.ModelType, context.HttpContext.Request.Body, _cborOptions);
#else
                ValueTask<object> task = Cbor.DeserializeAsync(context.ModelType, context.HttpContext.Request.BodyReader, _cborOptions);
#endif
                if (task.IsCompleted)
                {
                    return InputFormatterResult.SuccessAsync(task.Result);
                }

                return FinishReadRequestBodyAsync(task);
            }
            catch (Exception ex)
            {
                context.ModelState.AddModelError("CBOR", ex.Message);
                return failureTask;
            }

            async Task<InputFormatterResult> FinishReadRequestBodyAsync(ValueTask<object> localTask)
            {
                object result = await localTask.ConfigureAwait(false);
                return InputFormatterResult.Success(result);
            }
        }
    }
}
