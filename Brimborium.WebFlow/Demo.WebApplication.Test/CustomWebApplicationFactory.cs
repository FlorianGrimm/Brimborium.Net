using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Demo.WebApplication {
    public class CustomWebApplicationFactory<TStartup>
    : WebApplicationFactory<TStartup> where TStartup : class {
        protected override void ConfigureWebHost(IWebHostBuilder builder) {
            base.ConfigureWebHost(builder);
        }
    }
}
