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

    // ao iniciar verifica o estado em que o jogador esta, e inicia a mesh render adequada
    private void Start()
    {
        // confirma o estado
        StateChecker(char_info_.shape);

        // regista o render manager para receber eventos
        char_info_.RegistRendererManager(this);
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

    public void ChangeOccurred(PlayerShape toShape)
    {
        StateChecker(toShape);
    }
    // metodo chamado sempre que entrar em Aike
    private void EnableAike()
    {
        // ao entrar no modo de Aike, deve desativar o mesh render da outra forma
        arif_mesh_renderer.enabled = false;
        // activar o render da forma actual
        aike_mesh_renderer_.enabled = true;
    }

    // metodo chamado sempre que entrar em Arif
    private void EnableArif()
    {
        // ao entrar no modo de Arif, deve desativar o mesh render da outra forma
        aike_mesh_renderer_.enabled = false;
        // activa o renderer da forma actual
        arif_mesh_renderer.enabled = true;
    }


}
