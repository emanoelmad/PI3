using AppShowDoMilhao.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

var builder = WebApplication.CreateBuilder(args);

// Adicionar servi�os ao cont�iner
builder.Services.AddControllers();

// Configurar o ApplicationDbContext para usar SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar o esquema de autentica��o com cookies
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.Name = "YourAuthCookieName"; // Nome do cookie
        options.LoginPath = "/Auth/Login"; // Redireciona para o login se n�o estiver autenticado
        options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None; // Permite envio de cookies em requisi��es cross-origin
        options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always; // Exige HTTPS
    });

// Configurar cache distribu�do e sess�o
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tempo de expira��o da sess�o
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None; // Permite envio de cookies em requisi��es cross-origin
    options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always; // Exige HTTPS
});

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder => builder.WithOrigins("https://127.0.0.1:5173") // Substitua pelo URL do seu frontend
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials()); // Permite o envio de cookies e credenciais
});

var app = builder.Build();

// Configurar o pipeline de requisi��o HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// A ordem � importante aqui
app.UseCors("AllowSpecificOrigins"); // Adiciona o middleware CORS com a pol�tica espec�fica
app.UseSession(); // Middleware de sess�o deve ser chamado antes de `UseAuthentication`
app.UseAuthentication(); // Adiciona a autentica��o
app.UseAuthorization();  // Adiciona a autoriza��o

app.MapControllers();

app.Run();
