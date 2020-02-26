using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// componentes obrigatorios a este
[RequireComponent(typeof(CustomCharController))]
public class CharacterSystem : MonoBehaviour
{
      #region public fields
      // public fields ----------------------------------------
      // definiçoes de jogo
      [Header("Settings do jogo")]
      public GameSettings game_setting;
      // informaçao do jogador
      [Header("Informaçao do jogador")]
      public CharacterInfo char_infor;
      #endregion

      // private fields -----------------------------------------
      #region private Fields      
      //                            Referencias
      // comportamento da forma
      // comportamento do Aike
      private AikeBehavior _aikeBehavior;
      private ArifBehavior _arifBehavior;

      // variaveis de jogo
      // velocidade do jogador, nao visivel no editor
      [HideInInspector]
      public float _player_speed;
      #endregion

      // Start is called before the first frame update
      void Start()
      {
            // guarda referencias aos scripts de comportamento
            _aikeBehavior = new AikeBehavior(this);
            _arifBehavior = new ArifBehavior(this);
      }

      // Update is called once per frame
      void Update()
      {
            // com base na forma do jogador, corre o comportamento adequado
            switch (char_infor.shape)
            {
                  // caso a forma actual seja o Aike
                  case PlayerShape.Aike:
                        // corre o comportamento de Aike
                        _aikeBehavior.Behavior();
                        break;
                  // caso seja Arif
                  case PlayerShape.Arif:
                        // corre o comportamento de Arif
                        _arifBehavior.Behavior();
                        break;
                  default:
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
                        return new Vector3(char_infor.GetInputDir().x, 0f, char_infor.GetInputDir().z).normalized;
                  case PlayerShape.Arif:
                        // retorna todos os componentes do vector
                        return char_infor.GetInputDir().normalized;
                  default:
                        // caso nao esteja em nenhum dos estados, retorna um vector zero
                        return Vector3.zero;
            }
      }

      // retorna o gameObject referente ao player
      public GameObject GetPlayerGO() { return this.gameObject; }
      // retorna o transform referente ao jogador
      public Transform GetPlayerTransform() { return GetPlayerGO().transform; }
      // rotarna o CustomCharController do player
      public CustomCharController GetCustomController()
      { return GetPlayerGO().GetComponent<CustomCharController>(); }

}
