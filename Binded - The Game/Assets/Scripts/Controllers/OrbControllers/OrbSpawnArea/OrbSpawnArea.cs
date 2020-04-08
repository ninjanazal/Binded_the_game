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
        // define a cor do gizmos para cyan
        Color gizmosColor = Color.cyan;
        // define uma transparencia para a cor
        gizmosColor.a = 0.4f;
        // aplica a cor determinada
        Gizmos.color = gizmosColor;
        // define a matriz de transformaçao do gizmos de acordo com o transform do objecto
        Gizmos.matrix = this.transform.localToWorldMatrix;
        // define o cubo
        Gizmos.DrawCube(Vector3.zero, area_size_);
    }
}
