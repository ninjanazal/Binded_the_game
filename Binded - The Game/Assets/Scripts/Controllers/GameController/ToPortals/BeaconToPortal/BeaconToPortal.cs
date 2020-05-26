using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeaconToPortal : MonoBehaviour
{
    // comportamento de portal nos beacons
    [Header("Informaçoes de jogo")]
    public GameState game_state_;   // referencia ao estado do jogo

    // variaveis internas
    private PlayBeaconController becon_controller_; // referencia ao controlador do beacon

    // referencia public para a activaçao do portal
    public void ActivatePortal(PlayBeaconController beaconController)
    {
        // guarda referencia para o controlador do beaon
        this.becon_controller_ = beaconController;
    }

    // callback para ontriggerEnter
    private void OnTriggerEnter(Collider other)
    {
        // determina se foi o jogador que entrou no portal
        if (other.CompareTag("Player"))
        {
            // caso o jogador tenha entrado no portal
            // define que o nivel foi concluido
            game_state_.CompletedLevel();
            //IEnumeratorCallBacks.Instance.LevelCompletedCallBack(game_state_);

            // inicia o callBack para alterar a cena
            // avalia se o portal actual não é o portal de saida
            if (game_state_.GetCurrentPortal != kPortals.Exit)
                IEnumeratorCallBacks.Instance.LoadNewScene((int)(KLevelName.Gaol));
            else
            {
                // define o nivel inicial 
                game_state_.SetCurrentLevel = KLevelName.Hamr;
                // transita para a cutScen de saida
                IEnumeratorCallBacks.Instance.LoadNewScene((int)(KLevelName.ExitCutScene));                
            }
        }
    }
}
