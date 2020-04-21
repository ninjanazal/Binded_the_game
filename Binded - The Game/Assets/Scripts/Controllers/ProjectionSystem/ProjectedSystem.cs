using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectedSystem : MonoBehaviour
{
    // class de projecçao, responsavel por definir superficies de projecçao 
    // variaveis publicas
    [Header("Informaçoes sobre a area de projecçao")]
    [Tooltip("Os valores devem sem iguais, a area deve ser sempre um cubo")]
    public Vector3 projected_area_; // tamanho da area de projecçao
    public GameObject obj_to_project_;  // objecto que vai ser projectado


    // variaveis privadas
    private ProjecterSystem projecter_system_;   // referencia ao sistema projector
    private float cell_size_;   // tamanho de cada uma das celulas
    private GameObject[,] projected_Objects_; // array bidimensional com os objectos projectados
    private float[,] projected_distances_;  // array bidimensional com as valiaveis de influencia

    // metodo responsavel por activar a area de projecçao
    public void ActivateProjectedArea(ProjecterSystem system, int sizeX, int sizeY)
    {
        // guarda referencia para o sistema de projecçao
        projecter_system_ = system;

        // define o tamanho dos arrays
        // array de objectos na area de projecçao
        projected_Objects_ = new GameObject[sizeX, sizeY];

        // array de distancias projectadas
        projected_distances_ = new float[sizeX, sizeY];

        // determinar o tamanho de cada um das celulas relativo á quantidade recebida
        cell_size_ = projected_area_.x / sizeX;

        // calcula e instancia os objectos
        StartCoroutine(CalculateCellPosition());
    }

    // metodo responsavel por actualizar as posiçoes dos objectos de projecçao
    public void UpdateProjectedDistances(float[,] distances)
    {
        // guarda as novas posiçoes no array bidimensional local
        // por segurança verifica se a dimensao do array nao foi alterado
        if (distances.Length == projected_distances_.Length)
            // guarda os valores
            projected_distances_ = distances;
    }

    #region private Ienumerators
    // ienumerator para determinar a posiçao de cada uma das celulas no mundo
    private IEnumerator CalculateCellPosition()
    {
        // precorre todos os elementos, determinando a posiçao no mundo
        // segundo a linha
        for (int x = 0; x < projected_Objects_.GetLength(0); x++)
        {
            // segundo a coluna
            for (int y = 0; y < projected_Objects_.GetLength(1); y++)
            {
                // atribui o objecto a instanciar
                projected_Objects_[x, y] = obj_to_project_;

                // determina posiçoes locais
                // como a projecçao ocorre na direcçao de -z para +z, entao
                projected_Objects_[x, y].transform.position = new Vector3((-projected_area_.x * 0.5f) + cell_size_ * 0.5f + cell_size_ * x,
                    -0.5f * projected_area_.y, (-projected_area_.z * 0.5f) + cell_size_ * 0.5f + cell_size_ * y);

                // transforma essas posiçoes para posiçoes no mundo
                projected_Objects_[x, y].transform.position = this.transform.TransformPoint(projected_Objects_[x, y].transform.position);

                // determina a rotaçao que cada um dos objectos deve tomas, para tal, o up do objecto deve ser igual ao do projector
                projected_Objects_[x, y].transform.up = this.transform.up;

                // instancia este objecto no mundo
                // dada a posiçao calculada e a "normal"
                projected_Objects_[x, y] = Instantiate(obj_to_project_,
                    projected_Objects_[x, y].transform.position, projected_Objects_[x, y].transform.rotation, this.transform);

                // define o tamanho do objecto para ser igual ao tamanho da celula
                projected_Objects_[x, y].transform.localScale = new Vector3(cell_size_, cell_size_, cell_size_);
            }

            // aguarda o proximo frame
            yield return null;
        }
        // terminar inicia o trabalho de deslocamento para as posiçoes determinadas dos objectos
        StartCoroutine(ProjectedUpdate());
    }

    // inumerator actualizador da posiçao da superficie projectada
    private IEnumerator ProjectedUpdate()
    {
        while (projecter_system_)
        {
            // precorre todos os elementos, actualizando a posiçao de cada uma das celulas
            //segundo a linha
            for (int x = 0; x < projected_Objects_.GetLength(0); x++)
            {
                // segundo a coluna
                for (int y = 0; y < projected_Objects_.GetLength(1); y++)
                {
                    // avalia se a raçao é diferente de 0
                    if (projected_distances_[x, y] != 0f)
                    {
                        // caso exista profundidade
                        // define a nova posiçao, relaçao local
                        projected_Objects_[x, y].transform.position = new Vector3((-projected_area_.x * 0.5f) + cell_size_ * 0.5f + cell_size_ * x,
                            -0.5f * projected_area_.y + (projected_area_.y * projected_distances_[x, y]),
                            (-projected_area_.z * 0.5f) + cell_size_ * 0.5f + cell_size_ * y);

                        // transforma a nova posiçao para posiçao
                        projected_Objects_[x, y].transform.position = this.transform.TransformPoint(projected_Objects_[x, y].transform.position);

                        // existindo profundidade, o objecto deve estar activo
                        projected_Objects_[x, y].SetActive(true);
                    }
                    else
                        // caso esteja marcado com 0 de profundidade, o objecto deve ser desativado
                        projected_Objects_[x, y].SetActive(false);
                }
                // aguarda o proximo frame
                yield return null;
            }
        }
    }
    #endregion
    // on Gizmos
    private void OnDrawGizmos()
    {
        // define a cor do gizmos
        Gizmos.color = Color.white;
        // define a matriz relativa ao objecto
        Gizmos.matrix = this.transform.localToWorldMatrix;
        // desenha a area de projecçao
        Gizmos.DrawWireCube(Vector3.zero, projected_area_);

        // define a area a partir da qual sao projectados
        // define a cor do gizmos
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(new Vector3(0f, -projected_area_.y * 0.5f, 0f), new Vector3(projected_area_.x, 0f, projected_area_.z));

    }
}
