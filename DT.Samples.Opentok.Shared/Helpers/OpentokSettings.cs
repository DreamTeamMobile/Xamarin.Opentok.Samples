// Helpers/Settings.cs
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace DT.Samples.Opentok.Shared.Helpers
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters. 
    /// </summary>
    public class OpentokSettings : BasePropertyChanged
    {
        public const int MaxPossibleBitrate = 300;
        public const int MinPossibleBitrate = 100;

        static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        static OpentokSettings _settings;
        public static OpentokSettings Current
        {
            get { return _settings ?? (_settings = new OpentokSettings()); }
        }

        #region Setting Constants

        private const string ProfileKey = "op_profile_key";
        private static readonly int ProfileDefault = 30;

        private const string UseMySettingsKey = "op_useMySettings_key";
        private static readonly bool UseMySettingsDefault = false;

        private const string RoomNameKey = "op_roomName_key";
        private static readonly string RoomNameDefault = "drmtm.us";

        #endregion

        public int Profile
        {
            get
            {
                return AppSettings.GetValueOrDefault(ProfileKey, ProfileDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(ProfileKey, value);
                OnPropertyChanged();
            }
        }

        public bool UseMySettings
        {
            get
            {
                return AppSettings.GetValueOrDefault(UseMySettingsKey, UseMySettingsDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(UseMySettingsKey, value);
                OnPropertyChanged();
            }
        }

        public string RoomName
        {
            get
            {
                return AppSettings.GetValueOrDefault(RoomNameKey, RoomNameDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(RoomNameKey, value);
                OnPropertyChanged();
            }
        }
    }
}