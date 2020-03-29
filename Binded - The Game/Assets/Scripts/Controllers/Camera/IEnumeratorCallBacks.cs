using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// singleton responsavel pelos efeitos e manipulaçao da camera
public class IEnumeratorCallBacks : MonoBehaviour
{
    // singleton dos callbacks para alteraçao externa de paramentros
    private static IEnumeratorCallBacks _instance;
    // custom gettter para a instancia
    public static IEnumeratorCallBacks Instance { get { return _instance; } }

    // vars internas
    private bool is_enabled = false;    // define se os callbecks respondem ou nao
    GameSettings game_settings; // defeniçoes do jogo

    private IEnumerator new_camera_distance_coroutine;    // corotina que contrala a disntancia da camera
    private IEnumerator pulse_fov_effect;   // corroutina que controla o efeito de pulsar o fov

    // on awake
    void Awake()
    {
        // confirma a existencia de algum CameraCallBacks na cena
        if (_instance != null && _instance != this)
            Destroy(this);  // caso existe, destroy o object
        else
            _instance = this;    // caso contrario, esta é a instancia
    }
    // chamada pela camera para activas as funçoes
    public void Activate(GameSettings settings)
    {
        is_enabled = true;  // activa os callbacks
        game_settings = settings;   // guarda referencia para as settings do jogo

    }

    // callback para alterar a distancia da camera
    public void SetNewCameraDistance(float newDistance, float vel)
    {
        // se os callbacks estiverem activos
        if (is_enabled)
        {
            // se existir uma call para esta transformaçao
            if (new_camera_distance_coroutine != null)
                // para a transformaçao actual
                StopCoroutine(new_camera_distance_coroutine);
            // define a nova coroutina
            new_camera_distance_coroutine = ChangeCameraDistance(newDistance, vel);
            // inicia a coroutina definida anteriormente
            StartCoroutine(new_camera_distance_coroutine);
        }
    }

    // callback para o efeito de pulsar do fov
    public void PulseFOVEffectCallback(float val)
    {
        // se os callbacks estiverem activos
        if (is_enabled)
        {
            // caso exista o rotina, para-a
            if (pulse_fov_effect != null)
                StopCoroutine(pulse_fov_effect);
            // defina a nova
            pulse_fov_effect = PulseFOVEffet(val);
            // inicia a rotina de novo
            StartCoroutine(pulse_fov_effect);
        }
    }


    // metodos enumerados
    #region IEnumerators

    // corroutina que altera o valor da distancia da camera
    private IEnumerator ChangeCameraDistance(float newDistance, float vel)
    {
        // enquanto a transformaçao nao for concluida
        while (game_settings.CameraDistance != newDistance)
        {
            // move o valor da disntacia para a distancia definida
            game_settings.CameraDistance = Mathf.SmoothStep(game_settings.CameraDistance,
                newDistance, vel * game_settings.PlayerTimeMultiplication());

            // yield retunr para defenir aqui o ponto de paragem para o proximo ciclo "Async"
            yield return null;
        }
    }

    // coroutina que pulsa o valor de Field of view da camera
    private IEnumerator PulseFOVEffet(float val)
    {
        // enquanto este processo ocorre o tempo para o jogador é metade do normal
        game_settings.SetTimeMultiplier(0.01f);

        // enquanto o valor do fov nao for igual ao passado 
        while (game_settings.CameraFOV != val)
        {
            // move o valor defenido do fov para o valor passado
            game_settings.CameraFOV = Mathf.SmoothStep(game_settings.CameraFOV, val,
                game_settings.FOV_Pulse_Speed * Time.deltaTime);

            // se a diferença ente os dois valores for inferior ao threshold
            if (Mathf.Abs(game_settings.CameraFOV - val) < 0.1f)
            {
                // iguala o valor de fov ao desejado
                game_settings.CameraFOV = val;
                // quebra o ciclo
                break;
            }
            // ponot de paragem para o proximo ciclo "Async"
            yield return null;
        }

        // repoem a fov para a normal
        while (game_settings.CameraFOV != game_settings.base_camera_fov)
        {
            // desloca o valor definido para o valor base
            game_settings.CameraFOV = Mathf.SmoothStep(game_settings.CameraFOV, game_settings.base_camera_fov,
                game_settings.FOV_Pulse_Speed * Time.deltaTime);

            // determina se a diferença é menor que o threshold
            if (Mathf.Abs(game_settings.CameraFOV - game_settings.base_camera_fov) < 0.1f)
            {
                // se sim, aplica o valor desejado á camera
                game_settings.CameraFOV = game_settings.base_camera_fov;
                // quebra o ciclo
                break;
            }
            // ponto de paragem para o proximo ciclo
            yield return null;
        }
        // ao terminar volta a colocar o tempo normal
        game_settings.SetTimeMultiplier(1f);
    }

    #endregion
}
