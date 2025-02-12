
using grpc;
using gRPC.shared;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using ProtoBuf.Grpc.Server;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1;
    });

    options.ListenAnyIP(8585, listenOptions =>
    {
        listenOptions.UseHttps();
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});
builder.Services.AddSingleton<Mode>();
builder.Services.AddCodeFirstGrpc();
builder.Services.AddControllers();


var app = builder.Build();

app.MapControllers();

app.MapGrpcService<StyleService>();

app.Run();

public class StyleService(Mode mode) : IStyle
{
    public async Task<StyleResult> GetStyleAsync()
    {
        return new() { Style = mode.style.Name };
    }

    public async Task SendStyleAsync(StyleRequest styleResult)
    {
        mode.style = styleResult.StyleId switch
        {
            1 => new Hold(),
            2 => new Attack(),
            3 => new SuperSimpleExampleBot(),
            _ => new Default()
        };
    }
}