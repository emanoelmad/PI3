using AppShowDoMilhao.Data;
using Microsoft.EntityFrameworkCore;

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
    });

// Configurar cache distribu�do e sess�o
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tempo de expira��o da sess�o
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
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
app.UseSession(); // Middleware de sess�o deve ser chamado antes de `UseAuthentication`
app.UseAuthentication(); // Adiciona a autentica��o
app.UseAuthorization();  // Adiciona a autoriza��o

app.MapControllers();

app.Run();

