using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FenrirArmController : MonoBehaviour
{
    // comportamento para o braço de fenrir
    [Header("Valores de comportamento")]
    public float arm_aceleration_ = 10f;  //valor de aceleraçao do braço
    public float turning_speed_ = 5f;    // valor de velocidade de rotaçao
    public float arm_max_life_time; // tempo de vida do braço
    public float arm_velocity_;    // velocidade do braço
    public float arm_radius_;  // raio do braço

    // variaveis internas
    private Transform target_position_;    // posiçao do alvo
    private bool activated_ = false; // variavel de activaçao do comportamento

    // variaveis relacionadas com o deslocamento
    private Vector3 next_position_; // proxima posiçao que o braço vai tomar
    private Ray move_ray;   // ray de teste do deslocamento
    private RaycastHit move_ray_hit;    // hit do ray

    private TrailRenderer trail_renderer_;  // referencia ao trail do braço
    private void Update()
    {
        // caso o braço nao esteja activo
        if (activated_)
        {
            // determina a direcçao alvo
            Vector3 target_direction = target_position_.position - this.transform.position;

            // deve rodar a direcçao do braço na direcçao do target
            this.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(this.transform.forward,
                target_direction, turning_speed_ * Time.deltaTime, 0f));

            // aumenta a velocidade
            arm_velocity_ += arm_aceleration_ * Time.deltaTime;

            // com o objecto rodado, desloca na direcçao
            next_position_ = Vector3.Lerp(this.transform.position, this.transform.position + (this.transform.forward *
                arm_velocity_), arm_velocity_ * Time.deltaTime);

            // avalia o deslocamento
            LastMoveAvaliation();

            // define a posiçao do braço
            this.transform.position = next_position_;
        }
    }

    // metodos publicos
    // activaçao do braço
    public void ArmActivation(Transform target, Vector3 startForward)
    {
        // define o alvo do braço
        target_position_ = target;
        // aplica a direcçao definida
        this.transform.forward = startForward;

        // activa o braço
        activated_ = true;
        // guarda referencia para o trai do braço
        trail_renderer_ = GetComponent<TrailRenderer>();
        
        // ao activar o braço, é definida uma call para destroir ao fim do tempo maximo
        Destroy(this.gameObject, arm_max_life_time);

    }

    // metodos privador
    private void LastMoveAvaliation()
    {
        // determina se ao longo do deslocamento colide com algo relevante
        move_ray = new Ray(this.transform.position, next_position_ - this.transform.position);

        // avalia a colisao do ray
        if (Physics.SphereCast(move_ray, arm_radius_, out move_ray_hit, Vector3.Distance(this.transform.position, next_position_)))
        {
            // Determina se o ray atingiu uma orb 
            if (move_ray_hit.collider.CompareTag("Orb"))
            {
                // indica á orb para dar respawn novamente, tenta obter o controlador
                if (move_ray_hit.transform.GetComponent<OrbBehaviour>())
                    // se existir um controlador, ordena o respawn
                    move_ray_hit.transform.GetComponent<OrbBehaviour>().DestroyAndRespawn();
            }
            else if (move_ray_hit.transform.CompareTag("Player"))
            {
                // caso tenha atingido o jogador
                // avalia se existe um controlador do personagem
                if (move_ray_hit.transform.GetComponent<CharacterSystem>())
                    //    // caso exista, mata o jogador
                    move_ray_hit.transform.GetComponent<CharacterSystem>().KillPlayer();
            }
            // desativa o deslocamento do raio
            activated_ = false;
        }
    }
}
