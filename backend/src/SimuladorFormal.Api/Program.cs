using System.Text.Json.Serialization;
using SimuladorFormal.Api.Infra;
using SimuladorFormal.AutomatosFinitos;
using SimuladorFormal.AutomatosPilha;
using SimuladorFormal.Gramaticas;
using SimuladorFormal.Linguagens;
using SimuladorFormal.Turing;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers(o => o.Filters.Add<ErroValidacaoFilter>())
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Cada módulo registra seus próprios serviços e controllers.
builder.Services
    .AddModuloLinguagens()        // Módulo 1
    .AddModuloAutomatosFinitos()  // Módulo 2
    .AddModuloGramaticas()        // Módulo 3
    .AddModuloAutomatosPilha()    // Módulo 4
    .AddModuloTuring();           // Módulo 5

// Em desenvolvimento o front (Vite, porta 5173) chama a API diretamente.
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseCors();

// Depois de "npm run build", o front compilado fica em wwwroot e é servido pela própria API.
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();

public partial class Program;
