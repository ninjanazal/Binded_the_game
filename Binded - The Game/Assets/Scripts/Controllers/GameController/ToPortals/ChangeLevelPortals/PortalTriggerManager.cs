using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTriggerManager : MonoBehaviour
{
    // referencia privada para receber o evento
    private ToPortalManager portal_manager_;    // referencia para o script mae

    // funçao chamada para registar o portal
    public void RegistPortalManager(ToPortalManager manager){portal_manager_ = manager;}

    private void OnTriggerEnter(Collider other)
    {
        // caso este portal esteja connectado a um manager
        if (portal_manager_)
            // determina se colidio com o jogador
            if (other.CompareTag("Player")) portal_manager_.OnPlayerEnterCallBack();  // envia que o trigger foi activado pelo jogador
    }
}
