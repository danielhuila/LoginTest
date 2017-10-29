using LoginTest.Models;
using LoginTest.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace LoginTest.ViewModels
{
    public class LoginPageViewModel: ViewModelBase
    {

        #region Commands
        public INavigation Navigation { get; set; }
        public ICommand LoginCommand { get; set; }
        #endregion

        #region Properties
        private Users _user = new Users();

        private LoginUser _loginUser = new LoginUser();

        public Users User
        {
            get { return _user; }
            set { SetProperty(ref _user, value); }
        }

        private string _message;

        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }
        #endregion

        public LoginPageViewModel()
        {
            LoginCommand = new Command(Login);
        }

        /// <summary>
        /// Función de Login de la App
        /// </summary>
        public async void Login()
        {
            IsBusy = true;
            Title = string.Empty;

            try
            {

                if (User.UserName == null)
                {
                    IsBusy = false;
                    Message = "El Usuario es requerido";
                    return;
                }

                if (User.Password == null)
                {
                    IsBusy = false;
                    Message = "La contraseña es requerida";
                    return;
                }

                using (HttpClient clientHttp = new HttpClient())
                {
                    string url = "https://serveless.proximateapps-services.com.mx/catalog/dev/webadmin/authentication/login";

                    //Es necesaria para EasyTeable esta DefaultRequestHeaders
                    //clientHttp.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
                    string UserData = "{correo:" + User.UserName + ",contrasenia:" + User.Password + "}";
                    StringContent body = new StringContent(UserData, Encoding.UTF8, "application/json");
                    //Recibe nuestra Url y el Body a hacer Post osea Registar
                    var result = await clientHttp.PostAsync(url, body);

                    //Leemos el resultado tal como fue guardado.
                    string data = await result.Content.ReadAsStringAsync();

                    if (result.IsSuccessStatusCode)
                    {
                        //Deserializamos el objeto data para obtener el id serializado guardado
                        _loginUser = JsonConvert.DeserializeObject<LoginUser>(data);

                        if (_loginUser.success == "false")
                        {
                            IsBusy = false;
                            await App.Current.MainPage.DisplayAlert("Error", _loginUser.message, "Ok");
                            return;
                        }

                    }
                    else
                    {
                        IsBusy = false;
                        await App.Current.MainPage.DisplayAlert("Error","Error en la Conexión con el Servidor", "Ok");
                        return;
                    }
                   
                }

                await Navigation.PushAsync(new MainPage(_loginUser.token));
                          
            }
            catch (Exception e)
            {
                IsBusy = false;
                await App.Current.MainPage.DisplayAlert("Error de conexión", e.Message, "Ok");
            }
        }
    }
}
