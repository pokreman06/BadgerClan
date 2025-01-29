using Microsoft.Extensions.Configuration;
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
    }
    public class APIService(HttpClient client): IAPIService
    {
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
