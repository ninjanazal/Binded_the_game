using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListenner : MonoBehaviour
{
    // Evento a ser esperado
    [Header("Evento que deve esperar")]
    public EventSystem Event;
    // Resposta ao evento
    public UnityEvent Response;

    // ao activar, inscreve
    private void onEnable() => Event.RegisteListenner(this);
    // ao desactivar, remove o registo
    private void onDisable() => Event.UnRegisteListenner(this);
    // ao ser destruido
    void OnDestroy() => Event.UnRegisteListenner(this);
    // ao ser chamado
    public void OnEventRaised() => Response.Invoke();
}
