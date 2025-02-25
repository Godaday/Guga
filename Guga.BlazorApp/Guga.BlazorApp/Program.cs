using ColinChang.RedisHelper;
using Guga.BlazorApp;
using Guga.BlazorApp.Components;
using Guga.Collector;
using Guga.Core;
using Guga.Core.Init;
using Guga.Options.Collector;
using Guga.Redis;
using Guga.Redis.ConfigModels;
using MudBlazor.Services;
using SqlSugar;

var builder = WebApplication.CreateBuilder(args);
//ע��MudBlazor����
builder.Services.AddMudServices();

//ע���źŽ�������ط���
builder.Services.Configure<RedisKeyOptions>(builder.Configuration.GetSection(nameof(RedisKeyOptions)));//redis key����
builder.Services.Configure<RedisHelperOptions>(builder.Configuration.GetSection(nameof(RedisHelperOptions)));//redis ��������
builder.Services.Configure<LinkConectionOptions>(builder.Configuration.GetSection(nameof(LinkConectionOptions)));//��· ��������

builder.Services.Configure<ServerOptions>(builder.Configuration.GetSection(nameof(ServerOptions)));//��������
builder.Services.AddGugaRedisServices(builder.Configuration.GetSection(nameof(RedisHelperOptions)));//redis���ӷ���
builder.Services.AddGugaCoreServices();//���ķ���
builder.Services.AddGugaCollectorServices();//�ɼ���
// ע����·��ʼ������
builder.Services.AddHostedService<PLCLinksInitHostedService>();

// MySQL �����ַ���
var connectionString = builder.Configuration.GetConnectionString("MySQL");

// ע�� SqlSugar
builder.Services.AddSingleton<ISqlSugarClient>(s =>
{
    return new SqlSugarClient(new ConnectionConfig()
    {
        ConnectionString = connectionString,
        DbType = DbType.MySql,    // ���ݿ�����
        IsAutoCloseConnection = true, // �Զ��ͷ�
        InitKeyType = InitKeyType.Attribute // ͨ������ʶ������
    });
});


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();
//IAM  code First
using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    dbInitializer.Init();
}
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
