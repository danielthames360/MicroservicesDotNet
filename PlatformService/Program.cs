using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//Postgre
Console.WriteLine("--> Using Postgres Db");
builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseNpgsql(builder.Configuration.GetConnectionString("PlatformsPostgresConn")));

//SQL
// if (builder.Environment.IsProduction())
// {
//     Console.WriteLine("--> Using SqlServer Db");
//     builder.Services.AddDbContext<AppDbContext>(opt =>
//             opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsSQLConn")));
// }
// else
// {
//     Console.WriteLine("--> Using InMem Db");
//     builder.Services.AddDbContext<AppDbContext>(opt =>
//         opt.UseInMemoryDatabase("InMem"));
// }

builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

PrepDb.PrepPopulation(app, builder.Environment.IsProduction());

app.MapControllers();

app.Run();
