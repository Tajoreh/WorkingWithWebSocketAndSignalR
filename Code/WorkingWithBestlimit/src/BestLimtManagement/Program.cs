using BaseLimitManagement.Contracts;
using BestlimitManagement.Services;
using BestLimtManagement;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.WebSockets;
using Quartz;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configuration.Sources.Clear();

var appSettingsPath = $"appsettings.json";

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile(path: appSettingsPath, optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();
builder.Services.AddHttpClient();

var hostConfig = builder.Configuration.GetSection(nameof(HostConfig)).Get<HostConfig>();

builder.Services.AddSingleton<HostConfig>(hostConfig);

builder.Services.AddTransient<IDataService, DataService>();
builder.Services.AddHttpClient();

builder.Services.AddSingleton<WebSocketPublisher>();
builder.Services.AddSingleton<IMessagePublisher, WebSocketPublisher>();

builder.Services.AddSignalR();
builder.Services.AddSingleton<IMessagePublisher, SignalRPublisher>();
builder.Services.AddSingleton<SignalRPublisher>();


builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    

    q.AddJob<SignalRJob>(opts => opts.WithIdentity(nameof(SignalRJob)));
    q.AddJob<WebsocketJob>(opts => opts.WithIdentity(nameof(WebsocketJob)));

    q.AddTrigger(opts => opts
        .ForJob(nameof(SignalRJob))
        .WithIdentity("BestLimitJob-trigger")
        .WithSimpleSchedule(x => x
            .WithInterval(TimeSpan.FromSeconds(1))
            .RepeatForever()
        )
    );
    q.AddTrigger(opts => opts
        .ForJob(nameof(WebsocketJob))
        .WithIdentity("BestLimitJob-trigger")
        .WithSimpleSchedule(x => x
            .WithInterval(TimeSpan.FromSeconds(1))
            .RepeatForever()
        )
    );
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);



builder.Services.AddCors(options =>
{
    options.AddPolicy("Policy",
                      policy =>
                      {
                          policy
                          //.AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .SetIsOriginAllowed(x=>true)
                                .AllowCredentials();
                      });
});


var app = builder.Build();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();
app.UseCors("Policy");

app.UseHttpsRedirection();

app.UseAuthorization();





app.MapControllers();

#region WebSocket

app.UseWebSockets();

app.Map("/ws/ins", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var insCode = context.Request.Query["insCode"];
        var socket = await context.WebSockets.AcceptWebSocketAsync();

        var wsService = (WebSocketPublisher)context.RequestServices.GetRequiredService<IMessagePublisher>();
        await wsService.HandleClient(socket, insCode);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

#endregion

#region SignalR
app.MapHub<BestLimitHub>("/bestlimitHub");

#endregion

app.Run();
