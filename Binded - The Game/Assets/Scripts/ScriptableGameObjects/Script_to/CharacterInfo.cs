using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerShape { Aike, Arif }

[CreateAssetMenu(fileName = "CharacterInformation", menuName = "Binded/CharacterInformation")]
public class CharacterInfo : ScriptableObject
{
    // fields -----------------------------------------
    // input de direcçao do jogador pela camera
    private Vector3 input_cam_dir_;  // direcçao da camera
    private Vector3 input_cam_up_;  // up da camera

    private bool changing_state_ = false;    //valor guarda se o jogador está ou nao a mudar de forma 

    // informaçao sobre a forma do jogador
    [Header("Forma actual do jogador")]
    public PlayerShape shape;
    

    #region AikeVars
    // variavies relacionada com Aike
    [Header("Aike")]
    public float AikeGravityReflexition = 3.051f; // valor que simula a variaçao da gravidade com o peso
    public float AikeMaxWalkingSpeed = 2.5f;   // valor da velocidade maxima que aike consegue andar
    public float AikeMaxSpeed = 3.0f;      // velocidade maxima
    public float AikeAceleration = 3.0f;   // aceleraçao
    public float AikeRotationSpeed = 5f;   // velocidade de rotaçao
    public float AikeDrag = 5f;      // drag de movimento
    public float AikeJumpHeight = 50f;      // força de salto
    #endregion

    #region ArifVars
    // variavies relacionada com Aike
    [Header("Arif")]
    public float ArifGravityReflexition = 0.5f; // valor que simula a variaçao da gravidade com o peso
    public float ArifMaxSpeed = 20.0f;  // velocidade maxima em "corrida"
    public float ArifBaseSpeed = 10.0f;     // velocidade maxima
    public float ArifMinSpeed = 1.0f;   // velocidade minima que pode ter
    public float ArifAceleration = 5.0f;   // aceleraçao
    public float ArifBreakSpeed = 10f;  // força de travagem 
    public float ArifRotationSpeed = 4f;   // velocidade de rotaçao
    public float ArifRollSpeed = 10f;   // rotaçao de roll
    public float ArifDrag = 6f;      // drag de movimento
    #endregion

    // variaveis de evento
    private PlayerRenderManager render_manager_;    // render manager
    // regista o render manager
    public void RegistRendererManager(PlayerRenderManager manager) => render_manager_ = manager;
    public void ClearRegistRenderManager() => render_manager_ = null;   // limpa o render manager

    // metodos  --------------------------------
    #region Methods
    // altera a forma do jogador
    /// <summary>
    /// Switch the player shaper
    ///</summary>
    public void ChangeShape()
    {
        // atribui que o jogador esta a mudar de forma
        changing_state_ = true;

        // altera para a forma nao actual
        shape = (shape == PlayerShape.Aike) ? PlayerShape.Arif : PlayerShape.Aike;
        // caso exista um render manager registado
        if (render_manager_)
            render_manager_.ChangeOccurred(shape);  // envia que ocorreu uma alteraçao na forma
    }
    // metodo chamado quando o jogador terminou de mudar de forma
    public void ChangeEnded() { changing_state_ = false; }
    // metodo chamado para indicar se o jogador está a mudar de forma ou nao
    public bool IsChangingShape() { return changing_state_; }

    // atribui e retorna valor de direcçao de input
    public void UpdateInputDir(Vector3 direction) => input_cam_dir_ = direction;
    //retorna direcçao da camera
    public Vector3 GetInputDir() { return input_cam_dir_; }
    // guarda valor da direcçao up da camera
    public void UpdateInputDirUp(Vector3 upDirection) => input_cam_up_ = upDirection;
    // retorna o valor do up da camera
    public Vector3 GetInputUpDir() { return input_cam_up_; }

    #endregion
}
