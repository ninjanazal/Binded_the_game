using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FenrirBehaviour : MonoBehaviour
{
    // controlador de comportamento de Fenrir
    // variaveis publicas
    [Header("Informaçoes de FenRir")]
    public GameObject fenrir_arm_;  // referencia para o GO a instanciar
    public int fenrir_arm_count_;   // numero de braços que o fenrir tem
    public float look_for_interval = 2f; // intervalo entre looks do Fenrir
    public LayerMask layer_mask;    // variavel que determina em que layer deve o ray detectar

    // variaveis privadas
    private Transform player_transform_; // referencia para o transform do jogador
    private Transform fenrir_transform_;    // referencia para o transform do fenrir 

    // variaveis de comportamento
    private Vector3 player_seen_position;   // referencia para a posiçao do jogador em vista
    private Ray fenrir_ray_;    // raio lançado pelo fenrir para determinar se o player está em vista
    private RaycastHit ray_hit_;    // variavel de informaçao sobre o resultado do ray


    // metodo chamado para activar o Fenrir
    public void ActivateFenrir(Transform playerTransform)
    {
        // ao ser chamado guarda a referencia ao transform do jogador
        player_transform_ = playerTransform;

        // guarda a referencia para o transform local, maior commudidade
        fenrir_transform_ = this.transform;
        // inicia o processo de actividade do Fenrir
        StartCoroutine(FenrirActivity());
    }

    #region Metodos privados
    // metodo privado para determinar se o player está em visao
    private bool InVisionCheck()
    {
        // cria o ray a ser lançado
        fenrir_ray_ = new Ray(player_transform_.position, (fenrir_transform_.position - player_transform_.position).normalized);
        // lança um ray na direcçao do jogador
        if (!Physics.Raycast(fenrir_ray_, out ray_hit_, Vector3.Distance(fenrir_transform_.position, player_transform_.position),
            layer_mask, QueryTriggerInteraction.Ignore))
            // caso nao exista nada entre os dois elementos, o jogador está em vista
            return true;


        // caso nao exista um contacto
        return false;
    }

    // ienumerator para o scan de look do fenrir
    private IEnumerator FenrirActivity()
    {
        // enquanto existir a indicaçao da existencia do jogador
        while (player_transform_)
        {
            // chama a funçao que avalia se o jogador está em vista
            if (InVisionCheck())
            {
                // guarda a posiçao de hit
                player_seen_position = player_transform_.position;
                // deve ser instanciado o numero de braços definidos
                // inicia a corroutina e aguarda a conclusao da mesma
                yield return StartCoroutine(FenrirArmSpawner());
            }

            // aguarda o proximo frame
            yield return new WaitForSeconds(look_for_interval);
        }
    }

    // Ienumerator para a instanciaçao dos braços
    private IEnumerator FenrirArmSpawner()
    {
        // corre um determinado numero de vezes de acordo com o numero de braços
        for (int i = 0; i < fenrir_arm_count_; i++)
        {
            // determinar uma direcçao para instanciar o braço
            Vector3 armStartDirection =
                new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));

            // instancia o branço na posiçao do fenrir
            FenrirArmController arm = Instantiate(fenrir_arm_, this.transform.position, Quaternion.identity).
                GetComponent<FenrirArmController>();

            // Define a direcçao do braço e activa
            arm.ArmActivation(player_transform_, armStartDirection);

            // aguarda o proximo frame
            yield return null;
        }
    }
    #endregion

    // gizmos para degu
    private void OnDrawGizmos()
    {
        // define a cor vermelho para o gizmos
        Gizmos.color = Color.red;
        // define uma esfera no ultimo ponto que que o jogador foi visto
        Gizmos.DrawSphere(player_seen_position, 1f);
    }
}
