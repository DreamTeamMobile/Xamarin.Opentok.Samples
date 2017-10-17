using System;
using System.Threading.Tasks;
using DT.Samples.Opentok.Shared;
using DT.Samples.Opentok.Shared.Helpers;
using Foundation;
using UIKit;

namespace DT.Samples.Opentok.OneToOne.iOS
{
    public partial class RoomViewController : UIViewController
    {
        private OpentokStreamingService _opentokService;

        private bool _audioMuted = false;
        private bool _videoMuted = false;

        public bool AudioMuted
        {
            get
            {
                return _audioMuted;
            }
            set
            {
                _audioMuted = value;
                if (ToggleAudioButton != null)
                {
                    ToggleAudioButton.Selected = value;
                    _opentokService.IsAudioPublishingEnabled = !value;
                    UpdateMutedViewVisibility();
                }
            }
        }

        public bool VideoMuted
        {
            get
            {
                return _videoMuted;
            }
            set
            {
                _videoMuted = value;
                if (ToggleCamButton != null)
                {
                    ToggleCamButton.Selected = value;
                    LocalView.Hidden = value;
                    SwitchCamButton.Hidden = value;
                    _opentokService.IsVideoPublishingEnabled = !value;
                    UpdateMutedViewVisibility();
                }
            }
        }

        protected RoomViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            NavigationController.NavigationBarHidden = true;
            RoomNameLabel.Text = OpentokSettings.Current.RoomName;
            BackgroundTap.ShouldRequireFailureOfGestureRecognizer(BackgroundDoubleTap);
            BusyIndicatorView.StartAnimating();
            _opentokService = OpentokStreamingService.Instance;
            StartSession();
            _opentokService.OnPublishStarted += () => { VideoMuted = false; };
            _opentokService.OnSessionEnded += () => { LeaveChannel(); };
        }

        public async Task StartSession()
        {
            var sessionId = await OpentokSessionHelper.Request(OpentokSessionHelper.SessionRequestURI, OpentokSettings.Current.RoomName);
            var token = await OpentokSessionHelper.Request(OpentokSessionHelper.TokenRequestURI, sessionId);
            BusyIndicatorView.Hidden = BusyViewLayer.Hidden = BusyTextView.Hidden = true;
            ContainerView.Hidden = LocalView.Hidden = false;
            _opentokService.SetStreamContainer(ContainerView, false);
            _opentokService.SetStreamContainer(LocalView, true);
            _opentokService.InitNewSession(OpentokTestConstants.OpentokAPI, sessionId, token);
            UIApplication.SharedApplication.IdleTimerDisabled = true;
        }

        partial void ToggleAudioButtonClicked(NSObject sender)
        {
            AudioMuted = !AudioMuted;
        }

        partial void ToggleCamClicked(NSObject sender)
        {
            VideoMuted = !VideoMuted;
        }

        partial void SwitchCamClicked(NSObject sender)
        {
            _opentokService.SwapCamera();
        }

        public void LeaveChannel(bool frombutton = false)
        {
            if (frombutton)
                _opentokService.EndSession();
            NavigationController.NavigationBarHidden = false;
            NavigationController.PopViewController(true);
        }

        partial void EndCallClicked(NSObject sender)
        {
            LeaveChannel(true);
        }

        private void UpdateMutedViewVisibility()
        {
            if (!VideoMuted && AudioMuted)
            {
                MutedView.Hidden = false;
            }
            else
            {
                MutedView.Hidden = true;
            }
        }
    }
}
