using System;
using AVFoundation;
using CoreGraphics;
using OpenTok;
using UIKit;

namespace DT.Samples.Opentok.OneToOne.iOS
{
    public class OpentokStreamingService
    {
        #region private state

        protected object _syncRoot = new object();
        protected string _apiKey;
        protected string _sessionId;
        protected string _userToken;
        protected bool _subscriberConnected;
        protected bool _sessionTerminationIsInProgress = false;

        #endregion

        public event Action OnSessionEnded = delegate { };
        public event Action OnPublishStarted = delegate { };

        #region singletone

        private static OpentokStreamingService _instance = new OpentokStreamingService();

        public static OpentokStreamingService Instance
        {
            get { return _instance; }
        }

        #endregion

        #region private state
        private OTSession _session;
        private OTPublisher _publisher;
        private OTSubscriber _subscriber;
        private UIView _myStreamContainer;
        private UIView _otherStreamContainer;
        private bool _lastKnownVideoPublishingState;

        #endregion

        private OpentokStreamingService()
        {
        }

        public void SetConfig(string apiKey, string sessionId, string userToken)
        {
            _apiKey = apiKey;
            _sessionId = sessionId;
            _userToken = userToken;
        }

        public void InitNewSession()
        {
            InitNewSession(_apiKey, _sessionId, _userToken);
        }

        public void InitNewSession(string apiKey, string sessionId, string userToken)
        {
            if (_session != null)
            {
                //stop running session if any and dispose all the resources
                EndSession();
            }

            _apiKey = apiKey;
            _sessionId = sessionId;
            _userToken = userToken;

            IsVideoPublishingEnabled = true;
            IsAudioPublishingEnabled = true;
            IsVideoSubscriptionEnabled = true;
            IsAudioSubscriptionEnabled = true;
            IsSubscriberVideoEnabled = true;

            _session = new OTSession(_apiKey, _sessionId, null);
            SubscribeForSessionEvents(_session);
            _session.Init();

            OTError error;
            _session.ConnectWithToken(_userToken, out error);
        }

        private void Publish()
        {
            lock (_syncRoot)
            {
                if (_publisher != null || _session == null)
                    return;

                OTError error;
                _publisher = new OTPublisher(null, new OTPublisherSettings
                {
                    Name = "XamarinOpenTok",
                    CameraFrameRate = OTCameraCaptureFrameRate.OTCameraCaptureFrameRate15FPS,
                    CameraResolution = OTCameraCaptureResolution.High,
                });
                SubscribeForPublisherEvents(_publisher, true);
                _session.Publish(_publisher, out error);

                ActivateStreamContainer(_myStreamContainer, _publisher.View);
            }
        }

        private void Subscribe(OTStream stream)
        {
            lock (_syncRoot)
            {
                if (_subscriber != null || _session == null)
                    return;

                OTError error;
                _subscriber = new OTSubscriber(stream, null);
                SubscribeForSubscriberEvents(_subscriber);
                _session.Subscribe(_subscriber, out error);
            }
        }

        public void EndSession()
        {
            lock (_syncRoot)
            {
                try
                {
                    _sessionTerminationIsInProgress = true;
                    if (_subscriber != null)
                    {
                        if (_subscriber.SubscribeToAudio)
                            _subscriber.SubscribeToAudio = false;
                        if (_subscriber.SubscribeToVideo)
                            _subscriber.SubscribeToVideo = false;
                        SubscribeForSubscriberEvents(_subscriber, false);
                        _subscriber.Dispose();
                        _subscriber = null;
                    }
                    if (_publisher != null)
                    {
                        if (_publisher.PublishAudio)
                            _publisher.PublishAudio = false;
                        if (_publisher.PublishVideo)
                            _publisher.PublishVideo = false;
                        SubscribeForPublisherEvents(_publisher, false);
                        _publisher.Dispose();
                        _publisher = null;
                    }

                    if (_session != null)
                    {
                        SubscribeForSessionEvents(_session, false);
                        _session.Disconnect();
                        _session.Dispose();
                        _session = null;
                    }

                    _myStreamContainer.InvokeOnMainThread(() =>
                    {
                        DeactivateStreamContainer(_myStreamContainer);
                        _myStreamContainer = null;

                        DeactivateStreamContainer(_otherStreamContainer);
                        _otherStreamContainer = null;
                    });

                    _apiKey = null;
                    _sessionId = null;
                    _userToken = null;

                    IsVideoPublishingEnabled = false;
                    IsAudioPublishingEnabled = false;
                    IsVideoSubscriptionEnabled = false;
                    IsAudioSubscriptionEnabled = false;
                    IsSubscriberVideoEnabled = false;

                    _subscriberConnected = false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    _sessionTerminationIsInProgress = false;
                    OnSessionEnded();
                }
            }
        }

