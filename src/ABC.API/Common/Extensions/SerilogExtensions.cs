using Serilog;
using Serilog.Events;

namespace ABC.API.Common.Extensions;

public static class SerilogExtensions
{
    public static void AddSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, configuration) => 
            configuration.ReadFrom.Configuration(context.Configuration));
    }
    
    public static void UseSerilog(this WebApplication app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = 
                "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms" +
                Environment.NewLine + "--------------------------------------------------";
            options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Information;
        });
    }
}