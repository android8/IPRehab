[Net 5 CORE Web API CORS Configuration](https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-5.0)

There are three ways to enable CORS:

In middleware using a named policy or default policy.

> - Using endpoint routing.
>
> - With the [EnableCors] attribute.
>
> - Using the [EnableCors] attribute with a named policy provides the finest control in limiting endpoints that support CORS.

 Warning

> UseCors must be called in thecorrect order. For more information, see Middleware order. For example, UseCors must be called before UseResponseCaching when using UseResponseCaching.

[Set the allowed origins](https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-5.0#set-the-allowed-origins)

> AllowAnyOrigin: Allows CORS requests from all origins with any scheme (http or https). AllowAnyOrigin is insecure because any website can make cross-origin requests to the app.