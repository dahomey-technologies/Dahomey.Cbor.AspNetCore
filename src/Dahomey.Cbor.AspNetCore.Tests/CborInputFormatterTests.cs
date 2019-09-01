using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Dahomey.Cbor.AspNetCore.Tests
{
    public class CborInputFormatterTests
    {
        [Fact]
        public async Task ReadSimpleObject()
        {
            const string hexBuffer = "AE67426F6F6C65616EF56553427974650D64427974650C65496E7431360E6655496E7431360F65496E743332106655496E7433321165496E743634126655496E7436341366537472696E6766737472696E676653696E676C65FA41A1AE1466446F75626C65FB40363AE147AE147B684461746554696D6574323031342D30322D32315431393A30303A30305A64456E756D6656616C756531";
            byte[] buffer = hexBuffer.HexToBytes();

            Mock<HttpRequest> httpRequest = new Mock<HttpRequest>(MockBehavior.Strict);
            httpRequest.Setup(r => r.ContentType).Returns("application/cbor");
            httpRequest.Setup(r => r.ContentLength).Returns(buffer.Length);
            httpRequest.Setup(r => r.Body).Returns(new MemoryStream(buffer));

            Mock<HttpContext> httpContext = new Mock<HttpContext>(MockBehavior.Strict);
            httpContext.Setup(c => c.Request).Returns(httpRequest.Object);

            Mock<ModelMetadata> modelMetadata = new Mock<ModelMetadata>(
                MockBehavior.Strict,
                ModelMetadataIdentity.ForType(typeof(SimpleObject)));

            InputFormatterContext context = new InputFormatterContext(
                httpContext.Object,
                nameof(SimpleObject),
                new ModelStateDictionary(),
                modelMetadata.Object,
                (stream, encoding) => new StreamReader(stream, encoding));

            IInputFormatter inputFormatter = new CborInputFormatter(null);

            Assert.True(inputFormatter.CanRead(context));

            InputFormatterResult result = await inputFormatter.ReadAsync(context);

            Assert.False(result.HasError);
            Assert.True(result.IsModelSet);

            SimpleObject obj = (SimpleObject)result.Model;

            Assert.NotNull(obj);
            Assert.True(obj.Boolean);
            Assert.Equal(12, obj.Byte);
            Assert.Equal(13, obj.SByte);
            Assert.Equal(14, obj.Int16);
            Assert.Equal(15, obj.UInt16);
            Assert.Equal(16, obj.Int32);
            Assert.Equal(17u, obj.UInt32);
            Assert.Equal(18, obj.Int64);
            Assert.Equal(19ul, obj.UInt64);
            Assert.Equal(20.21f, obj.Single);
            Assert.Equal(22.23, obj.Double);
            Assert.Equal("string", obj.String);
            Assert.Equal(new DateTime(2014, 02, 21, 19, 0, 0, DateTimeKind.Utc), obj.DateTime);
            Assert.Equal(EnumTest.Value1, obj.Enum);
        }

        [Theory]
        [InlineData("application/json", "")]
        [InlineData("application/cbor", "application/cbor")]
        public void GetSupportedContentTypes(string actualContentType, string expectedContentType)
        {
            IApiRequestFormatMetadataProvider apiRequestFormatMetadataProvider = new CborInputFormatter(null);
            IReadOnlyList<string> contentTypes
                = apiRequestFormatMetadataProvider.GetSupportedContentTypes(
                    actualContentType, typeof(object));

            if (string.IsNullOrEmpty(expectedContentType))
            {
                Assert.Null(contentTypes);
            }
            else
            {
                Assert.Equal(1, contentTypes.Count);
                Assert.Equal(expectedContentType, contentTypes[0]);
            }
        }
    }
}
