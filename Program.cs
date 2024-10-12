using Serilog;
using var log = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("log.txt")
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();

try
{

    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddSerilog(log);
    // builder.Logging.AddSerilog(log);
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();
    app.UseSerilogRequestLogging();
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    var summaries = new[]
    {
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

    app.MapGet("/weatherforecast", () =>
    {
        log.Information("run start : {function}", "GetWeatherForecast");
        var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
            .ToArray();
        log.Information("request data : {data}", forecast);
        log.Information("run end : {function}", "GetWeatherForecast");
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

    app.Run();
}
catch (Exception ex)
{
    Log.Error(ex, "Unhandled exception");
}
finally
{
    await Log.CloseAndFlushAsync(); // ensure all logs written before app exits
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
