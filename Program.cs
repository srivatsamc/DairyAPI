using Dairy.Application;
using DbHelper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Dairy.API.Models.Login;
using FluentValidation;
using Dairy.API.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection("DairyDb:ConnectionStrings"));

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Dairy API", Version = "v1" });
});

builder.Services.AddScoped<DapperContext>();
builder.Services.AddTransient<IDapperHelper, DapperHelper>();

builder.Services.ResolveConfigurationApplicationDI(builder.Configuration);

//Validators
builder.Services.AddScoped<IValidator<LoginModel>, LoginRuleRequestValidator>();

//Model mappers
/*-------------*/

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder =>
                      {
                          builder.WithOrigins("https://localhost:7270/",
                                              "http://www.contoso.com");
                      });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "/swagger/v1/swagger.json";
    });
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dairy API V1");
    });
    //app.UseExceptionHandler("/Home/Error");
    //// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
}


//app.UseHttpsRedirection();

//app.UseStaticFiles();

app.UseRouting();

app.UseCors(MyAllowSpecificOrigins);


//app.UseAuthorization();

app.MapControllers();

app.Run();
