using SmartPipePlanner.Core.Search;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Core planner (DI)
builder.Services.AddScoped<PipePlanner>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    // 把 Swagger UI 掛在 root (/)
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartPipePlanner API");
        c.RoutePrefix = string.Empty; // ← 關鍵
    });
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
