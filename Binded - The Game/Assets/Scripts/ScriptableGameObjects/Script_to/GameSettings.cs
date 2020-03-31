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
    [Header("Camera")]
    // Sensibilidade rato
    [Range(0.5f, 7f)]
    public float MouseSensitivity = 2f;

    // Distancia da camera ao alvo
    [Range(0.5f, 8f)]
    public float CameraDistance = 5f;

    // clap de angulo vertica
    [Range(-45, 80)]
    public float MinVerticalCamAngle, MaxVerticalCamAngle;

    // fov da camera
    [Header("Efeito de pulsar o FOV")]
    [Range(10f, 60f)] public float FOV_Pulse_Speed = 2f;
    public float base_camera_fov = 60; 

    // private Vars -------------------------------------------------------
    // Multiplicador de tempo
    private float _timeMultiplier = 1f;
    // fov actual da camera
    private float camera_fov_ = 60;



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
    
    // get/Set para o fov da camea
    public float CameraFOV { get => camera_fov_; set => camera_fov_ = value; }

    // funçao chamada para defenir a distancia da camera
    public void SetCameraDistanceTarget(float targetDistance, float speed)
        => IEnumeratorCallBacks.Instance.SetNewCameraDistance(targetDistance, speed);

    // funçao chamada para definir a FOV da camera
    public void StartCameraPulseFOVEffect(float val) 
        => IEnumeratorCallBacks.Instance.PulseFOVEffectCallback(val);

    #endregion

}