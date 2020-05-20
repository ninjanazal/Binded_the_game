using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInController : MonoBehaviour
{
    // Metodo chamado para responder ao sinal de fadeIn_end
    public void FadeInEndHandler()
    {
        // ao receber o sinal deve destroir o objecto
        Object.DestroyImmediate(this.gameObject);
    }
}
