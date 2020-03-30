using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffectsManager : MonoBehaviour
{
    // singleton estatico para manipulaçao dos efeitos
    private static CameraEffectsManager effect_manager_;
    public static CameraEffectsManager Instance { get { return effect_manager_; } }

    // referencia para as settings do jogo
    public GameSettings game_settings_;

    [Header("Lista ordenada de efeitos a passar na camera")]
    public Material camDimmEffec;   // efeito principal da camera
    private IEnumerator effectRoutine;

    // variaveis internas
    public bool usingEffect = false;   // indica se estao a ser usados efeitos ou nao
    public Camera effect_camera_;    // referencia á camera para sobrepor sobre efeitos

    // guarda referencia para a camera e desativa
    private void Start()
    {
        effect_manager_ = this; // indica que a instancia é este objecto
        //effect_camera_ = GetComponentInChildren<Camera>();  // guarda referencia para a camera de efeitos
        effect_camera_.enabled = false; // desativa a camera
    }


    // efeitos de camera
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // ao imprimir a imagem final
        // define a imagem de entrada e saida, passando pelo matrial passado
        if (usingEffect)
        {    // caso exista efeito para passar na camera
            if (camDimmEffec)
                // passa a imagem de saida pelo shader do material
                Graphics.Blit(source, destination, camDimmEffec);
        }
        // se nao estiver a usar o efeito, corre o blit normal
        else Graphics.Blit(source, destination);

    }

    // metodo chamado para activar o effeito
    public void EnableEffects()
    {
        // ao ser chamado avalia se é um enable ou disable do effeito
        // se actualmente nao estiver o efeito em uso
        if (!usingEffect)
        {
            // caso exista uma routina, para
            if (effectRoutine != null)
                StopCoroutine(effectRoutine);   // para a routina
            effectRoutine = ChangeValueto(0f, 1f, 0.2f);    // cria um novo iEnumerate
            StartCoroutine(effectRoutine);  // inicia a nova routina
        }
        // caso esteja a usar um efeito, tem de desativar
        else if (usingEffect)
        {
            StopCoroutine(effectRoutine);   // visto que para acontecer um disable ja existe um efeito, apenas para
            effectRoutine = ChangeValuetoDisable(1f, 0f, 2f);   // cria o novo disable
            StartCoroutine(effectRoutine);  // começa a nova routina
        }

    }

    // metodo chamado para desativar o efeito
    public void DisableEffects() { if (usingEffect) StartCoroutine(ChangeValuetoDisable(1f, 0f, 50f)); }

    // ienumerador privado para acompanhar a activaçao de um efeito
    private IEnumerator ChangeValueto(float from, float to, float speed)
    {
        // activa a camera
        effect_camera_.enabled = true;
        usingEffect = true; // activa o efeito
        // torna o jogador mais lento no tempo
        game_settings_.SetTimeMultiplier(0.25f);

        // enquanto o valor nao for alcançado
        while (from != to)
        {
            // varia o valor de from
            from = Mathf.Lerp(from, to, speed * Time.deltaTime);

            // caso os valores ja estejam muito proximos
            if ((to - from) < 0.8f)
                from = to;

            // aplica esse valor ao material
            camDimmEffec.SetFloat("_DimmAmount", from);

            // return para o proximo ciclo
            yield return null;

        }
    }
    // ienumerador privado para acompanhar a desativaçao dos efeitos na camera
    private IEnumerator ChangeValuetoDisable(float from, float to, float speed)
    {
        // enquanto o valor nao for alcançado
        while (from != to)
        {
            // varia o valor de from
            from = Mathf.Lerp(from, to, speed * Time.deltaTime);

            // caso os valores ja estejam muito proximos
            if ((from) < 0.2f)
                from = to;

            // aplica esse valor ao material
            camDimmEffec.SetFloat("_DimmAmount", from);

            // return para o proximo ciclo
            yield return null;

        }
        // volta ao tempo normal
        game_settings_.SetTimeMultiplier(1f);

        // desativa o efeito assim que termina
        usingEffect = false;
        effect_camera_.enabled = false;
    }

}
