using LoginTest.Helpers;
using LoginTest.Models;
using LoginTest.Views;
using Newtonsoft.Json;
using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace LoginTest.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        #region "Variables"
        public INavigation Navigation { get; set; }
        public ICommand SaveUserCommand { get; set; }
        public ICommand LogoutCommand { get; set; }
        public ICommand TakePictureCommand { get; set; }
        private string _message;
        private string _latitud;
        private string _longitud;
        private ImageSource _photoUser;

        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public string Latitud
        {
            get { return _latitud; }
            set { SetProperty(ref _latitud, value); }
        }

        public string Longitud
        {
            get { return _longitud; }
            set { SetProperty(ref _longitud, value); }
        }

        List<Users> _listUsers = new List<Users>();

        public List<Users> ListUsers
        {
            get { return _listUsers; }
            set { SetProperty(ref _listUsers, value); }
        }

        public ImageSource PhotoUser
        {
            get { return _photoUser; }
            set { SetProperty(ref _photoUser, value); }
        }

        private string _storageDirectory;
        public string StorageDirectory
        {
            get { return _storageDirectory; }
            set { SetProperty(ref _storageDirectory, value); }
        }

        private ReturnLogin _returnLogin = new ReturnLogin();

        private Users objUser = new Users();

        //Datos de Usuario
        private string _nameUser;
        public string NameUser
        {
            get { return _nameUser; }
            set { SetProperty(ref _nameUser, value); }
        }

        private string _lastNameUser;
        public string LastNameUser
        {
            get { return _lastNameUser; }
            set { SetProperty(ref _lastNameUser, value); }
        }

        private string _identificationType;
        public string IdentificationType
        {
            get { return _identificationType; }
            set { SetProperty(ref _identificationType, value); }
        }

        private string _identificationNumber;
        public string IdentificationNumber
        {
            get { return _identificationNumber; }
            set { SetProperty(ref _identificationNumber, value); }
        }

        private string _emailUser;
        public string EmailUser
        {
            get { return _emailUser; }
            set { SetProperty(ref _emailUser, value); }
        }

        #endregion

        public MainPageViewModel(string TokenUser)
        {
            LogoutCommand = new Command(Logout);
            TakePictureCommand = new Command(TakePicture);
            SaveUserCommand = new Command(SaveUser);
            Message = "Bienvenido!" ;

            //Conecto con los servicios del WebService
            DataUser(TokenUser);

        }

        /// <summary>
        /// Método para obtener los datos de usuario
        /// </summary>
        /// <param name="token">Token obtenido en el login</param>
        private async void DataUser(string token)
        {
            using (HttpClient clientHttp = new HttpClient())
            {
                string url = "https://serveless.proximateapps-services.com.mx/catalog/dev/webadmin/users/getdatausersession";

                //Es necesaria para EasyTeable esta DefaultRequestHeaders
                clientHttp.DefaultRequestHeaders.Clear();
                clientHttp.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                clientHttp.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("utf-8"));
                clientHttp.DefaultRequestHeaders.Add("Authorization", token);

               
                string UserData = "{Authorization:" + token +"}";
                StringContent body = new StringContent(UserData, Encoding.UTF8, "application/json");
               
                var result = await clientHttp.PostAsync(url, body);

                //Leemos el resultado tal como fue guardado.
                string data = await result.Content.ReadAsStringAsync();

                if (result.IsSuccessStatusCode)
                {
                    //Deserializamos el objeto data para obtener el id serializado guardado
                    _returnLogin = JsonConvert.DeserializeObject<ReturnLogin>(data);

                    if (_returnLogin.success == "false")
                    {
                        IsBusy = false;
                        await App.Current.MainPage.DisplayAlert("Error", _returnLogin.message, "Ok");
                        return;
                    }

                    AssignValues();

                }
                else
                {
                    IsBusy = false;
                    await App.Current.MainPage.DisplayAlert("Error", "Error en la Conexión con el Servidor", "Ok");
                    return;
                }

            }
        }

        /// <summary>
        /// Método para salir
        /// </summary>
        private async void Logout()
        {

            Settings.IsLoggedIn = false;
            await Navigation.PushAsync(new LoginPage());
        }

        /// <summary>
        /// Método para tomar la foto. Se activa con el botón de cámara
        /// </summary>
        private async void TakePicture()
        {
            var photo = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions() { });

            if (photo != null)
            {
                StorageDirectory = photo.AlbumPath;
                IsBusy = true;
                PhotoUser = ImageSource.FromStream(() => { return photo.GetStream(); });
                LocationPhoto();
            }
            else
            {
                IsBusy = false;
            }

        }

        /// <summary>
        /// Método para obtener la Geolocalización y localización de la foto en el dispositivo
        /// </summary>
        private async void LocationPhoto()
        {
            try
            {
                TimeSpan TimeSearch = new TimeSpan(0, 0, 30);
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 50; // Precisión deseada (en metros)
                var position = await locator.GetPositionAsync(TimeSearch);
                if (position != null){
                    Latitud = position.Latitude.ToString();
                    Longitud = position.Longitude.ToString();

                    //Almaceno en las variables de Usuario los datos correspondientes a la imagen
                    objUser.Latitude = Latitud;
                    objUser.StorageUbication = StorageDirectory.ToString();
                    objUser.Longitud = Longitud;
                }

            }
            catch (Exception e)
            {
                IsBusy = false;
                await App.Current.MainPage.DisplayAlert("Ocurrió un error al momento de buscar la geolocalización", e.Message, "Ok");
            }
           
        }

        
        /// <summary>
        /// Método para Asignar Valores y crear el objeto a guardar
        /// </summary>
        private void AssignValues()
        {
            IsBusy = true;
            if (_returnLogin.data != null && _returnLogin.data.Count > 0)
            {
                foreach (UserData objReturn in _returnLogin.data)
                {
                    NameUser = objReturn.nombres.ToString();
                    LastNameUser = objReturn.apellidos.ToString();
                    EmailUser = objReturn.correo.ToString();
                    IdentificationNumber = objReturn.numero_documento.ToString();
                    IdentificationType = objReturn.documentos_abrev.ToString();
                    
                    objUser.Name = objReturn.nombres.ToString();
                    objUser.LastName = objReturn.apellidos.ToString();
                    objUser.IdentificationType = objReturn.documentos_abrev.ToString();
                    objUser.IdentificationNumber = objReturn.numero_documento.ToString();
                    objUser.LastSession = objReturn.ultima_sesion;
                  
                }

                IsBusy = false;
            }
        }

        /// <summary>
        /// Método para Almacenar Usuario. Se activa con el botón guardar
        /// </summary>
        private async void SaveUser()
        {
            try
            {
                IsBusy = true;
                using (var datos = new DataAccess())
                {

                    Users tmpUser = new Users();
                    tmpUser = datos.GetUser(objUser.IdentificationNumber);

                    if (tmpUser.IdUser > 0)
                    {

                        //El usuario ya existe, entonces se actualiza
                        datos.updateUser(objUser);
                        await App.Current.MainPage.DisplayAlert("Actualizar", "El Usuario se actualizó correctamente", "Ok");
                        IsBusy = false;
                    }
                    else
                    {
                        // El usuario es nuevo, entonces lo inserto
                        datos.insertUser(objUser);
                        await App.Current.MainPage.DisplayAlert("Actualizar", "El Usuario se insertó correctamente", "Ok");
                        IsBusy = false;
                       
                    }

                }  

            }
            catch (Exception e)
            {
                IsBusy = false;
                await App.Current.MainPage.DisplayAlert("No se pudo guardar, se presentó un error", e.Message, "Ok");
            }
           
          
        }

    }
}
