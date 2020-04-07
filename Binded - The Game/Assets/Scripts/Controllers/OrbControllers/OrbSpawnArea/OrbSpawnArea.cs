using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbSpawnArea : MonoBehaviour
{
    // controlador para a area de spawn
    [Header("Informaçoes da Area")]
    public Vector3 area_size_;  // referencia para a area de spawn

    // metodos publicos
    public Vector3 GetRandomPosInside()
    {
        // determina um valor aleatorio dentro da area local
        var returnPosition = new Vector3((Random.value - 0.5f) * area_size_.x,
            (Random.value - 0.5f) * area_size_.y,
            (Random.value - 0.5f) * area_size_.z);
        // transforma essa posiçao para posiçoes no mundo de acordo com a matriz do objecto
        returnPosition = transform.TransformPoint(returnPosition);

        // retorna a posiçao determinada
        return returnPosition;
    }



    private void OnDrawGizmos()
    {
        // define a cor do gizmos para branca
        Gizmos.color = Color.white;

        Gizmos.matrix = this.transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, area_size_);
    }
}
