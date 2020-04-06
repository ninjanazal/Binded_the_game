using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPortalManager : MonoBehaviour
{
    // referencias privadas
    private PlayGameManager game_manager_;

    // regista o manager para receber callbacks
    public void RegistManager(PlayGameManager manager) => game_manager_ = manager;
    // remove o registo para receber callbacks
    public void UnRegistManager(PlayGameManager manager) { if (manager == game_manager_) game_manager_ = null; }



    private void OnTriggerEnter(Collider other)
    {
        // determina se o trigger está activo ou nao e se foi o jogador que entrou
        if (game_manager_ && other.CompareTag("Player"))
            // caso sim, chama o callback do trigger
            game_manager_.RespawnCallBack();
    }

}
