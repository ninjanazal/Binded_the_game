using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    public Dropdown graphicsdropdown;
    public Dropdown ResolutionDropDown;
    public Slider VolumeSlider;
    public Resolution[] resolutions;
    public GameOptions gameOptions;
    public AudioSource volumesource;

    private void OnEnable()
    {
        gameOptions = new GameOptions();

        graphicsdropdown.onValueChanged.AddListener(delegate { OnGraphicsChange(); });
        ResolutionDropDown.onValueChanged.AddListener(delegate { OnResolutionChange(); });
        VolumeSlider.onValueChanged.AddListener(delegate { OnVolumeChange(); });


        resolutions = Screen.resolutions;
        foreach(Resolution resolution in resolutions)
        {
            ResolutionDropDown.options.Add(new Dropdown.OptionData(resolution.ToString()));

        }
    }

    public void OnResolutionChange()
    {
        Screen.SetResolution(resolutions[ResolutionDropDown.value].width, resolutions[ResolutionDropDown.value].height, Screen.fullScreen);

    }
    public void OnGraphicsChange()
    {
        QualitySettings.masterTextureLimit = gameOptions.graphicsquality = graphicsdropdown.value;
        
    }

    public void OnVolumeChange()
    {
        volumesource.volume = gameOptions.Volume = VolumeSlider.value;
    }

    
}
