using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections.Generic;
using Controllers;

namespace GameUI
{
    /// <summary>
    /// <c>SettingsMenu</c> defines handlers for the UI elements of the settings menu screen.
    /// This includes handlers for changing graphics, audio and mouse sensitivity.
    /// </summary>
    public class SettingsMenu : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private TMP_Dropdown qualityDropdown;
        [SerializeField] private Slider audioSlider;
        [SerializeField] private Slider mouseSensitivitySlider;
        [SerializeField] private Toggle fullscreenToggle;
        [SerializeField] private GameObject player;

        private Resolution[] resolutions;

        void Start()
        {
            // Load volume and mouse sensitivity settings from PlayerPrefs
            float volume = PlayerPrefs.GetFloat("Volume", 0);
            audioSlider.value = volume;
            audioMixer.SetFloat("Volume", volume);

            float mouseSensitivity = PlayerPrefs.GetFloat("mouseSensitivity", 0);
            mouseSensitivitySlider.value = mouseSensitivity;

            // Graphics settings are saved in windwos registry so no need to use PlayerPrefs
            qualityDropdown.value = QualitySettings.GetQualityLevel();

            fullscreenToggle.isOn = Screen.fullScreen;

            // Get available resolutions of current screen and add them as Dropdown options
            // & load last used resolution
            resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();

            List<string> options = new List<string>();

            int currentResolutionIndex = 0;
            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.width &&
                    resolutions[i].height == Screen.height)
                {
                    currentResolutionIndex = i;
                }
            }
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        /// <summary>
        /// <c>SetVolume</c> is hooked up to the "Volume" slider and allows to control the ingame audio mixer volume level.
        /// </summary>
        /// <param name="volume">the new volume value</param>
        public void SetVolume(float volume)
        {
            audioMixer.SetFloat("Volume", volume);
            PlayerPrefs.SetFloat("Volume", volume);
        }

        /// <summary>
        /// <c>SetQuality</c> is hooked up to the "Quality" dropdown and allows to control the ingame graphics quality.
        /// </summary>
        /// <param name="qualityIndex">Index of the new quality setting</param>
        public void SetQuality(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex);
        }

        /// <summary>
        /// <c>SetFullscreen</c> is hooked up to the "Fullscreen" toggle and allows to switch between windowed and fullscreen mode.
        /// </summary>
        /// <param name="isFullscreen">Indicator whether fullscreen mode is active</param>
        public void SetFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }

        /// <summary>
        /// <c>SetResolution</c> is hooked up to the "Resolution" dropdown and allows to control the game resolution.
        /// </summary>
        /// <param name="resolutionIndex">Index of the new resolution setting</param>
        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(width: resolution.width, height: resolution.height, fullscreen: Screen.fullScreen);
        }

        /// <summary>
        /// <c>SetMouseSensitivity</c> is hooked up to the "Mouse Sensitivity" slider and allows to control the ingame rotation speed of the player.
        /// </summary>
        /// <param name="mouseSensitivity">the new mouse sensitivity value</param>
        public void SetMouseSensitivity(float mouseSensitivity)
        {
            player.GetComponent<PlayerController>().rotationSpeed = mouseSensitivity;
            PlayerPrefs.SetFloat("mouseSensitivity", mouseSensitivity);
        }
    }
}