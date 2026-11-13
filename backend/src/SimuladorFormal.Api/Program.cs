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

builder.Services
    .AddModuloLinguagens()
    .AddModuloAutomatosFinitos()
    .AddModuloGramaticas()
    .AddModuloAutomatosPilha()
    .AddModuloTuring();

builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseCors();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();

public partial class Program;
