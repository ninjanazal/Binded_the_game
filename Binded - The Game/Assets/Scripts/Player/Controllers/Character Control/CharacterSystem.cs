using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// componentes obrigatorios a este
[RequireComponent(typeof(CharacterController),
   typeof(AikeBehavior), typeof(ArifBehavior))]
public class CharacterSystem : MonoBehaviour
{
    #region public fields
    // public fields ----------------------------------------
    [Header("Definiçoes necessarias")]
    public GameSettings game_settings_;      // definiçoes de jogo
    public CharacterInfo char_infor;      // informaçao do jogador

    // variaveis de comportamento
    [Header("Valores de comportamento")]
    public float GravityValue = -9.81f; // valor da gravidade
    public LayerMask GroundMask;  // mascara para a layer de ground
    public float maxFloorDistance = 0.4f; // distancia maxima que o player é considerado grounded

    #endregion

    // private fields -----------------------------------------
    #region private Fields      
    //                            Referencias
    private AikeBehavior _aikeBehavior; // comportamento da forma
    private ArifBehavior _arifBehavior; // comportamento do Aike

    private Transform groundPositionMarker;   // referencia á posiçao do marcador de ground

    // variaveis comuns
    private float char_speed = 0f;  // velocidade do jogador
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        // guarda referencias aos scripts de comportamento
        // caso consigo guardar referencia, inicia
        if (_aikeBehavior = this.GetComponent<AikeBehavior>())
            _aikeBehavior.AikeBehaviorLoad(this);
        // caso consiga guardar referencia, inicia
        if (_arifBehavior = this.GetComponent<ArifBehavior>())
            _arifBehavior.ArifBehaviorLoad(this);

    }

    // Update is called once per frame
    void Update()
    {
        // com base na forma do jogador, corre o comportamento adequado
        switch (char_infor.shape)
        {
            // caso a forma actual seja o Aike
            case PlayerShape.Aike:
                if (_aikeBehavior)
                    // corre o comportamento de Aike
                    _aikeBehavior.Behavior(ref char_speed);
                break;
            // caso seja Arif
            case PlayerShape.Arif:
                if (_arifBehavior)
                    // corre o comportamento de Arif
                    _arifBehavior.Behavior();
                break;
        }

    }

    // metodo que projecta a direçao do jogador no espaço de acçao
    public Vector3 ProjectDirection()
    {
        // a projecçao da direcçao é diferente com base na forma actual do jogador
        switch (char_infor.shape)
        {
            // caso o jogador esteja na forma de Aike,
            // a projecçao é interpertada apenas nas componentes x e z
            case PlayerShape.Aike:
                // caso esteja na fora de Aike a direçao é apenas os valores de x e de z
                // projectar o vector no plano
                return Vector3.ProjectOnPlane(new Vector3(char_infor.GetInputDir().x, 0f,
                  char_infor.GetInputDir().z), this.transform.up);
            case PlayerShape.Arif:
                // retorna todos os componentes do vector
                return char_infor.GetInputDir().normalized;
            default:
                // caso nao esteja em nenhum dos estados, retorna um vector zero
                return Vector3.forward;
        }
    }

    // retorna o gameObject referente ao player
    public GameObject GetPlayerGO() { return this.gameObject; }
    // retorna o transform referente ao jogador
    public Transform GetPlayerTransform() { return GetPlayerGO().transform; }
    // rotarna o CustomCharController do player
    public CharacterController GetCharController()
    { return this.GetComponent<CharacterController>(); }

    // Debug, on gizmos
    private void OnDrawGizmos()
    {
        // desenha esfera na posiçao de colisao determinada
        if (Physics.CheckSphere(this.transform.GetChild(0).transform.position, maxFloorDistance, GroundMask))
            // define a cor
            Gizmos.color = Color.green;
        else Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(this.transform.GetChild(0).transform.position, maxFloorDistance);

    }
}
