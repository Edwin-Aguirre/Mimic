using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Rendering;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField]
    private AudioMixer musicMixer;

    [SerializeField]
    private AudioMixer soundFXMixer;

    [SerializeField]
    private Slider musicSlider;

    [SerializeField]
    private Slider soundSlider;

    [SerializeField]
    private RenderPipelineAsset[] qualityLevels;
    [SerializeField]
    private TMP_Dropdown settingsDropdown;

    [SerializeField]
    private GameObject settingsMenu;

    // Start is called before the first frame update
    void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVol", musicSlider.value);
        soundSlider.value = PlayerPrefs.GetFloat("SoundVol", soundSlider.value);
        settingsDropdown.value = PlayerPrefs.GetInt("Settings", settingsDropdown.value);
        settingsDropdown.value = QualitySettings.GetQualityLevel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable() 
    {
        PlayerPrefs.SetFloat("MusicVol", musicSlider.value);
        PlayerPrefs.SetFloat("SoundVol", soundSlider.value);
        PlayerPrefs.SetInt("Settings", settingsDropdown.value);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        QualitySettings.renderPipeline = qualityLevels[qualityIndex];
    }

    public void SetLevel(float sliderValue)
    {
        musicMixer.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20f);
    }

    public void SetSoundLevel(float sliderValue)
    {
        soundFXMixer.SetFloat("SoundVol", Mathf.Log10(sliderValue) * 20f);
    }

    public void Fullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}
