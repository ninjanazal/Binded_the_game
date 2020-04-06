using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbTriggerManager : MonoBehaviour
{

    private ToPortalManager portal_manager_;    // referencia ao manager de portal

    // regista o portal
    public void RegistPortal(ToPortalManager manager) => portal_manager_ = manager;
    // remove o registo do portal
    public void UnregistPortal(ToPortalManager manager) { if (manager == portal_manager_) portal_manager_ = null; }

    // metodo utilizado para detectar se uma orb foi entregue
    private void OnTriggerEnter(Collider other)
    {
        //determina se uma orb entrou
        if (portal_manager_ && other.CompareTag("Orb"))
            portal_manager_.OnOrbEnterCallBack();   // chama o metodo para o callBack

    }
}
