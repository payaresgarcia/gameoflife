using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using GameOfLife.Domain.Interfaces;
using GameOfLife.Repositories;
using GameOfLife.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLogging();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddSingleton<IDynamoDBContext>(provider =>
{
    IAmazonDynamoDB client = new AmazonDynamoDBClient();
    return new DynamoDBContext(client);
});
builder.Services.AddSingleton<IBoardService, BoardService>();
builder.Services.AddSingleton<IBoardRepository, BoardDynamoRepository>();

builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApiVersioning();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
