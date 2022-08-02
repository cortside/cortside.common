using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Cortside.Common.Messages.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Cortside.Common.Correlation.Tests {
    public class CorrelationIdTest {
        [Fact]
        public async Task ReturnsCorrelationIdInResponseHeader() {
            var builder = new WebHostBuilder()
               .Configure(app => {
                   app.UseMiddleware<CorrelationMiddleware>();
               })
               .ConfigureServices(services => {
                   services.AddControllersWithViews(options => {
                       options.Filters.Add<MessageExceptionResponseFilter>();
                   });
                   services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
               });

            using var server = new TestServer(builder);

            var response = await server.CreateClient().GetAsync("");

            var header = response.Headers.GetValues("X-Correlation-Id");

            Assert.NotNull(header);
        }

        [Fact]
        public async Task ReturnsRequestCorrelationIdInResponseHeader() {
            var builder = new WebHostBuilder()
               .Configure(app => app.UseMiddleware<CorrelationMiddleware>())
               .ConfigureServices(services => {
                   services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
               });

            using var server = new TestServer(builder);

            var request = new HttpRequestMessage();
            request.Headers.Add("X-Correlation-Id", "ABC123");

            var response = await server.CreateClient().SendAsync(request);

            var header = response.Headers.GetValues("X-Correlation-Id").FirstOrDefault();

            Assert.Equal("ABC123", header);
        }

        [Fact]
        public async Task ReturnsCorrelationIdInResponseHeaderOnUnhandledException() {
            var builder = new WebHostBuilder()
               .Configure(app => {
                   app.UseMiddleware<CorrelationMiddleware>();
                   app.UseExceptionHandler(error => error.Run(_ => Task.CompletedTask));

                   app.Use(async (ctx, next) => {
                       // trigger exception
                       var z = 0;
                       var _ = 1 / z;

                       await next.Invoke();
                   });
               })
               .ConfigureServices(services => {
                   services.AddControllersWithViews(options => {
                       options.Filters.Add<MessageExceptionResponseFilter>();
                   });
                   services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
               });

            using var server = new TestServer(builder);

            var response = await server.CreateClient().GetAsync("");
            var content = await response.Content.ReadAsStringAsync();

            var header = response.Headers.GetValues("X-Correlation-Id").FirstOrDefault();

            Assert.NotNull(header);
            Assert.Equal("", content);
        }
    }
}
