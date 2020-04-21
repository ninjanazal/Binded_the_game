using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    public Transform player;
    public float maxAngle;
    public float maxRadius;

    private bool isInFov = false;

    private void OnDrawGizmos()
    {
        if (player)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, maxRadius); //desenho da capsula de detecçao

            Vector3 fovLine1 = Quaternion.AngleAxis(maxAngle, transform.up) * transform.forward * maxRadius;
            Vector3 fovLine2 = Quaternion.AngleAxis(-maxAngle, transform.up) * transform.forward * maxRadius;
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, fovLine1); //linha 1
            Gizmos.DrawRay(transform.position, fovLine2); //linha 2

            if (!isInFov)
                Gizmos.color = Color.red;
            else
                Gizmos.color = Color.green;


            Gizmos.DrawRay(transform.position, (player.position - transform.position).normalized * maxRadius); //ray para o player

            Gizmos.color = Color.black;
            Gizmos.DrawRay(transform.position, transform.forward * maxRadius); //ray visao frontal
        }
    }

    private bool InFov(Transform checkingObject, Transform target, float maxAngle, float maxRadius)
    {
        Collider[] overlaps = new Collider[10];
        int count = Physics.OverlapSphereNonAlloc(checkingObject.position, maxRadius, overlaps);

        for (int i = 0; i < count - 1; i++)
        {
            if (overlaps[i] != null)
            {
                if (overlaps[i].transform == target)
                {
                    Vector3 directionBetween = (target.position - checkingObject.position).normalized;
                    directionBetween.y *= 0;

                    float angle = Vector3.Angle(checkingObject.forward, directionBetween);
                    if (angle <= maxAngle)
                    {
                        Ray ray = new Ray(checkingObject.position, target.position - checkingObject.position);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit, maxRadius))
                        {
                            if (hit.transform == target)
                                return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    private void Update()
    {
        if (player)
            isInFov = InFov(transform, player, maxAngle, maxRadius);
    }
}
