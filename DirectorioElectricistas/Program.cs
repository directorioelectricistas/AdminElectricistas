using DirectorioElectricistas.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configurar SmtpSettings a partir de appsettings.json
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

// Registrar EmailService en el contenedor de servicios
builder.Services.AddTransient<EmailService>();

// Servicio password
builder.Services.AddSingleton<PasswordService>();

// Configurar la autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
    });

// Configurar la sesión
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Comentar HTTPS redirection temporalmente para Vercel
// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Configurar autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// Configurar sesión
app.UseSession();

// Cambiar temporalmente la ruta por defecto a Home/Index para pruebas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
