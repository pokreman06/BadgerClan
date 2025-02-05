using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BadgerClan.Mobile.Services;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
namespace BadgerClan.Mobile.ModelView
{
    public partial class MainPageModelView : ObservableObject
    {
        IAPIService api;
        public MainPageModelView(IAPIService api)
        {
            ApiManagers = new();
            this.api = api;
            ApiManagers.Add(new("default", api));
        }
        [ObservableProperty]
        string tempName;
        [ObservableProperty]
        string tempPath;
        async Task Change(string command)
        {
            foreach(var api in ApiManagers)
            {
                if (api.Selected)
                {
                    await api.APIService.Change(command);
                    await api.Initialize();
                }
                
            }
        }
        [RelayCommand]
        public async Task Default()
        {
            await Change("Default");
        }
        [RelayCommand]
        public async Task Hold()
        {
            await Change("Hold");
        }
        [RelayCommand]
        public async Task Attack()
        {
            await Change("Attack");
        }
        [RelayCommand]
        public async Task Basic()
        {
            await Change("Basic");
        }
        [RelayCommand]
        public async Task LogAPI()
        {
            try
            {
                var value = new ApiManager(TempName, TempPath);
                await value.Initialize();
                ApiManagers.Add(value);
                TempName = "";
                TempPath = "";

            }
            catch { }
        }

        public ObservableCollection<ApiManager> ApiManagers { get; set; }
    }
    public partial class ApiManager : ObservableObject
    {
        [ObservableProperty]
        string name;
        [ObservableProperty]
        bool selected = true;
        [ObservableProperty]
        string style;
        async partial void OnSelectedChanged(bool oldValue, bool newValue)
        {
            Style=await APIService.Get();
        }

        public HttpClient HttpClient;
        public IAPIService APIService { get; set; }
        
        public ApiManager(string name, IAPIService service)
        {
            APIService = service;
            Name = name;
        }
        public ApiManager(string _name, string client)
        {
            Name = _name;
            HttpClient= new HttpClient() { BaseAddress=new Uri(client)};
            APIService = new APIService(HttpClient);
        }
        public async Task Initialize()
        {
            Style = await APIService.Get();
        }
    }
}
