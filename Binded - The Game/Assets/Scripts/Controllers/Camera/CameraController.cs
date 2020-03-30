using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // player camera script
    // public vars -----------------------------------------
    [Header("GameSettings Obj")]
    public GameSettings game_settings;
    // valores de informaçao do jogador
    [Header("Char Informations")]
    public CharacterInfo char_Information;
    // camera offset
    [Header("Offset da camera")]
    public Vector3 camera_offset;
    // tempo para a rotaçao smooth da camera
    [Header("Tempo desejado para a rotaçao da camera")]
    public float SecToRotation = 0.2f;

    // distancia maxima de aproximaçao ao objecto 
    [Header("Distancia minima da cam ao alvo")]
    public float MinCameraDistance = 0.5f;

    // private vars ------------------------------------------
    private Camera _main_camera;    // camera principal
    private Vector3 _input_pitch_yaw;    // pitch e yaw determinado
    private Transform _camTarget;    // alvo da camera
    private Vector3 _cam_calculated_rot = Vector3.zero;    // vector de rotaçao defenida
    private Vector3 _offseted_cam_target;    // valor da posiçao do alvo com o offset
    private Vector3 _damp_speed;    // velocidade de damp
    private Vector3 _cam_calculated_pos;    // camera calculated position
    private LayerMask mask;    // testes de clipping da camera
    private RaycastHit clippingHit;    // hit usado para o test


    private void Start()
    {
        // cache a referencia para a camera principal
        _main_camera = Camera.main;
        // activa textura de profundidade
        _main_camera.depthTextureMode = DepthTextureMode.Depth;

        // inica o vetor de yaw e pitch
        _input_pitch_yaw = Vector2.zero;
        // garda cache para o player
        // tenta encontrar um jogador na cena
        try { _camTarget = GameObject.FindGameObjectWithTag("Player").transform; }
        catch (System.Exception ex) { Debug.LogError(ex); throw; }

        // bloquear o rato
        Cursor.lockState = CursorLockMode.Locked;

        // define a mask para teste de clipping
        mask = ~LayerMask.GetMask("Ignore CameraClipping");
        // inia a posiçao alvo da camera igual á actual
        _cam_calculated_pos = _main_camera.transform.position;

        // activa os callbacks para a camera
        IEnumeratorCallBacks.Instance.Activate(game_settings);
    }

    private void Update()
    {
        // call para o comportamento da camera
        CameraInputBehaviour();
        // determina se a camera esta em clipping ou nao
        AvoidCameraClipping();
        // actualiza o valor de fov
        UpdateFOVValue();

        // actualiza a posiçao de input na informaçao do jogador
        char_Information.UpdateInputDir(_main_camera.transform.forward);
        // actualiza o up da camera
        char_Information.UpdateInputDirUp(_main_camera.transform.up);
    }

    // metodo de comportamento da camera 
    private void CameraInputBehaviour()
    {
        //determina o valor de yaw e pitch para o input do rato
        // calcula o valor do angulo total
        _input_pitch_yaw += new Vector3(-Input.GetAxis("Mouse Y") * game_settings.MouseSensitivity,
            Input.GetAxis("Mouse X") * game_settings.MouseSensitivity);

        // limitada que o pitch seja maior que valores defenidos nas settings
        _input_pitch_yaw.x =
            Mathf.Clamp(_input_pitch_yaw.x, game_settings.MinVerticalCamAngle, game_settings.MaxVerticalCamAngle);

        // posiçao da camera
        // guarda o valor do offset com base na posiçao do alvo
        _offseted_cam_target = _camTarget.transform.position +
             _main_camera.transform.TransformDirection(camera_offset);

        // definir a direcçao da camera, com base na direcçao do alvo
        _cam_calculated_rot =
            Vector3.SmoothDamp(_cam_calculated_rot, _input_pitch_yaw, ref _damp_speed, SecToRotation);
        // atribui a rotaçao determinada
        _main_camera.transform.eulerAngles = _cam_calculated_rot;

        // Define a posiçao da camera
        _cam_calculated_pos = _offseted_cam_target -
            (_main_camera.transform.forward * game_settings.CameraDistance);

        // determina se esta posiçao calculada esta á distancia desejada do alvo
        if (Vector3.Distance(_cam_calculated_pos, _camTarget.position) > game_settings.CameraDistance)
            _cam_calculated_pos = _camTarget.transform.position +
                (_cam_calculated_pos - _camTarget.position).normalized * game_settings.CameraDistance;
    }

    // teste para inpedir o clipping da camera
    private void AvoidCameraClipping()
    {
        // utilizando um ray da direçao do player ate á posiçao da camera
        // caso colida com algum objecto , deve aproximar a camera

        if (Physics.Raycast(_camTarget.position, (_cam_calculated_pos - _camTarget.position).normalized,
         out clippingHit, Vector3.Distance(_cam_calculated_pos, _camTarget.position), mask))
        {
            // se este teste for positivo, a camera deve ser deslocada para a posicao de alvo mais um offset 
            // de distancia
            _cam_calculated_pos = clippingHit.point + _main_camera.transform.forward;

            // determina se a camera esta dentro das proximidades determinadas
            if (Vector3.Distance(_cam_calculated_pos, _camTarget.position) < MinCameraDistance)
                // ajusta a posiçao calculada
                _cam_calculated_pos = _offseted_cam_target - (_main_camera.transform.forward * MinCameraDistance);
        }

        // move a camera a posiçao determinada
        _main_camera.transform.position =
             Vector3.Lerp(_main_camera.transform.position, _cam_calculated_pos, game_settings.PlayerTimeMultiplication() / SecToRotation);
    }

    // actualiza o valor de fov 
    private void UpdateFOVValue()
    {
        // aplica o valor das settings no fov da camera
        _main_camera.fieldOfView = game_settings.CameraFOV;
    }
}