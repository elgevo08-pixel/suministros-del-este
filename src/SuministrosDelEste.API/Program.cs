using Microsoft.EntityFrameworkCore;
using SuministrosDelEste.Application.Factories;
using SuministrosDelEste.Application.Ports;
using SuministrosDelEste.Application.Strategies;
using SuministrosDelEste.Application.UseCases.CalcularDescuento;
using SuministrosDelEste.Application.UseCases.ConsultarInventario;
using SuministrosDelEste.Application.UseCases.RegistrarMaterial;
using SuministrosDelEste.Infrastructure.DependencyInjection;
using SuministrosDelEste.Infrastructure.Persistence.Context;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ============================================================
// CAPA DE INFRAESTRUCTURA — Adaptadores secundarios + seguridad
// Detecta automáticamente SQLite (demo) o SQL Server (producción)
// ============================================================
builder.Services.AddInfrastructure(builder.Configuration);

// ============================================================
// CAPA DE APLICACIÓN — Factories, Strategies, Use Cases
// ============================================================
builder.Services.AddScoped<IMaterialFactory, MaterialFactory>();
builder.Services.AddScoped<IStockAlertStrategy, StockMinimoAlertStrategy>();
builder.Services.AddScoped<IRegistrarMaterialUseCase, RegistrarMaterialHandler>();
builder.Services.AddScoped<IConsultarInventarioUseCase, ConsultarInventarioHandler>();

// Descuentos (Tarea 4 — SOLID): 3 estrategias intercambiables (OCP).
// Agregar un tipo de cliente nuevo = una línea más aquí, ninguna de las anteriores cambia.
builder.Services.AddScoped<IDescuentoStrategy, DescuentoPremiumStrategy>();
builder.Services.AddScoped<IDescuentoStrategy, DescuentoFrecuenteStrategy>();
builder.Services.AddScoped<IDescuentoStrategy, DescuentoPorVolumenStrategy>();
builder.Services.AddScoped<ICalcularDescuentoUseCase, CalcularDescuentoHandler>();

// ============================================================
// CAPA DE PRESENTACIÓN — Adaptadores primarios (REST)
// ============================================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Suministros del Este — ERP API",
        Version = "v1",
        Description = "API REST del módulo de Inventario. Arquitectura Hexagonal + DDD + SOLID."
    });
});

// ============================================================
WebApplication app = builder.Build();
// ============================================================

// Aplicar migraciones y seed automático al arrancar (modo demo/desarrollo)
bool isDemoMode = builder.Configuration.GetValue<bool>("Jwt:DemoMode");
if (isDemoMode || app.Environment.IsDevelopment())
{
    using IServiceScope scope = app.Services.CreateScope();
    AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Suministros del Este v1"));

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
