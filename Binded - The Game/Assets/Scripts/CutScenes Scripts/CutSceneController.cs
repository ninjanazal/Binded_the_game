using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneController : MonoBehaviour
{
    // indica para que scene deve transitar
    public KLevelName load_to;      // indica para que scene deve dar load

    // handler para o final da cutScene
    public void CutSceneEndHandler()
    {
        // ao terminar chama a o gestor de inumerados para iniciar o carregamento da nova scene
        IEnumeratorCallBacks.Instance.LoadNewScene((int)load_to);
    }

    // handler para o final da cutScene, loader interno
    public void LoadSceneAndWait()
    {
        // TODO, carregamente a par da cena
    }
}
