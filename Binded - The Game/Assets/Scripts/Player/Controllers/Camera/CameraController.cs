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
      // camera principal
      private Camera _main_camera;
      // pitch e yaw determinado
      private Vector3 _input_pitch_yaw;
      // alvo da camera
      private Transform _camTarget;
      // vector de rotaçao defenida
      private Vector3 _cam_calculated_rot = Vector3.zero;
      // valor da posiçao do alvo com o offset
      private Vector3 _offseted_cam_target;
      // velocidade de damp
      private Vector3 _damp_speed;

      // camera calculated position
      private Vector3 _cam_calculated_pos;

      // testes de clipping da camera
      private LayerMask mask;
      // hit usado para o test
      private RaycastHit clippingHit;

      private void Awake()
      {
            // cache a referencia para a camera principal
            _main_camera = Camera.main;
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
      }

      private void Update()
      {
            // call para o comportamento da camera
            CameraInputBehaviour();
            // determina se a camera esta em clipping ou nao
            AvoidCameraClipping();
            // actualiza a posiçao de input na informaçao do jogador
            char_Information.UpdateInputDir(_main_camera.transform.forward);
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
            _main_camera.transform.eulerAngles = _cam_calculated_rot + (_camTarget.eulerAngles * 0.25f);

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

            if (Physics.Raycast(_offseted_cam_target, (_cam_calculated_pos - _offseted_cam_target).normalized,
             out clippingHit, Vector3.Distance(_cam_calculated_pos, _offseted_cam_target), mask))
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
                 Vector3.Lerp(_main_camera.transform.position, _cam_calculated_pos, game_settings.TimeMultiplication() / SecToRotation);
      }
}