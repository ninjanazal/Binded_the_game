using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    // variaveis publicas
    [Header("Estado do jogo")] public GameState game_state_;    // referencias para o GameState
    public CharacterInfo char_info; // informaçoes do jogador

    [Header("Referencias á jogabilidade")]
    public Transform Spawner_point; // referencia para o spawnPoint
    public GameObject PlayerObject; // jogador a a ser instanciado
    public GameObject CameraControllerObject;   // referencia para o controlador da camera

    // informaçoes sobre os todos os portais
    [Header("Informaçoes sobre portais")]
    [Space(5)]
    public PortalTrigger[] triggers = new PortalTrigger[4];  // array para os triggers dos beacons


    // variaveis internas
    // referencias ao jogador
    private CharacterSystem char_system_;   // referencia ao controlador do jogador
    private Transform char_transform_;  // referencia para o transform do jogador
    private CameraController cam_controller;    // referencia para o controlador da camera

    // referencias para a camera

    // on awake
    private void Awake()
    {
        // define a forma do jogador como Aike
        char_info.shape = PlayerShape.Aike;

        // inicia o jogador e a camera no mundo de acordo com o spawn point
        // guarda referencia para o controlador do jogador
        char_system_ = GameObject.Instantiate(PlayerObject, Spawner_point.position, Spawner_point.rotation).
            GetComponent<CharacterSystem>();
        // guarda referencia para o transform do jogador
        char_transform_ = char_system_.transform;

        // indica que o jogador pode trocar de forma
        char_system_.CanGoArif(true);
        // torna o jogador vivo
        char_system_.RespawnPlayer();
        // confirma a escala do tempo para o jogador
        char_system_.game_settings_.SetTimeMultiplier(1f);

        // inicia a camera 
        cam_controller = GameObject.Instantiate(CameraControllerObject,
            Spawner_point.position, Spawner_point.rotation).GetComponent<CameraController>();

        // defini volume dos listenners
        char_system_.game_settings_.SetListenerVolume();

        // determinar qual portal está aberto
        ConfirmPortals();
    }

    // todos os frames é avaliado
    private void Update()
    {
        // se o jogador está morto
        if (!char_system_.GetPlayerState())
            // esteja morto, inicia o seu restauro
            // coloca o jogador de volta ao ponto de spawn
            StartCoroutine(RespawnPlayer());
    }

    // metodo avalia qual o portal aberto
    private void ConfirmPortals()
    {
        //ao confirmar desativa todos os portais
        foreach (var portalTrigger in triggers) { portalTrigger.enabled = false; }
        // switch para avaliar qual dos portais deve ser aberto
        switch (game_state_.GetCurrentPortal)
        {
            case kPortals.Hamr:
                // caso seja o portal Harm que esteja aberto
                triggers[0].enabled = true; // activa o portal
                break;
            case kPortals.Hugr:
                // caso seja o portal Harm que estaja aberto
                triggers[1].enabled = true;
                break;
            case kPortals.Fylgja:
                // caso seja o portal fylgja que esteja aberto
                triggers[2].enabled = true;
                break;
            case kPortals.Hamingja:
                // caso seja o portal Hamingja que esteja aberto
                triggers[3].enabled = true;
                break;
            case kPortals.Exit:
                // caso seja o portal de saida
                break;
        }
    }

    // callbacks
    public void OnTriggerEnterCallBack(KLevelName level, KLevelName beacon)
    {
        // determina a qual dos niveis o portal deve levar
        if (game_state_.GetCurrentLevel == level || game_state_.GetCurrentLevel == beacon)
        {
            // deve teleportar para o nivel
            IEnumeratorCallBacks.Instance.LoadNewScene(game_state_.GetCurrentLevelSceneIndex());
        }
    }

    // corroutina para dar respawn ao jogador
    private IEnumerator RespawnPlayer()
    {
        //coloca o jogador na posiçao de spawn
        char_system_.RespawnPlayer(Spawner_point.position, Spawner_point.rotation);
        // define a posiçao e rotaçao da camera
        //cam_controller.SetCamTransfor(Spawner_point.position, Spawner_point.rotation);

        // aguarda 1s antes de continuar
        yield return new WaitForSeconds(1);

        // define a forma do jogador
        char_info.shape = PlayerShape.Aike;

        // repoem o tempo para o jogador
        char_system_.game_settings_.SetTimeMultiplier(1f);
    }
}
