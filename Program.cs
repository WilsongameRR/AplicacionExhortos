using AplicacionExhortos.Data;
using AplicacionExhortos.Data.Repositories;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Configurar cultura global
var culture = new CultureInfo("es-MX");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

// Servicios MVC
builder.Services.AddControllersWithViews();

// Registro de dependencias
builder.Services.AddSingleton<BDConnection>();
builder.Services.AddScoped<LoginRepository>();
builder.Services.AddScoped<TuaRepository>();
builder.Services.AddScoped<TipoDiligenciaRepository>();
builder.Services.AddScoped<DiligenciasRepository>();
builder.Services.AddScoped<ConsultaExhortoRepository>();
builder.Services.AddScoped<ExhortosRepository>();

// Configuración de sesión
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configuración del pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

app.Run();