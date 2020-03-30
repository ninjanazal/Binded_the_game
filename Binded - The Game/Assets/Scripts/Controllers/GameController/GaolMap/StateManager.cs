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

    // on awake
    private void Awake()
    {
        // define a forma do jogador como Aike
        char_info.shape = PlayerShape.Aike;

        // inicia o jogador e a camera no mundo de acordo com o spawn point
        // inicia a camera 
        GameObject.Instantiate(PlayerObject, Spawner_point.position, Spawner_point.rotation);
        GameObject.Instantiate(CameraControllerObject, Spawner_point.position, Spawner_point.rotation);

        // determinar qual portal está aberto
        ConfirmPortals();
    }

    // metodo avalia qual o portal aberto
    private void ConfirmPortals()
    {
        //ao confirmar desativa todos os portais
        foreach (var portalTrigger in triggers){ portalTrigger.enabled = false; }
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
    public void OnTriggerEnterCallBack(string toLoad)
    {

    }
}
