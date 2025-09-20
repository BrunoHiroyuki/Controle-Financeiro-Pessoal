using ControleFinanceiroPessoal.Application.Interfaces;
using ControleFinanceiroPessoal.Application.Mappings;
using ControleFinanceiroPessoal.Application.Services;
using ControleFinanceiroPessoal.Domain.Interfaces;
using ControleFinanceiroPessoal.Infrastructure.Data.Context;
using ControleFinanceiroPessoal.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Entity Framework
builder.Services.AddDbContext<FinanceiroContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Repositories
builder.Services.AddScoped<IMovimentacaoRepository, MovimentacaoRepository>();

// Services
builder.Services.AddScoped<IMovimentacaoService, MovimentacaoService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddSingleton<JobService>();
builder.Services.AddHostedService<JobService>(provider => provider.GetRequiredService<JobService>());

// Email Settings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:8080", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowVueApp");

app.UseAuthorization();

app.MapControllers();

// Verifica se a base de dados foi criada
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FinanceiroContext>();
    context.Database.EnsureCreated();
}

app.Run();