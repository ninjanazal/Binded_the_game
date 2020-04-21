using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRenderManager : MonoBehaviour
{
    // class responsavel por definir a mesh a ser apresentada de acordo com o estado do jogador
    // referencias publicas
    [Header("Informaçoes em Asset")]
    public CharacterInfo char_info_;    // referencia para as informaçoes do jogador

    [Header("Componentes do jogador")]
    public MeshRenderer aike_mesh_renderer_;    // referencia para o renderer do Aike
    public MeshRenderer arif_mesh_renderer;     // referencia para o renderer do Arif

    [Header("Variaveis para animaçao")]
    public float Aike_max_fov_pulse = 80f;  // valor do pulso maximo para o Aike
    public float Arif_max_fov_pulse = 80f;  // valor do pulso maximo para o Arif

    [Header("Particulas de alteraçao de forma")]
    public ParticleSystem shape_switch_particle;    // referencia ao sistema de particulas de transformaçao

    [Header("Trails para o jogador")]    
    public TrailRenderer left_trail_;   // trail esquerdo de velocidade do Arif
    public TrailRenderer right_trail_; // trails direito de velocidade do arif


    // ao iniciar verifica o estado em que o jogador esta, e inicia a mesh render adequada
    private void Start()
    {
        // confirma o estado
        StateChecker(char_info_.shape);

        // regista o render manager para receber eventos
        char_info_.RegistRendererManager(this);

        // desativa o trails ao iniciar
        left_trail_.emitting = false;
        right_trail_.emitting = false;

    }

    // confirma o comportamento a tomar com forme a forma para que foi alterado
    private void StateChecker(PlayerShape shape)
    {
        // determina a acçao de acordo com a forma inicial
        switch (shape)
        {
            case PlayerShape.Aike:
                // caso seja a forma de Aike
                EnableAike();
                break;
            case PlayerShape.Arif:
                // caso seja a forma de Arif
                EnableArif();
                break;
        }
    }

    // funçao disperta sempre que o jogador muda de estado
    public void ChangeOccurred(PlayerShape toShape) => StateChecker(toShape);

    // metodo chamado sempre que entrar em Aike
    private void EnableAike()
    {
        // ao entrar no modo de Aike, deve desativar o mesh render da outra forma
        arif_mesh_renderer.enabled = false;

        // inicia o efeito de pulsa o fov da camera
        IEnumeratorCallBacks.Instance.PulseFOVEffectCallback(Aike_max_fov_pulse);

        // antes de trocar a formma, activa as particulas
        shape_switch_particle.Play();   
        // activar o render da forma actual
        aike_mesh_renderer_.enabled = true;

        // define a nova distancia para a camera
        IEnumeratorCallBacks.Instance.SetNewCameraDistance(5f, 20f);
    }

    // metodo chamado sempre que entrar em Arif
    private void EnableArif()
    {
        // ao entrar no modo de Arif, deve desativar o mesh render da outra forma
        aike_mesh_renderer_.enabled = false;

        // inicia o efeito de pulsa o fov da camera
        IEnumeratorCallBacks.Instance.PulseFOVEffectCallback(Arif_max_fov_pulse);

        // antes de trocar de forma, activa as particulas
        shape_switch_particle.Play();

        // activa o renderer da forma actual
        arif_mesh_renderer.enabled = true;

        // define uma nova distancia para a camera
        IEnumeratorCallBacks.Instance.SetNewCameraDistance(8f, 50f);

    }

    // metodos publicos
    // activa os trails das asas
    public void TrailsSetter(float ArifSpeed)
    {
        // so ocorre se estiver na forma de arif
        if (char_info_.shape != PlayerShape.Arif)
        {
            // caso nao esteja no estado de Arif, desativa os trails
            left_trail_.emitting = false;
            right_trail_.emitting = false;
            return;
        }

        // se a velocidade do arif actual for superior á metade da velocidade maxima
        // activa os trails
        left_trail_.emitting = (char_info_.ArifMaxSpeed * 0.5f < ArifSpeed) ?
            true : false;
        // iguala o valor atribuido ao trail esquerdo com o direito
        right_trail_.emitting = left_trail_.emitting;

    }

}
