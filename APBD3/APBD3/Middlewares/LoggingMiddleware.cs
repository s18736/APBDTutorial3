using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APBD3.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            httpContext.Request.EnableBuffering();
            var output = await prepareOutputAsync(httpContext.Request);
            writeToAFile(output.ToString());
            httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
            await _next(httpContext);

        }

        private async Task<string> prepareOutputAsync(HttpRequest request)
        {
            var message = "";
            message += ("1. " + request.Method + "\n");
            message += ("2. " + request.Path + "\n");
            using (StreamReader reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
            {
                var bodyStr = await reader.ReadToEndAsync();
                message += ("3. " + bodyStr + "\n");
            }
            
            message += ("4. " + request.QueryString.ToString() + "\n");
            return message;
        }

        private async void writeToAFile(string data)
        {
            using (StreamWriter writer = new StreamWriter("requestLog.txt", true))
            {
                await writer.WriteAsync(data);
                writer.Flush();
                
            }
                
        }

        
    }
}
