using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Brimborium.WebDavServer.AspNetCore.Formatters.Internal
{
    internal class WebDavXmlSerializerMvcOptionsSetup : IConfigureOptions<MvcOptions>
    {
        public void Configure(MvcOptions options)
        {
            options.InputFormatters.Add(new WebDavXmlSerializerInputFormatter(options));
        }
    }
}
