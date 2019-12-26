using DistraidaMente.Common.Helpers;
using DistraidaMente.Models;
using Octane.Xamarin.Forms.VideoPlayer;
using Octane.Xamarin.Forms.VideoPlayer.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Xamarin.Forms;

namespace DistraidaMente.Common.Pages
{
    public class VideoPage : ContentPage
    {
        private string videoSource;
        private VideoType videoType;
        private Action returnAction;
        private Assembly resourceAssembly;
        private bool displayControls;
        private bool enableClose;

        private VideoPlayer videoPlayer;

        public VideoPage(string videoSource, VideoType videoType, Action returnAction, Assembly resourceAssembly = null, bool displayControls = false, bool enableClose = true)
        {
            this.videoSource = videoSource;
            this.videoType = videoType;
            this.returnAction = returnAction;
            this.resourceAssembly = resourceAssembly;
            this.displayControls = displayControls;
            this.enableClose = enableClose;
        }

        protected override void OnAppearing()
        {
            try
            {
                IsBusy = true;

                base.OnAppearing();

                MessagingCenter.Send(this, "allowLandScapePortrait");

                NavigationPage.SetHasNavigationBar(this, false);

                AbsoluteLayout pageLayout = new AbsoluteLayout()
                {
                    BackgroundColor = Color.Black,
                };

                videoPlayer = new VideoPlayer
                {
                    AutoPlay = true,
                    BackgroundColor = Color.Black,
                    FillMode = FillMode.ResizeAspect,
                };

                videoPlayer.Failed += Vp_Failed;
                videoPlayer.Completed += Vp_Completed;

                videoPlayer.Source = VideoSourceFromType(videoSource, videoType);

                videoPlayer.AnchorX = 0.5;
                videoPlayer.AnchorY = 0.5;

                videoPlayer.DisplayControls = displayControls;

                pageLayout.Children.Add(videoPlayer, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);

                if (enableClose)
                {
                    var closeButton = FormsHelper.ConfigureImageButton("DeltaApps.CommonLibrary.Images.closevideo.png", (s, e) => { Close(); }, new Size(32, 32), false);

                    closeButton.AnchorX = 0.5;
                    closeButton.AnchorY = 0.5;

                    pageLayout.Children.Add(closeButton, new Rectangle(0.95, 0, 32, 32), AbsoluteLayoutFlags.XProportional);
                }

                Content = pageLayout;

                IsBusy = false;
            }
            catch
            {
                Close();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            MessagingCenter.Send(this, "preventLandScape");
        }

        private void Vp_Completed(object sender, Octane.Xamarin.Forms.VideoPlayer.Events.VideoPlayerEventArgs e)
        {
            Close();
        }

        private void Vp_Failed(object sender, Octane.Xamarin.Forms.VideoPlayer.Events.VideoPlayerErrorEventArgs e)
        {
            DisplayAlert("Error", "Error de video.", "OK");

            Close();
        }

        protected override bool OnBackButtonPressed()
        {
            Close();

            return true;
        }

        private void Close()
        {
            Task.Delay(100).ContinueWith((t) =>
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => returnAction?.Invoke());
            });
        }

        public static bool CheckVideoAvailability(string videoSource, VideoType videoType)
        {
            bool available = false;

            if (videoType == VideoType.YouTube)
            {
                available = CheckYouTubeVideoAvailability(videoSource);
            }
            else if (videoType == VideoType.Url)
            {
                available = CheckUriVideoAvailability(videoSource);
            }
            else if (videoType == VideoType.Resource)
            {
                available = CheckResourceVideoAvailability(videoSource);
            }
            else if (videoType == VideoType.File)
            {
                available = CheckFileVideoAvailability(videoSource);
            }

            return available;
        }

        private static bool CheckResourceVideoAvailability(string videoSource)
        {
            return true;
        }

        private static bool CheckFileVideoAvailability(string videoSource)
        {
            return true;
        }

        private static bool CheckUriVideoAvailability(string videoSource)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(3);

                var videoPageContent = "";

                try
                {
                    videoPageContent = client.GetStringAsync(videoSource).Result;
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CheckYouTubeVideoAvailability(string videoId)
        {
            var videoInfoUrl = $"http://www.youtube.com/get_video_info?video_id={videoId}";

            return CheckUriVideoAvailability(videoInfoUrl);
        }

        private VideoSource VideoSourceFromType(string videoSource, VideoType videoType)
        {
            VideoSource source = null;

            if (videoType == VideoType.YouTube)
            {
                source = YouTubeVideoSourceConstructor(videoSource);
            }
            else if (videoType == VideoType.Url)
            {
                source = VideoSource.FromUri(videoSource);
            }
            else if (videoType == VideoType.Resource)
            {
                source = VideoSource.FromResource(videoSource, resourceAssembly);
            }
            else if (videoType == VideoType.File)
            {
                source = VideoSource.FromFile(videoSource);
            }

            return source;
        }

        public VideoSource YouTubeVideoSourceConstructor(string videoId)
        {
            var videoInfoUrl = $"http://www.youtube.com/get_video_info?video_id={videoId}";

            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(3);

                var videoPageContent = client.GetStringAsync(videoInfoUrl).Result;
                var videoParameters = HttpUtility.ParseQueryString(videoPageContent);
                var encodedStreamsDelimited = WebUtility.HtmlDecode(videoParameters["url_encoded_fmt_stream_map"]);
                var encodedStreams = encodedStreamsDelimited.Split(',');
                var streams = encodedStreams.Select(HttpUtility.ParseQueryString);

                var orderedStreams = streams
                    .OrderBy(s =>
                    {
                        var type = s["type"];
                        if (type.Contains("video/mp4")) return 10;
                        if (type.Contains("video/3gpp")) return 20;
                        if (type.Contains("video/x-flv")) return 30;
                        if (type.Contains("video/webm")) return 40;
                        return int.MaxValue;
                    })
                    .ThenBy(s =>
                    {
                        var quality = s["quality"];

                        int i = 0;

                        switch (Device.Idiom)
                        {
                            case TargetIdiom.Phone:
                                i = Array.IndexOf(new[] { "hd720", "medium", "high", "small" }, quality);
                                break;
                            default:
                                i = Array.IndexOf(new[] { "high", "medium", "small" }, quality);
                                break;
                        }

                        return i; // i >= 0 ? i : Int32.MaxValue;
                    });


                var streamsByPriority = orderedStreams.FirstOrDefault();

                return VideoSource.FromUri(streamsByPriority["url"]);
            }
        }
    }
}