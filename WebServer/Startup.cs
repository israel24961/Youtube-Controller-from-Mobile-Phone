using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace WebServer
{
    public class Startup
    {
        static Socket handler = null;
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            StartupSocketListener();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseWebSockets();
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        await ReceiveMessageAsync(context, webSocket);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await next();
                }
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();

        }
        public static void StartupSocketListener()
        {
            var TAG = "LocalSocket:";
            var debug = new System.Action<string>((string str) =>
            {
                System.Console.WriteLine(TAG + str);
            });

            debug("Host Name: " + System.Net.Dns.GetHostName());

            var localIP = Program.getLocalIP();

            debug("IP: " + localIP.ToString());

            var socketClient = new Socket(AddressFamily.InterNetwork,
                                            SocketType.Stream,
                                            ProtocolType.Tcp);
            try
            {
                socketClient.Connect(System.Net.IPAddress.Parse("127.0.0.128"), 34197);
                handler = socketClient;
            }
            catch (Exception)
            {
                Thread.Sleep(1000);
            }
            
        }
        private async Task ReceiveMessageAsync(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {

                Console.WriteLine("Message Received:" + System.Text.Encoding.ASCII.GetString(buffer));
                handler.SendAsync(new ArraySegment<byte>(buffer,0,result.Count),SocketFlags.None);
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
                //clear buffer
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = 0;
                }
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}
