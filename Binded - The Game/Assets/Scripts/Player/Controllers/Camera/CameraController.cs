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
    // posiçao do alvo da camera
    [Header("Objecto foco")]
    public Transform CameraTarget;
    // distancia minima para que o snap da camera ocora com a direcçao desejada
    [Header("Distancia minima para snap para a direcçao da camera")]
    public float MinSnapDistance = 1f;
    // velocidade de deslocamento da camera
    [Header("Velocidade de deslocamento da camera")]
    public float CamMoveSpeed = 5f;


    // private Vars ---------------------------------------------------
    #region PrivateVars
    // referencia para a Camera principal da cena
    private Camera _mainCamera;

    // bool retem a utilizaçao da camera
    public static bool _usingCamera;

    // direçao e posiçao da camera desejada
    private Vector3 _cameraTargetPosition, _cameraTargetDirection;
    private Vector2 _inputCameraAngles;

    #endregion


    // On Start
    void Awake()
    {
        // retem a camera marcada com main
        _mainCamera = Camera.main;
        // reset camera target      
        CameraTarget = null;
        // reeniciar os angulos de input
        _inputCameraAngles = Vector2.zero;


        // para determinar a deslocaçao do rato, é colocado no centro do ecra
        // para executar calculos de deslocamento
        _usingCamera = true;
        Cursor.lockState = CursorLockMode.Locked;

        // procura pelo jogador em cena
        TryGetPlayerTransform();
    }

    // Update
    void Update()
    {
        // funçao para determinar o input do rato na camera
        ManageMouseCameraInput();
    }

    // metodos privados
    #region private
    // tenta encontrar a posiçao do jogador
    private void TryGetPlayerTransform()
    {
        // ao defenir a camera para seguir o jogar
        // tenta encontrar o jogador na cena
        var player = GameObject.FindGameObjectsWithTag("Player");
        // caso nao encontre nenhum objecto com a tag de Player
        if (player.Length == 0 || player == null)
        {
            // Apresenta info na console
            Debug.LogWarning("Player not found, camera has no target;");
        }
        // caso encontre varios players em jogo
        else if (player.Length > 1)
        {
            // apresenta info na consola
            Debug.LogWarning("Multiple players found!");
        }
        // caso encontre um jogador
        else
        {
            // guarda referencia para o transform
            CameraTarget = player[0].transform;
            // informa na consola que encontrou apenas um player em cena
            Debug.Log("Player found!");
        }

    }

    // Rotaçao da camera pelo input da camera
    private void ManageMouseCameraInput()
    {
        // determinado a quantidade de pixeis deslocados no ultimo frame
        var currentMousePosition = new Vector2(-Input.GetAxis("Mouse Y"),
                                    Input.GetAxis("Mouse X"));

        // caso esteja a utilizar a camera
        // e a posiçao do rato foi alterada        
        if (_usingCamera)
        {
            // caso nao exista um alvo da camera
            if (!CameraTarget && currentMousePosition != Vector2.zero)
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
                // determinar a posiçao ideal da camera relativa ao objecto alvo
                _inputCameraAngles.x += currentMousePosition.x * game_settings.MouseSensitivity;
                _inputCameraAngles.y += currentMousePosition.y * game_settings.MouseSensitivity;

                // limitar o valor de angulo entre valores definidos nas settings
                _inputCameraAngles.x =
                    Mathf.Clamp(_inputCameraAngles.x,
                     game_settings.MinVerticalCamAngle, game_settings.MaxVerticalCamAngle);

                // com base no input e na posiçao desejada
                _cameraTargetDirection = Quaternion.Euler(_inputCameraAngles.x, _inputCameraAngles.y, 0.0f) *
                    Vector3.back;

                // determina a posiçao alvo da camera
                _cameraTargetPosition = CameraTarget.position +
                    _cameraTargetDirection * game_settings.CameraMaxDistance;

                // define posiçao da camera
                _mainCamera.transform.position = Vector3.Lerp(_mainCamera.transform.position,
                    _cameraTargetPosition, game_settings.TimeMultiplication() * CamMoveSpeed);

                // define a direçao da camera
                _mainCamera.transform.LookAt(CameraTarget);
            }

        }
    }

    #endregion

    // metodo estatico para controlar se o rato esta ou nao preso
    public void IsUsingcamera(bool isUsing)
    {
        // guarda o ultimo estado
        _usingCamera = isUsing;
        // se sim
        if (isUsing)
        {
            // bloqueia a posiçao do rato
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            // se nao, liberta a utilizaçao do rato
            Cursor.lockState = CursorLockMode.None;
        }
    }
}