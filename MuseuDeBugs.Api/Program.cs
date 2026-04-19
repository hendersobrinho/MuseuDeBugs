using Microsoft.EntityFrameworkCore;
using MuseuDeBugs.Api;
using MuseuDeBugs.Api.Services;


var builder = WebApplication.CreateBuilder(args); 
// cria o objeto principal de configuração da aplicação;
// a partir daqui eu configuro serviços e recursos que a API vai usar

// registra os controllers
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// registra o service para injeção de dependência
builder.Services.AddScoped<BugService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
