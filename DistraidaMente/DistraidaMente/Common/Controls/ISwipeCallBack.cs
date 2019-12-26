using Xamarin.Forms;

namespace DistraidaMente.Controls
{
    public interface ISwipeCallBack
    {
        void OnLeftSwipe(View view);
        void OnRightSwipe(View view);
        void OnTopSwipe(View view);
        void OnBottomSwipe(View view);
        void OnNothingSwiped(View view);
    }
}
