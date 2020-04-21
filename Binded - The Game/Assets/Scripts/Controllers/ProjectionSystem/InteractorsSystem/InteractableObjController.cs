using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// enumerador para o tipo de interador
public enum kInteractorAxis { OnX, OnZ }

public class InteractableObjController : MonoBehaviour
{
    // controlador responsavel pelo movimento dos objectos controlaveis
    // variaveis publicas
    public InteractionObjsBehaviour[] interaction_objects_; // arraay com os controladores que este objecto tem
    public float rotation_velocity_;    // velocidade de rotaçao do objecto

    // Start is called before the first frame update
    void Start()
    {
        // ao iniciar, avalia se existem controladores
        if (interaction_objects_.Length > 0)
            // caso existam controladores, regista se nos objectos
            foreach (InteractionObjsBehaviour behaviour in interaction_objects_)
                // regista o controlador
                behaviour.RegistController(this);
    }

    //callBacks dos activadores
    public void ActivatorCallBack(kInteractorAxis axis)
    {
        // avalia qual é a influencia do controlador que activou
        switch (axis)
        {
            // caso o controlador tenha influencia sobre o x
            case kInteractorAxis.OnX:
                // caso influencie a rotaçao em x, roda sobre o eixo do objecto o um determinado valor dependendo da velocidade
                this.gameObject.transform.Rotate(Vector3.right, rotation_velocity_ * Time.deltaTime);
                break;
            case kInteractorAxis.OnZ:
                // caso influencie a rotaçao em x, roda sobre o eixo do objecto o um determinado valor dependendo da velocidade
                this.gameObject.transform.Rotate(Vector3.forward, rotation_velocity_ * Time.deltaTime);
                break;
        }
    }


}
