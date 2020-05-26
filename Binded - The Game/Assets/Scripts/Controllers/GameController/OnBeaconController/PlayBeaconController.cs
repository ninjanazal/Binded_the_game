using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBeaconController : MonoBehaviour
{
    // gestor de comportamento para beacons
    // estado do jogo
    [Header("Informaçoes de comportamento")]
    public CharacterInfo char_infor_;   // referencia ao container de informaçoes do jogador
    public Transform spawn_position_; //posiçao de spawn do jogador

    // referencia para instanciaçao do jogador
    [Header("Informaçoes de jogador")]
    public GameObject player_obj_;   // referencia ao prefab do jogador
    public GameObject camera_obj_;  // referencia ao prefab da camera


    // variaveis privadas
    private CharacterSystem char_system_;   // sistema de controlo do personagem
    private CameraController cam_controller_;   // referencia ao controlador da camera

    private void Start()
    {
        // ao iniciar instancia o jogador
        InstantiatePlayer();
    }

    private void Update()
    {
        // no update é confirmado apenas o estado do jogador, para call de respawn
        if (!char_system_.GetPlayerState())
            // quando o jogador morrer, é despertado o respawncall
            RespawncallBack();
    }

    // metodos privado
    // iniciar o jogador no mundo
    private void InstantiatePlayer()
    {
        // define a forma para o jogador
        char_infor_.shape = PlayerShape.Aike;

        // instancia o jogador e a camera no mundo de acordo com o spawnPoint
        // guarda a referencia para o controlador ao iinstanciar
        char_system_ = GameObject.Instantiate(player_obj_, spawn_position_.position, Quaternion.identity).
            GetComponent<CharacterSystem>();

        // define que o jogador nao pode trocar de forma
        char_system_.CanGoArif(false);
        // confirma que o jogador se encontra activo, nao definindo uma posiçao
        char_system_.RespawnPlayer();
        // confirma tambem o valor de escala de tempo para o jogador
        char_system_.game_settings_.SetTimeMultiplier(1f);

        // inica a camera, guardando referencia para o controlador da mesma
        cam_controller_ = GameObject.Instantiate(camera_obj_, spawn_position_.position, Quaternion.identity).
            GetComponent<CameraController>();

        // define o volume dos listener
        char_system_.game_settings_.SetListenerVolume();
    }

    // metodo privado que define o respawn do jogador
    private void RespawncallBack()
    {
        // ao morrer, é confirmado a escala de tempo para o jogador
        char_system_.game_settings_.SetTimeMultiplier(1f);

        // define o respawn do jogador, definindo a posiçao
        char_system_.RespawnPlayer(spawn_position_.position, Quaternion.identity);

        // define a posiçao para a camera, evitando resposta normal
        cam_controller_.SetCamTransfor(spawn_position_.position, Quaternion.identity);
    }
}
