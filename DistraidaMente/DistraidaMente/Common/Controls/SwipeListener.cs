using System;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Text;

namespace DistraidaMente.Controls
{
    public class SwipeListener : PanGestureRecognizer
    {
        private ISwipeCallBack mISwipeCallback;
        private double translatedX = 0, translatedY = 0;

        public SwipeListener(View view, ISwipeCallBack iSwipeCallBack)
        {
            mISwipeCallback = iSwipeCallBack;
            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += OnPanUpdated;
            view.GestureRecognizers.Add(panGesture);
        }

        void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {

            View Content = (View)sender;

            switch (e.StatusType)
            {
                case GestureStatus.Started:

                    translatedX = 0;
                    translatedY = 0;
                    break;

                case GestureStatus.Running:

                    try
                    {
                        translatedX = e.TotalX;
                        translatedY = e.TotalY;
                    }
                    catch (Exception err)
                    {
                        System.Diagnostics.Debug.WriteLine("" + err.Message);
                    }
                    break;

                case GestureStatus.Completed:

                    System.Diagnostics.Debug.WriteLine("translatedX : " + translatedX);
                    System.Diagnostics.Debug.WriteLine("translatedY : " + translatedY);

                    if (translatedX < 0 && Math.Abs(translatedX) > Math.Abs(translatedY))
                    {
                        mISwipeCallback.OnLeftSwipe(Content);
                    }
                    else if (translatedX > 0 && translatedX > Math.Abs(translatedY))
                    {
                        mISwipeCallback.OnRightSwipe(Content);
                    }
                    else if (translatedY < 0 && Math.Abs(translatedY) > Math.Abs(translatedX))
                    {
                        mISwipeCallback.OnTopSwipe(Content);
                    }
                    else if (translatedY > 0 && translatedY > Math.Abs(translatedX))
                    {
                        mISwipeCallback.OnBottomSwipe(Content);
                    }
                    else
                    {
                        mISwipeCallback.OnNothingSwiped(Content);
                    }

                    break;
            }
        }
    }
}
