using BadgerClan.Mobile.ModelView;
using BadgerClan.Mobile.Services;

namespace BadgerClan.Mobile
{
    public partial class MainPage : ContentPage
    {
        IAPIService api;
        MainPageModelView modelView;
        public MainPage(IAPIService api)
        {
            this.api = api;

            modelView = new MainPageModelView(api);
            BindingContext = modelView;
            InitializeComponent();
            Loaded += initialize;
        }
        async void initialize(object sender, EventArgs args)
        {
            modelView.Style = await api.Get();
        }
    }

}
