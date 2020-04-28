using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjecterSystem : MonoBehaviour
{
    [Header("Informaçoes do projector")]
    [Tooltip("Os valores devem sem iguais, a area deve ser sempre um cubo")]
    public Vector3 projecter_area_; // tamanho da area projector
    public ProjectedSystem[] project_to_; // referencia para os objectos para que este projector envia

    [Header("Tamanho da celula de projector")]
    public float projecter_cell_size_;    // tamanho da celula de projecçao

    #region private Vars
    // variaveis privadas
    private Vector3[,] projecter_cell_position;  // posiçao de cada uma das celulas de projecçao
    private float[,] projected_distance;    // distancia que se encontra do projector

    // variaveis de comportamento interno
    private Ray cell_test_ray_; // raio de teste para o projector
    private RaycastHit ray_hit; // resultado obtido do hit de cast
    #endregion


    private void Start()
    {
        // ao iniciar deve determinar o numero de celulas de acordo com o tamanho de cada uma
        projecter_cell_position = new Vector3[(int)(projecter_area_.x / projecter_cell_size_),
            (int)(projecter_area_.y / projecter_cell_size_)];

        // define o tamanho do array bidimensional dos resultados
        projected_distance = new float[projecter_cell_position.GetLength(0), projecter_cell_position.GetLength(1)];

        // define a posiçao para cada uma das celulas
        StartCoroutine(CalculateCellPositions());
    }

    // metodos privados
    private void UpdateProjecteds()
    {
        // caso existam areas de projeçao
        if (project_to_.Length > 0)
        {
            // actualiza a informaçao
            foreach (ProjectedSystem projected_system in project_to_)            
                // actualiza os valores de projecçao
                projected_system.UpdateProjectedDistances(projected_distance);
            
        }
    }

    #region Metodos privados
    // ienumeradores privados
    private IEnumerator CalculateCellPositions()
    {
        // precorre a lista de celulas e atribui a posiçao para cada uma delas
        // segundo a coluna
        for (int x = 0; x < projecter_cell_position.GetLength(0); x++)
        {
            // segundo a linha
            for (int y = 0; y < projecter_cell_position.GetLength(1); y++)
            {
                // define a posiçao da celula
                // posiçao local 
                projecter_cell_position[x, y] = new Vector3((-projecter_area_.x * 0.5f) + projecter_cell_size_ * 0.5f + projecter_cell_size_ * x,
                    (-projecter_area_.y * 0.5f) + projecter_cell_size_ * 0.5f + projecter_cell_size_ * y, -0.5f * projecter_area_.z);

                // calcula a posiçao no mundo relativa ao objecto
                projecter_cell_position[x, y] = this.transform.TransformPoint(projecter_cell_position[x, y]);
            }
            // aguarda o proximo frame
            yield return null;
        }
        //degub
        Debug.Log($"{projecter_cell_position.Length} Calculated positions");

        // ao terminar de calcular as posiçoes para cada uma das celulas
        // activa os elementos projectores
        foreach (ProjectedSystem projected_system in project_to_)
            // inicia estas areas
            projected_system.ActivateProjectedArea(this, projecter_cell_position.GetLength(0), projecter_cell_position.GetLength(1));

        // inicia a projecçao
        StartCoroutine(ProjectionActivity());

    }


    // ienumerador da actividade do objecto, ao longo de tempo recorre a rays para determinar a distancia 
    private IEnumerator ProjectionActivity()
    {
        while (true)
        {
            // precorre todas as celulas, emitindo um ray com a distancia maxima da profundidade da area de projecçao
            // segundo a coluna
            for (int x = 0; x < projecter_cell_position.GetLength(0); x++)
            {
                // segundo a linha
                for (int y = 0; y < projecter_cell_position.GetLength(1); y++)
                {
                    // define o ray
                    cell_test_ray_ = new Ray(projecter_cell_position[x, y], this.transform.forward * projecter_area_.z);

                    // lança um ray da posiçao da celula com a distancia maxima
                    if (Physics.Raycast(cell_test_ray_, out ray_hit, projecter_area_.z))
                    {
                        // caso exista um contacto, determina se foi uma colisao valida
                        if (ray_hit.transform.CompareTag("ProjecterTarget"))
                            // caso tenha detectado a existencia do objecto na area, determina a distancia relativa á area
                            // obtendo assim valores entre 0 (sobre a superficie emissora) e 1 (distancia maxima)
                            projected_distance[x, y] = (Vector3.Distance(projecter_cell_position[x, y], ray_hit.point)) / projecter_area_.z;

                    }
                    else
                        // caso nao tenha existido nenhuma colisao, o valor a retornar é maximo
                        projected_distance[x, y] = 0f;

                }
                // ciclo aguarda ao finaliza cada uma das linhas
                // aguarda o proximo frame
                yield return null;
            }
            // assim que todas as linhas tenham sido verificas actualiza as profundidades
            UpdateProjecteds();
        }

    }
    #endregion



    // gizmos de debug
    private void OnDrawGizmos()
    {
        // define a cor da area que projecta
        Gizmos.color = Color.green;
        // define a matriz de transformaçao
        Gizmos.matrix = this.transform.localToWorldMatrix;
        // define a area de projecçao
        Gizmos.DrawWireCube(Vector3.zero, projecter_area_);

        // face onde começa a projecçao
        // define a area em que a projecçao é iniciada
        Gizmos.color = Color.blue;
        // difine o plano de projecçao
        Gizmos.DrawCube(new Vector3(0f, 0f, 0f - projecter_area_.z * 0.5f),
            new Vector3(projecter_area_.x, projecter_area_.y, 0f));

        // face onde termina a projecçao maxima
        Gizmos.color = Color.red;
        //define o plano de fim da projecçao
        Gizmos.DrawCube(new Vector3(0f, 0f, 0f + projecter_area_.z * 0.5f),
            new Vector3(projecter_area_.x, projecter_area_.y, 0f));

        //// desenha linhas da posiçao das celulas
        //if (projecter_cell_position != null)
        //    foreach (Vector3 cell in projecter_cell_position)
        //    {
        //        // define a cor para o gizmos
        //        Gizmos.color = Color.gray;
        //        Gizmos.DrawLine(cell, cell + Vector3.forward * projecter_area_.z);
        //    }
    }
}
