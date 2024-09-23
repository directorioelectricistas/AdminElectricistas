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

//Servicio password
builder.Services.AddSingleton<PasswordService>();


// Configurar la autenticaci�n con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied"; // Ruta para acceso denegado si es necesario
    });

// Configurar la sesi�n
builder.Services.AddDistributedMemoryCache(); // Usar cach� en memoria para la sesi�n
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Establecer tiempo de espera
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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Configurar autenticaci�n y autorizaci�n
app.UseAuthentication();
app.UseAuthorization();

// Configurar sesi�n
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

// Mueve esta configuraci�n a la parte superior para que sea la primera ruta verificada
app.MapControllerRoute(
    name: "home",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .RequireAuthorization(); // Asegura que la ruta est� protegida


app.Run();
