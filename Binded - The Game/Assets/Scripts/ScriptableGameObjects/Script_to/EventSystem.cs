using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Event", menuName = "Binded/Event")]
public class EventSystem : ScriptableObject
{
    // lista de listenners
    private List<GameEventListenner> listenners =
            new List<GameEventListenner>();

    // elevar o evento
    public void Raise()
    {
        foreach (GameEventListenner listenner in listenners)
            listenner.OnEventRaised();
    }
    // registar um listenner ao evento
    public void RegisteListenner(GameEventListenner listenner) => listenners.Add(listenner);
    // remover um listenner do evento
    public void UnRegisteListenner(GameEventListenner listenner)
    {
        // confirma se o listenner existe na lista
        if (listenners.Contains(listenner))
            //se sim remove da lista
            listenners.Remove(listenner);
    }


}
