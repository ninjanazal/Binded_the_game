using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbProximityController : MonoBehaviour
{
    private OrbBehaviour orb_behaviour; // referencia para o comportamento da orb

    // regista o comportamento da orb no trigger
    public void RegistOrbTriggerCallBacks(OrbBehaviour behaviour) => orb_behaviour = behaviour;

    // responsavel por detectar se na proximidade existe um jogador
    private void OnTriggerEnter(Collider other)
    {
        // determina se foi um jogador e se a orb está registada
        if (orb_behaviour && other.CompareTag("Player"))
            // caso o jogador tenha entrado no campo da orb, retorna o transform
            orb_behaviour.OrbTriggerCallBack(other.transform);
    }

    // responsavel por remover o jogador do campo de observaçao da orb
    private void OnTriggerExit(Collider other)
    {
        // determina se foi o jogador que abandonou a area e se a orb está registada
        if (orb_behaviour && other.CompareTag("Player"))
            // caso o jogador tenha abandonado o campo, remove o jogador de observado
            orb_behaviour.OrbTriggerExitCallBack();
    }

    // metodos publicos para obtençao de informaçao
    public float GetColliderRange() { return GetComponent<SphereCollider>().radius; }

}
