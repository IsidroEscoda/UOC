using DistraidaMente.Helpers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xamarin.Forms;

namespace DistraidaMente.Common.Helpers
{
    public class FormsHelper
    {
        public static Image ConfigureImageButton(string image, EventHandler onTapHandler = null, Size? size = null, bool fromFile = true, Assembly resourceAssembly = null)
        {
            Image imageButton = new Image()
            {
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };

            if (size != null)
            {
                imageButton.WidthRequest = size.Value.Width;
                imageButton.HeightRequest = size.Value.Height;
            }

            if (fromFile)
            {
                imageButton.Source = ImageSource.FromFile(image);
            }
            else
            {
                resourceAssembly = resourceAssembly ?? Assembly.GetCallingAssembly();

                imageButton.Source = ImageSource.FromResource(image, resourceAssembly);
            }

            if (onTapHandler != null)
            {
                imageButton.AddTapRecognizer(onTapHandler);
            }

            return imageButton;
        }
    }
}

