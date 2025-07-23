using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Project21AS.Server.Middleware
{
    // Middleware that proxies FM220U-LI fingerprint requests to the local service
    public class Fm220Middleware
    {
        private const string BaseUri = "http://localhost:8005/fm220/";
        private readonly RequestDelegate _next;
        private readonly HttpClient _httpClient;

        public Fm220Middleware(RequestDelegate next, HttpClient httpClient)
        {
            _next = next;
            _httpClient = httpClient;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/fm220"))
            {
                var downstreamUrl = BaseUri + context.Request.Path.Value!.Substring("/fm220".Length).TrimStart('/');

                if (HttpMethods.IsPost(context.Request.Method))
                {
                    string body = await new StreamReader(context.Request.Body).ReadToEndAsync();
                    using var content = new StringContent(body, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync(downstreamUrl, content);
                    context.Response.StatusCode = (int)response.StatusCode;
                    await response.Content.CopyToAsync(context.Response.Body);
                    return;
                }
                else if (HttpMethods.IsGet(context.Request.Method))
                {
                    var response = await _httpClient.GetAsync(downstreamUrl);
                    context.Response.StatusCode = (int)response.StatusCode;
                    await response.Content.CopyToAsync(context.Response.Body);
                    return;
                }
            }

            await _next(context);
        }
    }
}
