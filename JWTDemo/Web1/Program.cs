using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using Web1;
using Common.Authorize;
using Common;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile("appsettingsdevelopment.json",optional:true,reloadOnChange:true);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#region JWT»œ÷§◊¢»Î            
builder.Services.AddAuthentication_JWTSetup();
builder.Services.AddAuthorizationSetup();
#endregion
builder.Services.AddTransient<IUser,AspNetUser>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
