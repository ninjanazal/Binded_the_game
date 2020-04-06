using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToPortalManager : MonoBehaviour
{
    [Header("Variaveis do portal")]
    public KLevelName teleportTo;   // varialvel regista para que nivel o portal teleporta
    public LevelInfo level_infor_;  // informaçao do nivel em que o portal se encontra
    public int orbDelivered = 0;   // indica o valor de orbs recebidas

    // variaveis internas
    private bool isActivated = false;   // variavel interna para determinar se o protal está activo
    private OrbTriggerManager orb_trigger_manager_; // referencia para o manager de orbs


    // ao iniciar regista regista o portal de orbs
    private void Start()
    {
        // guarda referencia para o trigger manager de orbs
        orb_trigger_manager_ = GetComponentInChildren<OrbTriggerManager>();
        orb_trigger_manager_.RegistPortal(this);    // regista o portal
    }

    // metodo chamado quando o jogador entrou no portal
    public void OnPlayerEnterCallBack()
    {
        // ao ser chamado, deve iniciar a transiçao para a cena de teleportTO
        Debug.Log("PlayerEntered");
    }

    // metodo chamado quando uma orb entra no portal
    public void OnOrbEnterCallBack()
    {
        // ao entregar uma orb, incrementa o valor
        level_infor_.DeliveredEnergy++;
        orbDelivered = level_infor_.DeliveredEnergy;
        // determina se o valor de orbs entregues corresponde ao numero de activaçao
        isActivated = (orbDelivered >= level_infor_.EnergyRequired) ? ActivatePortal() : false;
    }

    // metodos internos
    private bool ActivatePortal()
    {
        // remove o registo para receber orbs
        orb_trigger_manager_.UnregistPortal(this);
        // metodo ao ser chamado deve activar o portal, para tal basta registar o script
        GetComponentInChildren<PortalTriggerManager>().RegistPortalManager(this);
        // retorna que o portal foi activado
        return true;
    }
}
