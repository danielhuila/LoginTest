using LoginTest.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LoginTest
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        private MainPageViewModel viewModel;
        public MainPage(string TokenUser)
        {
            InitializeComponent();
            BindingContext = viewModel = new MainPageViewModel(TokenUser);
            viewModel.Navigation = this.Navigation;
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }

    }
}
