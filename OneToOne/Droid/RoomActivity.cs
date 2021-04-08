using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using DT.Samples.Opentok.Shared;
using DT.Samples.Opentok.Shared.Helpers;

namespace DT.Samples.Opentok.OneToOne.Droid
{
    [Activity(Label = "Room", Theme = "@style/DT.Theme.Room", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class RoomActivity : Activity
    {
        protected const int MaxLocalVideoDimension = 150;
        private bool _isVideoEnabled = true;
        private OpenTokStreamingService _opentokService;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Room);
            FindViewById<TextView>(Resource.Id.room_name).Text = OpentokSettings.Current.RoomName;
            _opentokService = OpenTokStreamingService.Instance;
            StartSessionAsync().NoWait();
        }

        public async Task StartSessionAsync()
        {
            var sessionId = OpentokTestConstants.GenerateSessionAndTokenWithServer ? await OpentokSessionHelper.RequestDataFromApiAsync(OpentokSessionHelper.SessionRequestURI, OpentokSettings.Current.RoomName) : OpentokTestConstants.SessionId;
            var token = OpentokTestConstants.GenerateSessionAndTokenWithServer ? await OpentokSessionHelper.RequestDataFromApiAsync(OpentokSessionHelper.TokenRequestURI, sessionId) : OpentokTestConstants.Token;
            FindViewById<RelativeLayout>(Resource.Id.loading_layer).Visibility = ViewStates.Gone;
            FindViewById<ProgressBar>(Resource.Id.loading_indicator).Visibility = ViewStates.Gone;
            _opentokService.SetStreamContainer(FindViewById<FrameLayout>(Resource.Id.local_video_view_container), true);
            _opentokService.SetStreamContainer(FindViewById<FrameLayout>(Resource.Id.remote_video_view_container), false);
            _opentokService.InitNewSession(OpentokTestConstants.OpentokAPIKey, sessionId, token);
            this.Window.AddFlags(WindowManagerFlags.KeepScreenOn);
            var self = this;
            _opentokService.OnSessionEnded += () =>
            {
                Finish();
                self.Window.ClearFlags(WindowManagerFlags.KeepScreenOn);
            };
            _opentokService.OnPublishStarted += () =>
            {
                FindViewById(Resource.Id.local_video_container).Visibility = _isVideoEnabled ? ViewStates.Visible : ViewStates.Gone;
                FindViewById(Resource.Id.local_video_view_container).Visibility = _isVideoEnabled ? ViewStates.Visible : ViewStates.Gone;
                FindViewById(Resource.Id.local_video_overlay).Visibility = _isVideoEnabled && !_opentokService.IsAudioPublishingEnabled ? ViewStates.Visible : ViewStates.Gone;
            };
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        [Java.Interop.Export("OnLocalVideoMuteClicked")]
        public void OnLocalVideoMuteClicked(View view)
        {
            ImageView iv = (ImageView)view;
            if (iv.Selected)
            {
                iv.Selected = false;
                iv.SetImageResource(Resource.Drawable.ic_cam_active_call);
            }
            else
            {
                iv.Selected = true;
                iv.SetImageResource(Resource.Drawable.ic_cam_disabled_call);
            }
            _opentokService.IsVideoPublishingEnabled = !iv.Selected;
            _isVideoEnabled = !iv.Selected;
            FindViewById<FrameLayout>(Resource.Id.local_video_view_container).GetChildAt(0).Visibility = _isVideoEnabled ? ViewStates.Visible : ViewStates.Gone;
            FindViewById(Resource.Id.local_video_container).Visibility = _isVideoEnabled ? ViewStates.Visible : ViewStates.Gone;
            FindViewById(Resource.Id.local_video_view_container).Visibility = _isVideoEnabled ? ViewStates.Visible : ViewStates.Gone;
            FindViewById(Resource.Id.local_video_overlay).Visibility = _isVideoEnabled && !_opentokService.IsAudioPublishingEnabled ? ViewStates.Visible : ViewStates.Gone;
        }

        [Java.Interop.Export("OnLocalAudioMuteClicked")]
        public void OnLocalAudioMuteClicked(View view)
        {
            ImageView iv = (ImageView)view;
            if (iv.Selected)
            {
                iv.Selected = false;
                iv.SetImageResource(Resource.Drawable.ic_mic_active_call);
            }
            else
            {
                iv.Selected = true;
                iv.SetImageResource(Resource.Drawable.ic_mic_inactive_call);
            }
            _opentokService.IsAudioPublishingEnabled = !iv.Selected;
            var visibleMutedLayers = iv.Selected ? ViewStates.Visible : ViewStates.Invisible;
            FindViewById(Resource.Id.local_video_overlay).Visibility = visibleMutedLayers;
            FindViewById(Resource.Id.local_video_muted).Visibility = visibleMutedLayers;
        }

        [Java.Interop.Export("OnSwitchCameraClicked")]
        public void OnSwitchCameraClicked(View view)
        {
            _opentokService.SwapCamera();
        }

        [Java.Interop.Export("OnEncCallClicked")]
        public void OnEncCallClicked(View view)
        {
            _opentokService.EndSession();
        }
    }
}

