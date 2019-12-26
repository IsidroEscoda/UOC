using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace DistraidaMente.Helpers
{
    public static class ViewExtensions
    {
        public static void AddTapRecognizer(this View view, EventHandler onTapHandler)
        {
            var tapGestureRecognizer = new TapGestureRecognizer();

            tapGestureRecognizer.Tapped += onTapHandler;

            view.GestureRecognizers.Add(tapGestureRecognizer);
        }

        public static async Task AppearRight(this View view, uint duration)
        {
            view.IsVisible = false;

            await view.TranslateTo(DeviceDisplay.MainDisplayInfo.Width, 0, 0);

            view.IsVisible = true;

            await view.TranslateTo(0, 0, duration, Easing.CubicOut);
        }

        public static async Task DisappearRight(this View view, uint duration)
        {
            await view.TranslateTo(DeviceDisplay.MainDisplayInfo.Width, 0, duration, Easing.CubicIn);
        }

        public static async Task AppearLeft(this View view, uint duration)
        {
            view.IsVisible = false;

            await view.TranslateTo(-DeviceDisplay.MainDisplayInfo.Width, 0, 0);

            view.IsVisible = true;

            await view.TranslateTo(0, 0, duration, Easing.CubicOut);
        }

        public static async Task DisappearLeft(this View view, uint duration)
        {
            await view.TranslateTo(-DeviceDisplay.MainDisplayInfo.Width, 0, duration, Easing.CubicIn);
        }

        public static async Task AppearTop(this View view, uint duration)
        {
            view.IsVisible = false;

            await view.TranslateTo(0, -DeviceDisplay.MainDisplayInfo.Height * 1.1, 0);

            view.IsVisible = true;

            await view.TranslateTo(0, 0, duration, Easing.CubicOut);
        }

        public static async Task DisappearTop(this View view, uint duration)
        {
            await view.TranslateTo(0, -DeviceDisplay.MainDisplayInfo.Height * 1.1, duration, Easing.CubicIn);
        }

        public static async Task AppearBottom(this View view, uint duration)
        {
            view.IsVisible = false;

            await view.TranslateTo(0, DeviceDisplay.MainDisplayInfo.Height * 1.1, 0);

            view.IsVisible = true;

            await view.TranslateTo(0, 0, duration, Easing.CubicOut);
        }

        public static async Task DisappearBottom(this View view, uint duration)
        {
            await view.TranslateTo(0, DeviceDisplay.MainDisplayInfo.Height * 1.1, duration, Easing.CubicIn);
        }
    }
}
