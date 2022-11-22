using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Insurance.Data;
using System;

var builder = WebApplication.CreateBuilder(args);
  
builder.Services.AddDbContext<AuthDbContext>(options=>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefualtConnection")));

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddMvc();
builder.Services.AddCors(options => options.AddPolicy(name: "InsuranceAngular",
    policy =>
    {
        policy.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader();
    }
    ));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("InsuranceAngular");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();








