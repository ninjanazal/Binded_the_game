using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Asset criado em Binded
[CreateAssetMenu(fileName = "Game Settings", menuName = "Binded/GameSettings")]
public class GameSettings : ScriptableObject
{
    // objecto que refere as defeniçoes do jogo
    // Master volume
    [Header("Sound")]
    [Range(0.1f, 1f)]
    public float MasterVolume = 1f;

    [Header("Controls")]
    [Header("   Camera")]
    // Sensibilidade rato
    [Range(0.5f, 7f)]
    public float MouseSensitivity = 2f;

    // Distancia da camera ao alvo
    [Range(0.5f, 5f)]
    public float CameraDistance = 5f;

    // clap de angulo vertica
    [Range(-45, 80)]
    public float MinVerticalCamAngle, MaxVerticalCamAngle;

    // private Vars -------------------------------------------------------
    // Multiplicador de tempo
    private float _timeMultiplier = 1f;


    // funcs
    #region Funcs
    // tempo
    // define um novo multiplicador
    public void SetTimeMultiplier(float value)
    {
        // atribui o novo valor de tempo
        _timeMultiplier = value;
    }
    // ajuste ao arranjo do tempo em jogo
    public float PlayerTimeMultiplication()
    {
        // retorna o tempo basal ajustado á multiplicaçao
        return Time.deltaTime * _timeMultiplier;
    }
    #endregion
}