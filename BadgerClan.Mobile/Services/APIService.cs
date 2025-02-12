using Grpc.Net.Client;
using gRPC.shared;
using Microsoft.Extensions.Configuration;
using ProtoBuf.Grpc.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BadgerClan.Mobile.Services
{
    public interface IAPIService
    {
        public Task Change(string name);
        public Task<string> Get();
        public void changeClient(HttpClient newC);
    }
    public class APIServiceG(string address) : IAPIService
    {
        public async Task Change(string name)
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;
            using  (var channel=GrpcChannel.ForAddress(address))
            {
                var client = channel.CreateGrpcService<IStyle>();
                int j = name.ToLower() switch
                {
                    "hold" => 1,// => new Hold(),
                    "attack" => 2, //=> new Attack(),
                    "basic" => 3, //=> new SuperSimpleExampleBot(),
                    _ => 0
                };
                await client.SendStyleAsync(new StyleRequest() { Style=name, StyleId=j});
            }
        }

        public void changeClient(HttpClient newC)
        {
            throw new NotImplementedException();
        }

        public async Task<string> Get()
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;
            using (var channel = GrpcChannel.ForAddress(address))
            {
                var client = channel.CreateGrpcService<IStyle>();
            var result = await client.GetStyleAsync();
                return result.Style;
            }
        }
    }
    public class APIService(HttpClient client): IAPIService
    {
        public void changeClient(HttpClient newC)
        {
            client=newC;
        }
        public async Task Change(string name) 
        {
            await client.PostAsync("Style/"+name, null);
        }

        public async Task<string> Get()
        {
            return await (await client.GetAsync("/Style")).Content.ReadAsStringAsync();
        }
    }
}
