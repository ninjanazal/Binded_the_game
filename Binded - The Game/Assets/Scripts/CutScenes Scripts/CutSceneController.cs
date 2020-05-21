using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayableDirector), typeof(IEnumeratorCallBacks))]
public class CutSceneController : MonoBehaviour
{
    // private vars
    private bool is_scene_ready = false, waiting_input = false;    // indica se a cena está pronta
    AsyncOperation load_operation;      // operaçao para load


    // indica para que scene deve transitar
    public KLevelName load_to;      // indica para que scene deve dar load
    // indica que precisa de input
    public bool need_input;     // indica se o loader precisa de input

    private void Update()
    {
        // se nao utilizar input
        if (!need_input) return;

        // se estiver á espera de input e o entrer for pressionado
        if (waiting_input && Input.GetAxisRaw("Start") != 0)
        {
            Debug.Log("Showing scene");

            // Mostra a cena
            ShowLoadedScene();
        }
    }


    // handler para o final da cutScene
    public void CutSceneEndHandler()
    {
        // ao terminar chama a o gestor de inumerados para iniciar o carregamento da nova scene
        IEnumeratorCallBacks.Instance.LoadNewScene((int)load_to);
    }

    // handler para o final da cutScene, loader interno
    public void LoadSceneAndWait()
    {
        // aplica o volume definido
        IEnumeratorCallBacks.Instance.game_settings.SetListenerVolume();
        // TODO, carregamente a par da cena
        StartCoroutine(LoadAsyncAndWaitScene());
    }

    // handler para mostrar a cena carregada
    public void ShowLoadedScene() => StartCoroutine(ShowOrWaitForLoad());
    // handler para esperar input
    public void StartWaitingForInput() { waiting_input = true; }

    // carrega async a cena definida
    private IEnumerator LoadAsyncAndWaitScene()
    {
        // iniciar o load async
        load_operation = SceneManager.LoadSceneAsync((int)load_to);
        // impede que active imediatamente a cena
        load_operation.allowSceneActivation = false;

        // enquanto estiver a dar load
        while (load_operation.progress >= .9f) { yield return null; }

        //assim que terminar de carregar indica que está pronta
        is_scene_ready = true;
    }

    // mostra ou espera o final do carregamento
    private IEnumerator ShowOrWaitForLoad()
    {
        // enquanto nao estiver carregado, espera
        while (!is_scene_ready) { yield return null; }

        // assim que disponivel mostra a cena
        load_operation.allowSceneActivation = true;
    }

}
