using Application.Extentions;
using Application.Services;
using Domain.Data;
using Microsoft.EntityFrameworkCore;
using RaesShopNew.Components;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRazorComponents()   
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<DataContext>(a =>
    a.UseSqlServer(ConstantTypes.ConnectionString));
builder.Services.AddControllers();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<Application.Helpers.Cpanel.FileUpload>();
builder.Services.AddScoped(a => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7154/")
});
builder.Services.AddScoped<FeedbackService>();
builder.Services.AddDbContext<DataContext>(a =>
    a.UseSqlServer(ConstantTypes.ConnectionString));
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()//builder.WithOrigins("https://example.com", "https://api.example.com") 

       .AllowAnyMethod()
               .AllowAnyHeader();
    });
});




builder.Services.AddSingleton<BasketService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.MapControllers();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