        public void Dispose()
        {
            EndSession();
        }

        private bool _isVideoPublishingEnabled;

        public bool IsVideoPublishingEnabled
        {
            get { return _isVideoPublishingEnabled; }
            set
            {
                if (_isVideoPublishingEnabled == value)
                    return;
                _isVideoPublishingEnabled = value;
                OnVideoPublishingChanged();
            }
        }

        private bool _isAudioPublishingEnabled;

        public bool IsAudioPublishingEnabled
        {
            get
            {
                return _isAudioPublishingEnabled;
            }
            set
            {
                if (_isAudioPublishingEnabled == value)
                    return;
                _isAudioPublishingEnabled = value;
                OnAudioPublishingChanged();
            }
        }

        private bool _isVideoSubscriptionEnabled;

        public bool IsVideoSubscriptionEnabled
        {
            get { return _isVideoSubscriptionEnabled; }
            set
            {
                if (_isVideoSubscriptionEnabled == value)
                    return;
                _isVideoSubscriptionEnabled = value;
                OnVideoSubscriptionChanged();
            }
        }

        private bool _isAudioSubscriptionEnabled;

        public bool IsAudioSubscriptionEnabled
        {
            get
            {
                return _isAudioSubscriptionEnabled;
            }
            set
            {
                if (_isAudioSubscriptionEnabled == value)
                    return;
                _isAudioSubscriptionEnabled = value;
                OnAudioSubscriptionChanged();
            }
        }

        private bool _isSubscriberVideoEnabled;

        public bool IsSubscriberVideoEnabled
        {
            get { return _isSubscriberVideoEnabled; }
            protected set
            {
                if (_isSubscriberVideoEnabled == value)
                    return;
                _isSubscriberVideoEnabled = value;
                OnSubscriberVideoChanged();
            }
        }

        protected virtual void OnSubscriberVideoChanged()
        {
            //we have to keep and raise the enabled/disabled flag because Stream.HasVideo property doesn't reflect the change at once
        }

        public void SwapCamera()
        {
            if (_publisher == null)
                return;

            if (_publisher.CameraPosition != AVCaptureDevicePosition.Front)
                _publisher.CameraPosition = AVCaptureDevicePosition.Front;
            else
                _publisher.CameraPosition = AVCaptureDevicePosition.Back;
        }

        protected void OnVideoPublishingChanged()
        {
            if (_publisher == null)
                return;
            if (_publisher.PublishVideo != IsVideoPublishingEnabled)
                _publisher.PublishVideo = IsVideoPublishingEnabled;
        }

        protected void OnAudioPublishingChanged()
        {
            if (_publisher == null)
                return;
            if (_publisher.PublishAudio != IsAudioPublishingEnabled)
                _publisher.PublishAudio = IsAudioPublishingEnabled;
        }

        protected void OnVideoSubscriptionChanged()
        {
            if (_subscriber == null)
                return;
            if (_subscriber.SubscribeToVideo != IsVideoSubscriptionEnabled)
                _subscriber.SubscribeToVideo = IsVideoSubscriptionEnabled;
        }

        protected void OnAudioSubscriptionChanged()
        {
            if (_subscriber == null)
                return;
            if (_subscriber.SubscribeToAudio != IsAudioSubscriptionEnabled)
                _subscriber.SubscribeToAudio = IsAudioSubscriptionEnabled;
        }

        public void SetStreamContainer(object container, bool myStream)
        {
            var streamContainer = ((UIView)container);
            UIView streamView = null;

            if (myStream)
            {
                _myStreamContainer = streamContainer;
                if (_publisher != null)
                    streamView = _publisher.View;
            }
            else
            {
                _otherStreamContainer = streamContainer;
                if (_subscriber != null)
                    streamView = _subscriber.View;
            }

            ActivateStreamContainer(streamContainer, streamView);
        }

        #region private helpers

