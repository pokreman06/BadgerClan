using System;
using System.Collections.Generic;
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
    }
}
