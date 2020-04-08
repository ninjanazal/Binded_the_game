using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbBehaviour : MonoBehaviour
{
    // variaveis publicas
    [Header("Variaveis da orb")]
    [Range(0f, 1f)] public float minRangeInfluence = 0.5f;    // a razao comparada ao range de observaçao
    [Range(0f, 1f)] public float orbPositionOffSetInfluence = 0.5f;    // distancia que a orb deve guardar
    public float moveToSpeed = 5f;      // velocidade da orb aquando nao esta em followMode
    public float orbitSpeed = 10f;  // valor da velocidade de orbitra
    [Header("objecto de render da orb")]
    public GameObject orb_renderer_;    // referencia para o renderer da particula

    // variaveis privadas
    private OrbProximityController orb_proximity_;  // referencia ao controlador de proximidade
    private Transform player_transform_;     // referencia para o transform passado por paramentro pelos metodos
    private bool in_follow_mode_ = false;   // variavel que indica em que estado a orb está
    private float min_range_val;    // valor do trigger de follow
    private float distance_Offset;  // distancia que a orb deve guardar do alvo relativa com a distancia minima
    private Vector3 calculater_dir_pos;     // vector que determina a posiçao da orb relativa ao jogador

    private OrbSpawnerManager spawn_manager;    // referencia ao spawnManager
    private GameObject particle_object; // reference parao objecto que contem o sistema de particulas
    private ParticleSystem orb_particle_system_;    // referencia para o sistema de particulas

    // Start is called before the first frame update
    void Start()
    {
        // ao iniciar, regista o trigger da orbbb
        orb_proximity_ = GetComponentInChildren<OrbProximityController>();
        orb_proximity_.RegistOrbTriggerCallBacks(this);

        // guarda o valor do alcance relativo ao valor de influencia
        min_range_val = orb_proximity_.GetColliderRange() * minRangeInfluence;
        // determina a distancia que deve ser mantida de acordo com a influencia do offset
        distance_Offset = min_range_val * orbPositionOffSetInfluence;

        // guarda referencia para o sistema de particulas
        orb_particle_system_ = GetComponentInChildren<ParticleSystem>();
        // guarda referencia para objecto que contem o sistema de particulas
        particle_object = orb_particle_system_.gameObject;

    }

    // Update is called once per frame
    void Update()
    {
        // todos os frams, verifica se existe um jogador como alvo
        if (player_transform_)
        {
            // determina a distancia do jogador
            float player_Distance = Vector3.Distance(this.transform.position, player_transform_.position + (calculater_dir_pos * distance_Offset));
            // caso exista, verifica a distancia do jogador em caso da existencia de antecipaçao em funçao do trigger
            if (player_Distance > orb_proximity_.GetColliderRange())
            {
                // caso a distancia seja maior que o valor do trigger, remove o jogador
                OrbTriggerExitCallBack();
                // abandona o ciclo
                return;
            }
            else
                // caso contrario avalia a distancia do jogador para determinar o tipo de comportamento
                OrbMove(player_Distance);
        }
    }


    // metodos internos
    private void OrbMove(float playerDistance)
    {
        // aponta a orb para a posiçao do jogador
        this.transform.LookAt(player_transform_.position);
        // desloca a orb na direçao do jogador a uma velocidade dependendo do estado
        this.transform.position = Vector3.Lerp(this.transform.position, player_transform_.position + (calculater_dir_pos * distance_Offset),
        moveToSpeed * Time.deltaTime);
    }


    #region TriggerCallBacks
    // trigger callback
    public void OrbTriggerCallBack(Transform playerTransform)
    {
        // ao registar que o jogador entrou no campo de observaçao da orb, guarda o transform do jogador
        player_transform_ = playerTransform;
        // determina o vector de direcçao da orb
        calculater_dir_pos = (this.transform.position - player_transform_.position).normalized;

        // muda o tamanho ao entrar em contacto com o jogador
        orb_renderer_.transform.localScale /= 3f;

        // define o tamanho de inicio da particula
        ParticleSystem.MainModule main = orb_particle_system_.main;
        main.startSize = 0.5f;

        // ao entrar na proximidade do jogador muda a layer para nao ser afetada pela visao
        particle_object.layer = 0;

    }

    // callback para remover o jogador de observado
    public void OrbTriggerExitCallBack()
    {
        // caso o jogador saia do campo de observaçao, remove o transform e verifica o estado da orb
        player_transform_ = null;
        in_follow_mode_ = false;
        // muda o tamanho ao entrar em contacto com o jogador
        orb_renderer_.transform.localScale *= 3f;

        // define o tamanho de inicio da particula
        ParticleSystem.MainModule main = orb_particle_system_.main;
        main.startSize = 2;

        // ao entrar na proximidade do jogador muda a layer para  ser afetada pela visao
        particle_object.layer = 8;

    }
    #endregion

    #region ActionsCallBacks
    // registar na orb a referencia para o spawnManager
    public void RegistSpawnManager(OrbSpawnerManager manager) => spawn_manager = manager;

    // call para destruir a orb sem respawn
    public void DestroyNorespawn() => Destroy(this.gameObject);
    // call para destruir com novo spawn
    public void DestroyAndRespawn()
    {
        // avalia se existe o spawnManager, apenas por conveniencia
        if (spawn_manager)
            spawn_manager.SpawnNewOrb();

        // destroi a orb actual
        Destroy(this.gameObject);
    }
    // call retorna se a orb tem o jogador como follow target ou nao
    public bool HasPlayerMarked() { return (player_transform_) ? true : false; }

    #endregion

    // debug
    // desenha no gismos
    private void OnDrawGizmos()
    {
        // alcance minimo
        // em cor azul
        Gizmos.color = Color.blue;
        // desenha a esfera
        Gizmos.DrawWireSphere(this.transform.position, GetComponentInChildren<OrbProximityController>().GetColliderRange() * minRangeInfluence);
    }

}
