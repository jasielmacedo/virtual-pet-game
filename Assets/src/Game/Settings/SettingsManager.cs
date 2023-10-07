using UnityEngine;
using Core.save;

namespace Game.Settings
{
    public class SettingsManager
    {
        public enum ESettingsType
        {
            All,
            Audio,
            Video,
            Gameplay
        }

        public const string prefInstanceId = "SimulationInstanceId";
        private const string prefsId = "gameSettingPreset";

        public SettingsKeeper settings;

        public SettingsManager()
        {
            this.settings = new SettingsKeeper();

            if (this.LoadSettings())
            {
                this.settings.video.Apply();
                this.settings.audio.Apply();
            }
        }

        public void RevertToDefault(ESettingsType which)
        {
            switch (which)
            {
                case ESettingsType.Audio:
                    this.settings.audio.Defaults();
                    break;
                case ESettingsType.Gameplay:
                    this.settings.gameplay.Defaults();
                    break;
                case ESettingsType.Video:
                    this.settings.video.Defaults();
                    break;
                default:
                    this.settings.audio.Defaults();
                    this.settings.gameplay.Defaults();
                    this.settings.video.Defaults();
                    break;
            }

            this.SaveSettings();
        }

        public bool LoadSettings()
        {
            if (!PlayerPrefs.HasKey(SettingsManager.prefsId))
            {
                return false;
            }

            SafePlayerPrefs safe = new SafePlayerPrefs("SimulationPreset", SettingsManager.prefsId);
            if (safe.HasBeenEdited())
            {
                return false;
            }

            string loaded = PlayerPrefs.GetString(SettingsManager.prefsId);
            SettingsKeeper keeper = MemoryCard.revertFromJson<SettingsKeeper>(loaded);
            if (keeper != null)
            {
                this.settings = keeper;
                return true;
            }

            return false;
        }

        public bool SaveSettings()
        {
            string json = MemoryCard.convertToJson<SettingsKeeper>(this.settings);
            if (!string.IsNullOrEmpty(json))
            {
                PlayerPrefs.SetString(SettingsManager.prefsId, json);
                PlayerPrefs.Save();
                SafePlayerPrefs safe = new SafePlayerPrefs("SimulationPreset", SettingsManager.prefsId);
                safe.Save();
                return true;
            }
            return false;
        }
    }

    [System.Serializable]
    public class SettingsKeeper
    {
        public SettingsVideo video;
        public SettingsAudio audio;
        public SettingsGameplay gameplay;

        public SettingsKeeper()
        {
            this.video = new SettingsVideo();
            this.video.Defaults();
            this.audio = new SettingsAudio();
            this.audio.Defaults();
            this.gameplay = new SettingsGameplay();
            this.gameplay.Defaults();
        }

        [System.Serializable]
        public struct SettingsVideo
        {
            public int GraphicQuality;
            public bool Shadow;

            public void Defaults()
            {
                GraphicQuality = QualitySettings.GetQualityLevel();
                Shadow = true;
            }

            public void Apply()
            {
                QualitySettings.SetQualityLevel(GraphicQuality, true);
                QualitySettings.shadows = (Shadow ? ShadowQuality.All : ShadowQuality.Disable);
            }
        }

        [System.Serializable]
        public struct SettingsAudio
        {
            // AudioListener.volume
            public int MasterVolumePercentage;

            public void Defaults()
            {
                MasterVolumePercentage = 100;
            }

            public void Apply()
            {
                AudioListener.volume = Mathf.Clamp((MasterVolumePercentage / 100f), 0.0f, 1.0f);
            }
        }

        [System.Serializable]
        public struct SettingsGameplay
        {
            public string locale;

            public void Defaults()
            {
                locale = "en";
            }
        }
    }
}
