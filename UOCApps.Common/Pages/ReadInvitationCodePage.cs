using DeltaApps.CommonLibrary.Controls;
using DeltaApps.CommonLibrary.Helpers;
using DeltaApps.PositiveApps.Common.Model;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DeltaApps.PositiveApps.Common.Pages
{
    public delegate bool ReadCodeHandler(string code);

    public class ReadInvitationCodePage : BasePage
    {

        public event ReadCodeHandler ReadCode;

        private Dialog _dialog;

        private Entry _codeEntry, _codeEntry2;

        private Configuration _configuration;
        public ReadInvitationCodePage(Configuration configuration) : base(configuration)
        {
            ShowNavigationBarOnEntry = false;
            _configuration = new DeltaApps.PositiveApps.Common.Model.Configuration();
        }

        protected override Layout SetupContentLayout()
        {
            BackgroundColor = Configuration.Theme.BackgroundColor;

            Grid mainLayout = new Grid();

            StackLayout contentLayout = new StackLayout()
            {
                Padding = new Thickness(20, 100),
                VerticalOptions = LayoutOptions.Start,
            };

            //contentLayout.Children.Add(new Label()
            //{
            //    Text = "Escanea el código QR de tu invitación",
            //    FontSize = Configuration.Theme.BigFontSize,
            //    TextColor = Configuration.Theme.TextColor,
            //    HorizontalTextAlignment = TextAlignment.Center,
            //});

            //contentLayout.Children.Add(new Image { Source = "logo__new.png" });

            contentLayout.Children.Add(new Label()
            {
                Text = "Introduce el código que se te ha enviado por email y escribe tu nombre de usuario",
                FontSize = Configuration.Theme.SmallFontSize,
                TextColor = Configuration.Theme.TextColor,
                VerticalTextAlignment = TextAlignment.Start,
                HorizontalTextAlignment = TextAlignment.Center,
            });

            Grid introduceCodeLayout = new Grid()
            {
                Padding = new Thickness(10),
                Margin = new Thickness(10, 5, 10, 5),
            };

            introduceCodeLayout.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            introduceCodeLayout.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10, GridUnitType.Absolute) });
            introduceCodeLayout.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            _codeEntry = new Entry()
            {
                //BackgroundColor = Configuration.Theme.SecondaryBackgroundColor,
                FontSize = Configuration.Theme.MediumFontSize,
                TextColor = Configuration.Theme.TextColor,
            };

            introduceCodeLayout.Children.Add(_codeEntry, 0, 0);

            introduceCodeLayout.Children.Add(FormsHelper.ConfigureImageButton("settings.png", (e, s) => { return; }, new Size(4, 4), true, Assembly.GetCallingAssembly()), 2, 0);

            contentLayout.Children.Add(introduceCodeLayout);

            Grid introduceCodeLayout2 = new Grid()
            {
                Margin = new Thickness(10,5,10,5),
                Padding = new Thickness(10),
            };

            introduceCodeLayout2.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            introduceCodeLayout2.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10, GridUnitType.Absolute) });
            introduceCodeLayout2.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            _codeEntry2 = new Entry()
            {
                //BackgroundColor = Configuration.Theme.SecondaryBackgroundColor,
                FontSize = Configuration.Theme.MediumFontSize,
                TextColor = Configuration.Theme.TextColor,
            };

            introduceCodeLayout2.Children.Add(_codeEntry2, 0, 0);

            introduceCodeLayout2.Children.Add(FormsHelper.ConfigureImageButton("person.png", (e, s) => { return; }, new Size(4, 4), true, Assembly.GetCallingAssembly()), 2, 0);

            contentLayout.Children.Add(introduceCodeLayout2);

            EventHandler scanButtonAction = (sender, e) =>
            {
                /*
                 var emailPattern = @"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$";
                if (Regex.IsMatch(email, emailPattern))
                {
                    ErrorLabel.Text = "Email is valid";
                }
                else
                {
                    ErrorLabel.Text = "EMail is InValid";
                }
                 */



                    var code_Input = _codeEntry.Text;
                    var name_Input = _codeEntry2.Text;
                    if (code_Input == null)
                    {
                        DisplayAlert("Cuidado", "Entra un código válido o consulte con su médico", "ok");
                    }
                    else if (name_Input == null)
                    {
                        DisplayAlert("Falta un campo", "Rellena el campo con un nombre de referencia.", "Rellenar");
                        /*var answer = await DisplayAlert("Exit", "Do you wan't to exit the App?", "Yes", "No");
                        if(answer)
                        {
                            _canClose = false;
                            OnBackButtonPressed;
                        }*/
                    }

                    else
                {
                        _configuration.SaveProperty("name", name_Input);
                        _configuration.SaveProperty("docId", code_Input);
                        CheckCode(_codeEntry.Text); 
                        _codeEntry.Text = string.Empty;
                        _codeEntry2.Text = string.Empty;
                    } 
                

                /*if (!string.IsNullOrWhiteSpace(_codeEntry.Text) && !string.IsNullOrWhiteSpace(_codeEntry2.Text))
                {
                    firebaseHelper.AddPerson(Convert.ToInt32(_codeEntry.Text.Substring(5)), _codeEntry2.Text, _codeEntry.Text);
                    DisplayAlert("Success", "Person Added Successfully", "OK");
                    var allPersons =  firebaseHelper.GetAllPersons();
                    //lstPersons.ItemsSource = allPersons;
                    CheckCode(_codeEntry.Text); 
                    _codeEntry.Text = string.Empty;
                    _codeEntry2.Text = string.Empty;
                }
                else
                {
                    DisplayAlert("Alert", "Consulta a tu médico para que te de un código o rellena el campo de nombre.", "OK");
                }*/
            };

            var scanButton = new FrameButton("Acceder", scanButtonAction)
            {
                Margin = new Thickness(40),
                BackgroundColor = Configuration.Theme.SelectedBackgroundColor,
                FontSize = Configuration.Theme.SmallFontSize,
                TextColor = Color.White,
            };

            contentLayout.Children.Add(scanButton);

            mainLayout.Children.Add(contentLayout);

            _dialog = new Dialog(new Size(300, 300))
            {
                Content = new Label()
                {
                    Text = "Código incorrecto. Vuelva a intentarlo o comuníquese con el organizador.",
                    FontSize = Configuration.Theme.MediumFontSize,
                    TextColor = Configuration.Theme.TextColor,
                    VerticalTextAlignment = TextAlignment.Center,
                    HorizontalTextAlignment = TextAlignment.Center,
                    LineBreakMode = LineBreakMode.WordWrap,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    MaxLines = 10,
                },
                DialogBackgroundColor = Configuration.Theme.SecondaryBackgroundColor,
                DialogSize = new Size(300, 300),
                DialogHeight = 300,
            };

            mainLayout.Children.Add(_dialog);

            return mainLayout;
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();

            await Task.Delay(1000).ContinueWith((t) => { Device.BeginInvokeOnMainThread(()=> _codeEntry.Focus()); });
        }

        private void CheckCode(string text)
        {
            _codeEntry.Unfocus();

            if (!ReadCode?.Invoke(text) ?? true)
            {
                _dialog.Show();
            }
        }
    }
}
