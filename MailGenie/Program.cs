
using MailGenie.Domain;
using MailGenie.Domain.Model.EmailTemplateModel;
using MailGenie.Domain.Model.Mail;
using MailGenie.Infra;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);


// 1. Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // your frontend URL
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<SmtpSetting>(builder.Configuration.GetSection("SmtpSettings"));

builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();

ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");

var app = builder.Build();

// 2. Use CORS middleware
app.UseCors("AllowSpecificOrigin");

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EmailGenie");
    c.RoutePrefix = "swagger"; // Swagger UI URL: http://yourdomain/swagger
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
