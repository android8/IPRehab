using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Net;

namespace IPRehabWebAPI2.Helpers
{
    /// <summary>
    /// extends IApplicationBuilder
    /// </summary>
    public static class ExceptionMiddlewareExtension
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    // setting the response code as internal server error.
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    if (contextFeature != null)
                    {
                        var ex = contextFeature?.Error;
                        var isDev = env.IsDevelopment();
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(
                            // using problem details object to  response to caller
                            new ProblemDetails
                            {
                                Type = ex.GetType().Name,
                                Status = (int)HttpStatusCode.InternalServerError,
                                Instance = contextFeature?.Path,
                                // i am just using generic statement.
                                // it can be customised based on path or any other condition
                                Title = isDev ? $"{ex.Message}" : "An error occurred.",
                                // in case of dev, it returns the complete stack trace.
                                Detail = isDev ? ex.StackTrace.Replace("\r\n", Environment.NewLine) : null
                            }));
                    }
                });
            });
        }
    }
}
