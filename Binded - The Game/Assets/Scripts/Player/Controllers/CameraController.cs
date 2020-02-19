using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Script fror controlling the Main camera
    // -----------------------------------------------------------------------

    // referencia para Settings do jogo
    [Header("Game Settings Object")]
    public GameSettings game_settings;

    // private Vars
    #region PrivateVars
    private Camera _mainCamera;                 // referencia para a Camera principal da cena
    public Transform CameraTarget;     // posiçao do alvo da camera
    private bool _usingCamera;                  // bool retem a utilizaçao da camera
    #endregion


    // On Start
    void Awake()
    {

        _mainCamera = Camera.main;          // retem a camera marcada com main
        CameraTarget = null;       // reset camera target

        // para determinar a deslocaçao do rato, é colocado no centro do ecra
        // para executar calculos de deslocamento
        _usingCamera = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update
    void Update()
    {
        // funçao para determinar o input do rato na camera
        ManageMouseCameraInput();
    }

    // debug gizmos
    private void OnDrawGizmos()
    {
        // Draw para debugs
    }
    // metodos privados
    #region private
    // Rotaçao da camera pelo input da camera
    private void ManageMouseCameraInput()
    {
        // determinado a quantidade de pixeis deslocados no ultimo frame
        var currentMousePosition = new Vector2(-Input.GetAxis("Mouse Y"),
                                    Input.GetAxis("Mouse X"));

        // caso esteja a utilizar a camera
        // e a posiçao do rato foi alterada        
        if (_usingCamera && currentMousePosition != Vector2.zero)
        {
            // confirma se exite um alvo definido
            if (CameraTarget == null)
            {
                // se nao existir a camera deve rodar sobre si propria
                // determina a rotaçao da direcçao actual para a direçao 
                // desejada atravez do deslocamento do rato

                _mainCamera.transform.forward =
                    Quaternion.Euler(currentMousePosition.x * game_settings.MouseSensitivity,
                    currentMousePosition.y * game_settings.MouseSensitivity, 0.0f) *
                    _mainCamera.transform.forward;
            }
            else
            {
                // se tiver um alvo definido, a camera deverá rodar em torno desse objecto
                // para tar a camera deverá estabelecer a posiçao alvo de lookAt
                _mainCamera.transform.LookAt(CameraTarget, Vector3.up);
            }
        }
    }

    #endregion

    // metodo estatico para controlar se o rato esta ou nao preso
    public void IsUsingcamera(bool isUsing)
    {
        // se sim
        if (isUsing)
            // bloqueia a posiçao do rato
            Cursor.lockState = CursorLockMode.Locked;
        else
            // se nao, liberta a utilizaçao do rato
            Cursor.lockState = CursorLockMode.None;
    }
}