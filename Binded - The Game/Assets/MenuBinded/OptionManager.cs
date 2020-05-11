using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// class de controlo do menu
public class OptionManager : MonoBehaviour
{
    public Dropdown graphics_dropdown;   // dropdown a ser preenchida com as qualidades graficas
    public Dropdown resolution_dropDown; // dropdown a ser preenchida com as resoluçoes disponiveis
    public Slider volume_slider;         // slider para controlo do volume

    // variaveis privadas
    private Resolution[] resolutions_;   // array das resoluçoes disponiveis

    private void OnEnable()
    {
        // atribui um delegate para callback aquando aletraçao do valor 
        // callback para alteraçao da qualidade grafica
        graphics_dropdown.onValueChanged.AddListener(delegate { OnGraphicsChange(); });
        // callback para alteraçao da resoluçao 
        resolution_dropDown.onValueChanged.AddListener(delegate { OnResolutionChange(); });
        // callback para alteraçao do valor de volume
        volume_slider.onValueChanged.AddListener(delegate { OnVolumeChange(); });

        // guarda todas as resoluçoes disponiveis no array
        resolutions_ = Screen.resolutions;
        foreach (Resolution resolution in resolutions_)
            // preenche a dropdown com as resoluçoes disponiveis
            resolution_dropDown.options.Add(new Dropdown.OptionData(resolution.ToString()));


    }

    // handler para alteraçao da resoluçao
    public void OnResolutionChange()
    {
        // define a resoluçao selecionada de acordo com a largura e altura
        // nota que está sempre definido que o jogo executa em ecra completo
        Screen.SetResolution(resolutions_[resolution_dropDown.value].width, resolutions_[resolution_dropDown.value].height, Screen.fullScreen);
    }
    // handler para alteraçao da qualidade dos graficos
    public void OnGraphicsChange()
    {
        // define a qualidade do jogo de acordo com o selecionado no dropdown
        QualitySettings.masterTextureLimit = graphics_dropdown.value;
    }

    // altera o valor do volume nas definiçoes do jogo
    public void OnVolumeChange()
    {
        // altera valores do volume master
        AudioListener.volume = volume_slider.value;
    }


}
