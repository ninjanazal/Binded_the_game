using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// custom controller para o movimento de jogadores
[RequireComponent(typeof(CharacterController))]
public class CustomCharController : MonoBehaviour
{
      // variaveis gerais para o controllo do personagem
      #region publicVars
      [Header("Valores gerais do controlador")]
      public Vector3 BaseGravity;
      //valor maximo de gravidade
      public float MaxGravitySpeed;

      #endregion

      #region privateVars
      // private vars
      // referencia ao systema de personage
      private CharacterSystem char_system;
      // referencia para o transform do jogador
      private CharacterController char_Controller;
      // referencia ao transform do jogador
      private Transform char_transform;


      // variaveis de jogo
      // velocidade geral do jogador
      // velocidade do jogador
      private float player_speed;
      // acceleraçao actual de input
      private float _player_currentFrame_acceleration;


      // movimento resultante do frame
      private Vector3 _calculated_motion;

      #endregion

      private void Start()
      {
            // referencia para o transform local
            char_Controller = this.GetComponent<CharacterController>();
            // guarda referencia para o sistema de personage
            char_system = this.GetComponent<CharacterSystem>();

            // guarda referencia para o transform do jogador
            char_transform = this.transform;
      }

      // update logico do controller
      private void Update()
      {
            // determina se o jogador está ou nao grounded
            if (!char_Controller.isGrounded)
            {
                  // caso o jogador nao esteja no chao, a gravidade é adicionada
                  // ao vector de movimento
                  _calculated_motion += BaseGravity *
                        char_system.game_setting.TimeMultiplication();
            }
            
            // adiciona a velocidade ao controlador
            _calculated_motion += char_transform.forward * player_speed *
                        char_system.game_setting.TimeMultiplication();

            // jogador continua a mover-se, com base no resultado
            if (_calculated_motion != Vector3.zero)
                  char_Controller.Move(_calculated_motion);


            // vector resultante do movimento é colocado a 0
            _calculated_motion = Vector3.zero;
      }



      // deloca o jogador
      public void SimpleMove(float acceleration)
      {
            // deve calcular o mvimento a ser realizado
            _player_currentFrame_acceleration = acceleration;

            // confirma a velocidade com base na aceleraçao introduzida
            VelocityControl();
      }

      // metodo para determinar se o jogador esta grounded
      // metodo para analizar a velocidade do jogador
      private void VelocityControl()
      {
            // determina se nao existe aceleraçao e o jogador continua com velocidade
            if (_player_currentFrame_acceleration == 0 && player_speed > 0)
            {
                  // se passar esta condiçao, a velocidade de deslocamento deve 
                  // abrandar de acordo com o drag determinado nas definiçoes do jogador
                  player_speed -= char_system.char_infor.AikeDrag *
                        char_system.game_setting.TimeMultiplication();
            }
            //caso tenha ocorrido input e exista aceleraçao
            else
                  player_speed += _player_currentFrame_acceleration;

            // para que a velocidade nao cresça infinitamente, cada forma tem uma velocidade maxima
            // determinada
            player_speed = Mathf.Clamp(player_speed, 0f, char_system.char_infor.AikeMaxSpeed);


      }
}
