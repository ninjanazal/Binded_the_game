using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    // define para onde o portal deve transportar
    public KLevelName Level;  // nome da scena para ler
    public KLevelName Beacon;   // nome do beacon que tambem pode levar
    public StateManager state_manager_; // referencia para o state manager
    public GameObject PortalEffectMesh; // mesh para o efeito do portal

    private void Awake()
    {
        // on awake, desativa o portal
        this.enabled = false;
    }

    // ao iniciar este objecto deve activar o collider associado
    private void OnEnable()
    {
        // caso exista uma box collider
        if (GetComponent<BoxCollider>())
        {
            GetComponent<BoxCollider>().isTrigger = true;        // define o collider como um trigger
            GetComponent<BoxCollider>().enabled = true;        // activa o collider
            if (PortalEffectMesh)
                PortalEffectMesh.SetActive(true);        // activa o mesh render
        }
    }

    // ao desativar deve desativar o collider
    private void OnDisable()
    {
        // caso exista uma box collider
        if (GetComponent<BoxCollider>())
        {
            GetComponent<BoxCollider>().enabled = false;            // desativa o colider 
            if (PortalEffectMesh)
                PortalEffectMesh.SetActive(false);            // desativa o efeito do portal
        }
    }

    // on trigger enter deve enviar para o stateManager a call de que o jogador entrou no trigger
    // e qual a scena a carregar
    private void OnTriggerEnter(Collider other) => state_manager_.OnTriggerEnterCallBack(Level, Beacon);

}
