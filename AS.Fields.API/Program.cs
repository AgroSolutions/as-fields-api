using AS.Fields.API;
using AS.Fields.API.Configurations;
using AS.Fields.Application;
using AS.Fields.Application.Middlewares;
using AS.Fields.Infra;
using AS.Fields.Infra.Messaging.Config;
using AS.Fields.Infra.Persistence.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();


#region SQL Server
builder.Services.AddDbContext<ASFieldsContext>(options =>
{
    options.UseSqlServer(
       builder.Configuration.GetConnectionString("ASFieldsConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null)
    );
});
#endregion

#region OpenAPI / Swagger
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerConfiguration();
#endregion

#region Dependency Injection

builder.Services.AddJwtAuthentication(builder.Configuration);

#region Amazon SQS
var messagingSection = builder.Configuration.GetSection("Messaging");
if (!messagingSection.Exists())
    throw new InvalidOperationException("Section 'Messaging' not found in configuration.");

var queuesSection = messagingSection.GetSection("Queues");
builder.Services.Configure<QueuesOptions>(queuesSection);

builder.Services.ConfigureAmazonSQS(builder.Configuration);
#endregion

builder.Services.AddInfraModules();
builder.Services.AddApplicationModules();

#endregion

builder.Services.AddHostedService<Worker>();

var app = builder.Build();

app.UseSwaggerConfiguration();
app.MapOpenApi();

try
{
    await using var scope = app.Services.CreateAsyncScope();
    await using var dbContext = scope.ServiceProvider.GetRequiredService<ASFieldsContext>();
    bool criado = dbContext.Database.EnsureCreated();
    Console.WriteLine(criado ? "Banco de dados criado com sucesso." : "Banco de dados já existe.");
}
catch (Exception ex)
{
    Console.WriteLine($"Erro ao criar banco de dados: {ex.Message}");
}

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();


app.UseAuthorization();

app.MapControllers();

app.Run();
