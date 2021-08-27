using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;

using Brimborium.WebDavServer.Utils.UAParser;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Brimborium.WebDavServer.AspNetCore {
    /// <summary>
    /// The ASP.NET core specific implementation of the <see cref="IWebDavContext"/> interface
    /// </summary>
    public sealed class WebDavContext : IWebDavContext {
        private readonly HttpContext _HttpContext;

        private readonly Lazy<Uri> _ServiceAbsoluteRequestUrl;

        private readonly Lazy<Uri> _ServiceRelativeRequestUrl;

        private readonly Lazy<Uri> _ServiceBaseUrl;

        private readonly Lazy<Uri> _ServiceRootUrl;

        private readonly Lazy<Uri> _PublicRelativeRequestUrl;

        private readonly Lazy<Uri> _PublicAbsoluteRequestUrl;

        private readonly Lazy<Uri> _PublicBaseUrl;

        private readonly Lazy<Uri> _PublicRootUrl;

        private readonly Lazy<Uri> _PublicControllerUrl;

        private readonly Lazy<Uri> _ControllerRelativeUrl;

        private readonly Lazy<Uri> _ActionUrl;

        private readonly Lazy<WebDavRequestHeaders> _RequestHeaders;

        private readonly Lazy<IUAParserOutput> _DetectedClient;

        private readonly Lazy<IPrincipal> _Principal;

        private readonly Lazy<IWebDavDispatcher> _Dispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebDavContext"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider used to get the <see cref="IWebDavDispatcher"/> with</param>
        /// <param name="httpContextAccessor">The <see cref="HttpContext"/> accessor</param>
        /// <param name="options">The options for the <see cref="WebDavContext"/></param>
        public WebDavContext(IServiceProvider serviceProvider, IHttpContextAccessor httpContextAccessor, IOptions<WebDavHostOptions> options) {
            var opt = options?.Value ?? new WebDavHostOptions();
            var httpContext = httpContextAccessor.HttpContext!;
            this._HttpContext = httpContext;
            this._ServiceBaseUrl = new Lazy<Uri>(() => BuildServiceBaseUrl(httpContext));
            this._PublicBaseUrl = new Lazy<Uri>(() => BuildPublicBaseUrl(httpContext, opt));
            this._PublicRootUrl = new Lazy<Uri>(() => new Uri(this.PublicBaseUrl, "/"));
            this._ServiceAbsoluteRequestUrl = new Lazy<Uri>(() => BuildAbsoluteServiceUrl(httpContext));
            this._ServiceRootUrl = new Lazy<Uri>(() => new Uri(this.ServiceAbsoluteRequestUrl, "/"));
            this._ServiceRelativeRequestUrl = new Lazy<Uri>(() => this.ServiceRootUrl.MakeRelativeUri(this.ServiceAbsoluteRequestUrl));
            this._PublicAbsoluteRequestUrl = new Lazy<Uri>(() => new Uri(this.PublicBaseUrl, this.ServiceBaseUrl.MakeRelativeUri(this.ServiceAbsoluteRequestUrl)));
            this._ActionUrl = new Lazy<Uri>(() => new Uri(httpContext.GetRouteValue("path")?.ToString() ?? string.Empty, UriKind.RelativeOrAbsolute));
            this._PublicRelativeRequestUrl = new Lazy<Uri>(() => new Uri(this.PublicBaseUrl, this.ActionUrl));
            this._PublicControllerUrl = new Lazy<Uri>(() => new Uri(this.PublicBaseUrl, this.ControllerRelativeUrl));
            this._ControllerRelativeUrl = new Lazy<Uri>(
                () => {
                    var path = httpContext.GetRouteValue("path")?.ToString();
                    var input = this.ServiceAbsoluteRequestUrl.ToString();
                    string remaining;
                    if (path != null) {
                        var pattern = string.Format("{0}$", Regex.Escape(path));
                        remaining = Regex.Replace(input, pattern, string.Empty);
                    } else {
                        remaining = input;
                    }

                    var serviceControllerAbsoluteUrl = new Uri(remaining);
                    var result = this.ServiceBaseUrl.MakeRelativeUri(serviceControllerAbsoluteUrl);
                    return result;
                });
            this._RequestHeaders = new Lazy<WebDavRequestHeaders>(() => {
                var request = httpContext.Request;
                var headerItems = request.Headers.Select(x => new KeyValuePair<string, IEnumerable<string>>(x.Key, x.Value));
                return new WebDavRequestHeaders(headerItems, this);
            });
            this._DetectedClient = new Lazy<IUAParserOutput>(() => DetectClient(httpContext));
            this._Principal = new Lazy<IPrincipal>(() => httpContext.User);
            this._Dispatcher = new Lazy<IWebDavDispatcher>(serviceProvider.GetRequiredService<IWebDavDispatcher>);
        }

        /// <inheritdoc />
        public Uri ServiceRelativeRequestUrl => this._ServiceRelativeRequestUrl.Value;

        /// <inheritdoc />
        public Uri ServiceAbsoluteRequestUrl => this._ServiceAbsoluteRequestUrl.Value;

        /// <inheritdoc />
        public Uri ServiceBaseUrl => this._ServiceBaseUrl.Value;

        /// <inheritdoc />
        public Uri ServiceRootUrl => this._ServiceRootUrl.Value;

        /// <inheritdoc />
        public Uri PublicRelativeRequestUrl => this._PublicRelativeRequestUrl.Value;

        /// <inheritdoc />
        public Uri PublicAbsoluteRequestUrl => this._PublicAbsoluteRequestUrl.Value;

        /// <inheritdoc />
        public Uri PublicControllerUrl => this._PublicControllerUrl.Value;

        /// <inheritdoc />
        public Uri PublicBaseUrl => this._PublicBaseUrl.Value;

        /// <inheritdoc />
        public Uri PublicRootUrl => this._PublicRootUrl.Value;

        /// <inheritdoc />
        public Uri ControllerRelativeUrl => this._ControllerRelativeUrl.Value;

        /// <inheritdoc />
        public Uri ActionUrl => this._ActionUrl.Value;

        /// <inheritdoc />
        public IPrincipal User => this._Principal.Value;

        /// <inheritdoc />
        public string RequestProtocol => this._HttpContext.Request.Protocol;

        /// <inheritdoc />
        public IWebDavRequestHeaders RequestHeaders => this._RequestHeaders.Value;

        /// <inheritdoc />
        public IUAParserOutput DetectedClient => this._DetectedClient.Value;

        /// <inheritdoc />
        public IWebDavDispatcher Dispatcher => this._Dispatcher.Value;

        private static Uri BuildAbsoluteServiceUrl(HttpContext httpContext) {
            var request = httpContext.Request;
            var result = new StringBuilder();
            var basePath = request.PathBase.ToString();
            var path = request.Path.ToString();
            if (!basePath.EndsWith("/") && !path.StartsWith("/")) {
                basePath += "/";
            }

            result.Append(request.Scheme).Append("://").Append(request.Host)
                .Append(basePath)
                .Append(path);

            return new Uri(result.ToString());
        }

        private static Uri BuildPublicBaseUrl(HttpContext httpContext, WebDavHostOptions options) {
            if (options.BaseUrl == null) {
                return BuildServiceBaseUrl(httpContext);
            }

            var result = new StringBuilder();
            result.Append(options.BaseUrl);

            var resultUrl = result.ToString();
            if (!resultUrl.EndsWith("/", StringComparison.Ordinal)) {
                resultUrl += "/";
            }

            return new Uri(resultUrl, UriKind.RelativeOrAbsolute);
        }

        private static Uri BuildServiceBaseUrl(HttpContext httpContext) {
            var result = new StringBuilder();
            var request = httpContext.Request;
            result.Append(request.Scheme).Append("://").Append(request.Host)
                .Append(request.PathBase);

            var resultUrl = result.ToString();
            if (!resultUrl.EndsWith("/", StringComparison.Ordinal)) {
                resultUrl += "/";
            }

            return new Uri(resultUrl);
        }

        private static IUAParserOutput DetectClient(HttpContext httpContext) {
            var userAgent = httpContext.Request.Headers["User-Agent"].FirstOrDefault();
            return Parser.GetDefault().Parse(userAgent ?? string.Empty);
        }
    }
}
