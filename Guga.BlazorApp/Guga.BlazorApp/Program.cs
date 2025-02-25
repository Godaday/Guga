using ColinChang.RedisHelper;
using Guga.BlazorApp;
using Guga.BlazorApp.Components;
using Guga.Collector;
using Guga.Core;
using Guga.Options.Collector;
using Guga.Redis;
using Guga.Redis.ConfigModels;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);
//注册MudBlazor服务
builder.Services.AddMudServices();

//注册信号解析器相关服务
builder.Services.Configure<RedisKeyOptions>(builder.Configuration.GetSection(nameof(RedisKeyOptions)));//redis key配置
builder.Services.Configure<RedisHelperOptions>(builder.Configuration.GetSection(nameof(RedisHelperOptions)));//redis 连接配置
builder.Services.Configure<LinkConectionOptions>(builder.Configuration.GetSection(nameof(LinkConectionOptions)));//链路 连接配置

builder.Services.Configure<ServerOptions>(builder.Configuration.GetSection(nameof(ServerOptions)));//服务配置
builder.Services.AddGugaRedisServices(builder.Configuration.GetSection(nameof(RedisHelperOptions)));//redis连接服务
builder.Services.AddGugaCoreServices();//核心服务
builder.Services.AddGugaCollectorServices();//采集器
// 注册链路初始化服务
builder.Services.AddHostedService<PLCLinksInitHostedService>();




// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Guga.BlazorApp.Client._Imports).Assembly);

app.Run();
