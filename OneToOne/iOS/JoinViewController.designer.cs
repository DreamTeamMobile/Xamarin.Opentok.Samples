// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace DT.Samples.Opentok.OneToOne.iOS
{
    [Register ("JoinViewController")]
	partial class JoinViewController
	{
		[Outlet]
		UIKit.UIActivityIndicatorView ActivityInProgressView { get; set; }

		[Outlet]
		UIKit.UITextField ChannelNameEdit { get; set; }

		[Outlet]
		UIKit.UILabel ConnectingLabel { get; set; }

		[Outlet]
		UIKit.UIImageView ConnectionImage { get; set; }

		[Outlet]
		UIKit.UITextField EncryptionKeyEdit { get; set; }

		[Outlet]
		UIKit.UIButton JoinButton { get; set; }

		[Outlet]
		UIKit.UIView OpacityLayerView { get; set; }

		[Outlet]
		UIKit.UILabel SDKVersionLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ActivityInProgressView != null) {
				ActivityInProgressView.Dispose ();
				ActivityInProgressView = null;
			}

			if (SDKVersionLabel != null) {
				SDKVersionLabel.Dispose ();
				SDKVersionLabel = null;
			}

			if (ChannelNameEdit != null) {
				ChannelNameEdit.Dispose ();
				ChannelNameEdit = null;
			}

			if (ConnectingLabel != null) {
				ConnectingLabel.Dispose ();
				ConnectingLabel = null;
			}

			if (ConnectionImage != null) {
				ConnectionImage.Dispose ();
				ConnectionImage = null;
			}

			if (EncryptionKeyEdit != null) {
				EncryptionKeyEdit.Dispose ();
				EncryptionKeyEdit = null;
			}

			if (JoinButton != null) {
				JoinButton.Dispose ();
				JoinButton = null;
			}

			if (OpacityLayerView != null) {
				OpacityLayerView.Dispose ();
				OpacityLayerView = null;
			}
		}
	}
}
