using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionObjsBehaviour : MonoBehaviour
{
    // variaveis publicas
    public kInteractorAxis rotate_Axis_;    // vector de rotaçao que este controlador controla

    // variaveis privadas
    private List<InteractableObjController> interactable_Controllers_ = new List<InteractableObjController>(); // referencia para os controladores que este está associado

    // metodo chamado para registar o controlador para receber chamadas de activaçao
    public void RegistController(InteractableObjController controller)
    {
        // ao ser registado, verifica se o controlador já existe
        if (interactable_Controllers_.Count == 0 || !interactable_Controllers_.Contains(controller))
            // se nao existir regista o cotnrollador
            interactable_Controllers_.Add(controller);
    }

    // assim que um jogador entrar no trigger do activador
    private void OnTriggerStay(Collider other)
    {
        // confirma se o objecto que entrou é o jogador
        if (other.gameObject.CompareTag("Player"))
        {
            // chama em cada um dos controladores registados o callBack
            foreach (var controller in interactable_Controllers_)
            {
                // chama o callBack para  a rotaçao do objecto controlado
                controller.ActivatorCallBack(rotate_Axis_);
            }
        }
    }
}
