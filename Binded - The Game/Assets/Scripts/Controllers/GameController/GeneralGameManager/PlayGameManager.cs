using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayGameManager : MonoBehaviour
{
    //variaveis publicas
    [Header("Variaveis de sistema")]
    public CharacterInfo char_info_;    // referencia para as informaçoes do jogador

    [Header("Referencias á jogabilidade")]
    public Transform StartSpawn;    // referencia para o ponto de spawn inicial
    public Transform DeathSpawnPoint;  // referencia para o ponto de spawn quando morto
    public Transform[] SpawnPoints; // pontos de spawn no mapa
    public GameObject PlayerObject; // prefab do jogador
    public GameObject CameraControllerObject;   // referencia para o controlador da camera

    [Header("Referencia para actividades")]
    public RespawnPortalManager respawn_portal_manager; // referencia para o portal de respawn

    // variaveis internas
    // referencia para elementos do jogador
    private CharacterSystem char_system_;   // referencia para o controlador do jogador
    private Transform char_transform_;    // referencia para o transform do jogador
    private CameraController cam_controller_;   // referencia para o controlador da camera
    // ao acordar
    private void Start()
    {
        // inicia o jogador
        InicializePlayer();

    }

    // Update is called once per frame
    void Update()
    {
        // avalia se o jogador está morto
        if (!char_system_.GetPlayerState())
        {
            // regista-se para receber chamadas de callback
            respawn_portal_manager.RegistManager(this);

            // caso o jogador esteja mordo, deve ser colocado na area de respawn
            StartCoroutine(SetPlayerOnRespawn());
        }
    }



    #region Metodos privados
    // metodos privados
    // inicializar o jogador
    private void InicializePlayer()
    {
        // define a forma do jogador para Aike
        char_info_.shape = PlayerShape.Aike;

        // instancia o jogador e a camera no mundo de acordo com o primeiro spawnPoint
        // guarda a refenrecia para o controlador ao instanciar
        char_system_ = GameObject.Instantiate(PlayerObject, StartSpawn.position, StartSpawn.rotation).
            GetComponent<CharacterSystem>();
        // guarda tambem a referencia para o transform do jogador
        char_transform_ = char_system_.transform;

        // define valores iniciais para o estado do jogador para evitar que seja transportado de outras cenas
        char_system_.CanGoArif(true);
        // torna o jogador activo tambem
        char_system_.RespawnPlayer();
        // confirma o valor da escala de tempo
        char_system_.game_settings_.SetTimeMultiplier(1f);

        // inicia a camera, guardando referencia para o controlador da camera
        cam_controller_ = GameObject.Instantiate(CameraControllerObject, StartSpawn.position, StartSpawn.rotation).
            GetComponent<CameraController>();
    }


    #endregion

    #region Metodos publicos
    // callback chamado para por de novo o jogador no mapa
    public void RespawnCallBack()
    {
        // remove o registo 
        respawn_portal_manager.UnRegistManager(this);

        // para o respawn, escolhe um ponto de spawn aleatorio dos pontos fornecidos
        // define a posiçao para o jogador com base no valor
        // determina um valor aleatorio dentro do alcançe para os pontos fornecidos
        var randomVal = Random.Range(0, SpawnPoints.Length - 1);

        // com base no valor determinado, poem o jogador na posiçao
        char_system_.RespawnPlayer(SpawnPoints[randomVal].position, SpawnPoints[randomVal].rotation);
        // define a posiçao da camera
        cam_controller_.SetCamTransfor(SpawnPoints[randomVal].position, SpawnPoints[randomVal].rotation);

        // define que o jogador pode alterar entre as formas
        char_system_.CanGoArif(true);
        // confirma a escala do tempo para o jogador
        char_system_.game_settings_.SetTimeMultiplier(1f);
        
    }
    #endregion

    #region Routines
    // definir o jogador para a sala de respanw
    private IEnumerator SetPlayerOnRespawn()
    {
        // deve colocar o jogador e a camera no campo de respawn
        // define o respaw do jogador na posiçao
        char_system_.RespawnPlayer(DeathSpawnPoint.position, DeathSpawnPoint.rotation);
        // define a posiçao e rotaçao da camera
        cam_controller_.SetCamTransfor(DeathSpawnPoint.position, Quaternion.identity);
        // aguarda 1s antes de continuar
        yield return new WaitForSeconds(1);

        // desativa a possibilidade de alteral de forma
        char_system_.CanGoArif(false);
        // define a forma do jogador como Aike
        char_info_.shape = PlayerShape.Aike;

        // confirma o multiplicador de tempo
        char_system_.game_settings_.SetTimeMultiplier(1f);
    }
    #endregion
}
