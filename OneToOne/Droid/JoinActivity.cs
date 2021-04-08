using System.Linq;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using DT.Samples.Opentok.Shared;
using DT.Samples.Opentok.Shared.Helpers;

namespace DT.Samples.Opentok.OneToOne.Droid
{
    [Activity(Label = "@string/app_name", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class JoinActivity : AppCompatActivity
    {
        protected const int REQUEST_ID = 0;
        protected string[] REQUEST_PERMISSIONS = new string[] {
            Manifest.Permission.Camera,
            Manifest.Permission.WriteExternalStorage,
            Manifest.Permission.RecordAudio,
            Manifest.Permission.ModifyAudioSettings,
            Manifest.Permission.Internet,
            Manifest.Permission.AccessNetworkState
        };

        protected const string QualityFormat = "Current Connection - {0}";
        protected const string OpentokVersionText = " <version>";
        private View _layout;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Join);
            _layout = FindViewById(Resource.Id.joinLayout);
            CheckPermissions();
            FindViewById<EditText>(Resource.Id.channelName).Text = OpentokSettings.Current.RoomName;
            FindViewById<TextView>(Resource.Id.version_text).Text = OpentokVersionText;
        }

        protected bool CheckPermissions(bool requestPermissions = true)
        {
            var isGranted = REQUEST_PERMISSIONS.Select(permission => ContextCompat.CheckSelfPermission(this, permission) == (int)Permission.Granted).All(granted => granted);
            if (requestPermissions && !isGranted)
            {
                ActivityCompat.RequestPermissions(this, REQUEST_PERMISSIONS, REQUEST_ID);
            }
            return isGranted;
        }

        [Java.Interop.Export("OnJoin")]
        public void OnJoin(View v)
        {
            OpentokSettings.Current.RoomName = FindViewById<EditText>(Resource.Id.channelName).Text;
            CheckPermissionsAndStartCall();
        }

        private void CheckPermissionsAndStartCall()
        {
            if (CheckPermissions(false))
            {
                StartActivity(typeof(RoomActivity));
            }
            else
            {
                Snackbar.Make(_layout, Resource.String.permissions_not_granted, Snackbar.LengthShort).Show();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {

            switch (item.ItemId)
            {
                case Resource.Id.menu_share:
                    ShareActivity();
                    break;
            }
            return true;
        }

        private void ShareActivity()
        {
            Intent sendIntent = new Intent();
            sendIntent.SetAction(Intent.ActionSend);
            sendIntent.PutExtra(Intent.ExtraText, OpentokTestConstants.ShareString);
            sendIntent.SetType("text/plain");
            StartActivity(sendIntent);
        }


        public override bool OnTouchEvent(MotionEvent e)
        {
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(CurrentFocus.WindowToken, 0);
            return base.OnTouchEvent(e);
        }
    }
}
