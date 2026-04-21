using Microsoft.EntityFrameworkCore; // para reconhecer AddDbContext, UseMySql e ServerVersion
using MuseuDeBugs.Api.Data; // para reconhecer o AppDbContext
using MuseuDeBugs.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<BugService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// aqui eu pego do appsettings a connection string chamada DefaultConnection

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    ));
// aqui eu registro o AppDbContext e digo para o EF Core usar MySQL com essa conexão
// essas opções montadas aqui depois serão recebidas no construtor do AppDbContext

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
