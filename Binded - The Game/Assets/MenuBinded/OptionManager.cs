using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    public Dropdown graphicsdropdown;
    public Dropdown ResolutionDropDown;
    public Slider VolumeSlider;
    private Resolution[] resolutions;

    private void OnEnable()
    {
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
        QualitySettings.masterTextureLimit = graphicsdropdown.value;
        
    }

    public void OnVolumeChange()
    {
        // altera valores do volume master
    }

    
}
