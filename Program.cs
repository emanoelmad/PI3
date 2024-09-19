using AppShowDoMilhao.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços ao contêiner
builder.Services.AddControllers();

// Configurar o ApplicationDbContext para usar SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar o esquema de autenticação com cookies
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.Name = "YourAuthCookieName"; // Nome do cookie
        options.LoginPath = "/Auth/Login"; // Redireciona para o login se não estiver autenticado
        options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None; // Permite envio de cookies em requisições cross-origin
        options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always; // Exige HTTPS
    });

// Configurar cache distribuído e sessão
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tempo de expiração da sessão
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None; // Permite envio de cookies em requisições cross-origin
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

// Configurar o pipeline de requisição HTTP
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

// A ordem é importante aqui
app.UseCors("AllowSpecificOrigins"); // Adiciona o middleware CORS com a política específica
app.UseSession(); // Middleware de sessão deve ser chamado antes de `UseAuthentication`
app.UseAuthentication(); // Adiciona a autenticação
app.UseAuthorization();  // Adiciona a autorização

app.MapControllers();

app.Run();
