using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTriggerManager : MonoBehaviour
{
    // referencia privada para receber o evento
    private ToPortalManager portal_manager_;    // referencia para o script mae
    private ParticleSystem.EmissionModule emission_module_; // referencia ao modulo de emissao do sistema
    private AudioSource beacon_audio_souce; // referencia para o source de som 

    private void Awake()
    {
        // guarda referencia para o modulo de emissao do sistema
        emission_module_ = GetComponent<ParticleSystem>().emission;
        // desativa a emissao
        emission_module_.enabled = false;

        // guarda referencia para o source de audio
        beacon_audio_souce = GetComponent<AudioSource>();
    }

    // funçao chamada para registar o portal
    public void RegistPortalManager(ToPortalManager manager)
    {
        // guarda o registo do portal
        portal_manager_ = manager;
        // ao registar activa o efeito de emissao
        emission_module_.enabled = true;
        // activa o efeito sonoro
        beacon_audio_souce.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        // caso este portal esteja connectado a um manager
        if (portal_manager_)
            // determina se colidio com o jogador
            if (other.CompareTag("Player")) portal_manager_.OnPlayerEnterCallBack();  // envia que o trigger foi activado pelo jogador
    }
}