        private void ActivateStreamContainer(UIView streamContainer, UIView streamView)
        {
            DeactivateStreamContainer(streamContainer);
            if (streamContainer == null || streamView == null)
                return;

            if (streamView.Superview != null)
                streamView.RemoveFromSuperview();
            streamView.Frame = new CGRect(0, 0, streamContainer.Frame.Width, streamContainer.Frame.Height);
            streamContainer.Add(streamView);
        }

        private void DeactivateStreamContainer(UIView streamContainer)
        {
            if (streamContainer == null || streamContainer.Subviews == null)
                return;

            while (streamContainer.Subviews.Length > 0)
            {
                var view = streamContainer.Subviews[0];
                view.RemoveFromSuperview();
            }
        }

        #endregion

        #region events subscription

        private void SubscribeForSessionEvents(OTSession session, bool subscribe = true)
        {
            if (session == null)
                return;

            if (subscribe)
            {
                session.ConnectionDestroyed += OnConnectionDestroyed;
                session.DidConnect += OnDidConnect;
                session.StreamCreated += OnStreamCreated;
                session.StreamDestroyed += OnStreamDestroyed;
            }
            else
            {
                session.ConnectionDestroyed -= OnConnectionDestroyed;
                session.DidConnect -= OnDidConnect;
                session.StreamCreated -= OnStreamCreated;
                session.StreamDestroyed -= OnStreamDestroyed;
            }
        }

        private void SubscribeForSubscriberEvents(OTSubscriber subscriber, bool subscribe = true)
        {
            if (subscriber == null)
                return;

            if (subscribe)
            {
                subscriber.DidConnectToStream += OnSubscriberDidConnectToStream;
                subscriber.VideoDataReceived += OnSubscriberVideoDataReceived;
                subscriber.VideoEnabled += OnSubscriberVideoEnabled;
            }
            else
            {
                subscriber.DidConnectToStream -= OnSubscriberDidConnectToStream;
                subscriber.VideoDataReceived -= OnSubscriberVideoDataReceived;
                subscriber.VideoEnabled -= OnSubscriberVideoEnabled;
            }
        }

        private void SubscribeForPublisherEvents(OTPublisher publisher, bool subscribe = true)
        {
            if (publisher == null)
                return;

            if (subscribe)
            {
                publisher.StreamCreated += OnPublisherStreamCreated;
            }
            else
            {
                publisher.StreamCreated += OnPublisherStreamCreated;
            }
        }

        #endregion

        #region session events

        private void OnConnectionDestroyed(object sender, OTSessionDelegateConnectionEventArgs e)
        {
            EndSession();
        }

        private void OnDidConnect(object sender, EventArgs e)
        {
            Publish();
        }

        private void OnStreamCreated(object sender, OTSessionDelegateStreamEventArgs e)
        {
            Subscribe(e.Stream);
        }

        private void OnStreamDestroyed(object sender, OTSessionDelegateStreamEventArgs e)
        {
            _myStreamContainer.InvokeOnMainThread(() =>
            {
                DeactivateStreamContainer(_myStreamContainer);
                DeactivateStreamContainer(_otherStreamContainer);
            });
        }

        #endregion

        #region subscriber evenets

        private void OnSubscriberVideoDataReceived(object sender, EventArgs e)
        {
            ActivateStreamContainer(_otherStreamContainer, _subscriber.View);
        }

        private void OnSubscriberVideoEnabled(object sender, OTSubscriberKitDelegateVideoEventReasonEventArgs e)
        {
            lock (_syncRoot)
            {
                if (_subscriber != null && _subscriber.Stream != null && _subscriber.Stream.HasVideo)
                    IsSubscriberVideoEnabled = true;
            }
        }

        private void OnSubscriberDidConnectToStream(object sender, EventArgs e)
        {
            lock (_syncRoot)
            {
                if (_subscriber != null)
                {
                    _subscriberConnected = true;
                    ActivateStreamContainer(_otherStreamContainer, _subscriber.View);
                    if (_subscriber.Stream != null && _subscriber.Stream.HasVideo)
                        IsSubscriberVideoEnabled = true;
                }
            }
        }

        #endregion

        #region subscriber evenets

        void OnPublisherStreamCreated(object sender, OTPublisherDelegateStreamEventArgs e)
        {
            OnPublishStarted();
        }

        #endregion
    }
}
