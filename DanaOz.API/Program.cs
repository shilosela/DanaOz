using DanaOz.BLL.Services;
using DanaOz.DAL.Context;
using DanaOz.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Twilio.AspNet.Core;

var builder = WebApplication.CreateBuilder(args);

// =====================
// DAL — Database
// =====================
builder.Services.AddDbContext<DanaOzDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("SupabaseConnection")));

// =====================
// DAL — Repositories
// =====================
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<ChatLogRepository>();
builder.Services.AddScoped<UserClassRepository>();
builder.Services.AddScoped<UserPointsRepository>();
builder.Services.AddScoped<UserMaterialRepository>();
builder.Services.AddScoped<ReminderRepository>();
builder.Services.AddScoped<UserSettingsRepository>();

// =====================
// BLL — Services
// =====================
builder.Services.AddScoped<ConversationService>();
builder.Services.AddScoped<OnboardingService>();
builder.Services.AddScoped<AIService>();

// =====================
// API
// =====================
builder.Services.AddTwilioRequestValidation();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();