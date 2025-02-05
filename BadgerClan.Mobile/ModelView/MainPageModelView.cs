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
    public partial class MainPageModelView(IAPIService api) : ObservableObject
    {
        [ObservableProperty]
        private string style;
        [ObservableProperty]
        string tempName;
        [ObservableProperty]
        string tempPath;
        [RelayCommand]
        public async Task Hold()
        {
            api.Change("Hold");
            Style = "Hold";
        }
        [RelayCommand]
        public async Task Attack()
        {
            api.Change("Attack");
            Style = "Attack!";
        }
        public async void UpdateStyle()
        {
            Style = await api.Get();
        }
        [RelayCommand]
        public void LogAPI()
        {
            ApiManagers.Add(new(api) { Name = TempName, HttpClient = new() { BaseAddress = new Uri(TempPath) } });
            TempName = "";
            TempPath = "";
        }

        public ObservableCollection<ApiManager> ApiManagers { get; set; }
    }
    public partial class ApiManager(IAPIService service) : ObservableObject
    {
        [ObservableProperty]
        string name;
        [RelayCommand]
        async Task Select()
        {
            service.changeClient(HttpClient);
        }

        public HttpClient HttpClient;

    }
}
