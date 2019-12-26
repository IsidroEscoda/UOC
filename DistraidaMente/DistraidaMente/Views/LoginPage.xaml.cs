using DistraidaMente.Controllers;
using DistraidaMente.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DistraidaMente.Views
{
    public delegate bool ReadCodeHandler(string code);
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private Configuration _configuration;
        FirebaseHelper firebaseHelper = new FirebaseHelper();
        public event ReadCodeHandler ReadCode;
        public LoginPage()
        {
            _configuration = new Configuration();
            InitializeComponent();

			codeButton.Clicked += OnLoginButtonClicked;
		}

        public LoginPage(Configuration configuration)
        {
            _configuration = configuration;
            InitializeComponent();

            codeButton.Clicked += OnLoginButtonClicked;
        }

        async void OnLoginButtonClicked(object sender, EventArgs e)
        {

            var isValid = false;
            var code_Input = codeEntry.Text;
            var name_Input = nameEntry.Text;
            var strinDOC = "DOC001P1";
            if (code_Input == null)
            {
                DisplayAlert("Cuidado", "Entra un código válido o consulte con su médico", "ok");
            }
            else if (name_Input == null)
            {
                DisplayAlert("Falta un campo", "Rellena el campo con un nombre de referencia.", "Rellenar");
            }
            else
            {
                /*string pattern = @"^[a-zA-Z]{3}\d{3}$";

                if (Regex.IsMatch(code_Input, pattern, RegexOptions.IgnoreCase))
                {
                    _configuration.SaveProperty("name", name_Input);
                    _configuration.SaveProperty("docId", code_Input);
                    CheckCode(codeEntry.Text);

                    codeEntry.Text = string.Empty;
                    nameEntry.Text = string.Empty;
                    isValid = true;
                }
                else
                {
                    DisplayAlert("Cuidado", "Entra un código válido o consulte con su médico", "ok");
                }*/
                await firebaseHelper.AddPerson(Convert.ToInt32(codeEntry.Text.Substring(7)), nameEntry.Text.ToUpper(), codeEntry.Text.ToUpper(), false);

                _configuration.SaveProperty("name", name_Input);
                _configuration.SaveProperty("docId", code_Input);
                CheckCode(codeEntry.Text);

                codeEntry.Text = string.Empty;
                nameEntry.Text = string.Empty;

            }
		}

        private void CheckCode(string text)
        {
            codeEntry.Unfocus();

            if (!ReadCode?.Invoke(text) ?? true)
            {
                DisplayAlert("Código erroneo", "El código que has introducido no es válido", "OK");
            }
            else
            {
                //await firebaseHelper.AddPerson(Convert.ToInt32(codeEntry.Text.Substring(7)), nameEntry.Text.ToUpper());
                //await DisplayAlert("Success", "Person Added Successfully to FireBase", "OK");
            }
        }

    }
}