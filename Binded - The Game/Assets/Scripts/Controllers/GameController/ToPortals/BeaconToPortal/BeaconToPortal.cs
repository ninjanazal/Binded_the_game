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
            // inicia o callBack para alterar a cena
            IEnumeratorCallBacks.Instance.LoadNewScene((int)(KLevelName.Gaol));
        }
    }
}
