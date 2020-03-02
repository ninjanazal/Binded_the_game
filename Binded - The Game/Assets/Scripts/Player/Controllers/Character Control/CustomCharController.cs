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
   public Vector3 BaseGravityAcceleration; // valor basico da aceleraçao da gravidade

   #endregion

   #region privateVars
   // private vars
   private CharacterSystem char_system_;    // referencia ao systema de personage
   private CharacterController char_controller_;    // referencia para o transform do jogador
   private Transform char_transform_;    // referencia ao transform do jogador

   // variaveis de jogo
   // velocidade geral do jogador
   private float player_speed_;    // velocidade do jogador
   private float player_currentFrame_acceleration_;    // acceleraçao actual de input

   private Vector3 gravity_motion_ = Vector3.zero; // deslocamento calculado pela gravidade
   private float falling_time_ = 0f;   // tempo de queda
   private Vector3 calculated_motion_;    // movimento resultante do frame
   private Vector3 calculated_vertical_dir_;  // vector de deslocamento de salto
   #endregion

   private void Start()
   {
      // referencia para o transform local
      char_controller_ = this.GetComponent<CharacterController>();
      // guarda referencia para o sistema de personage
      char_system_ = this.GetComponent<CharacterSystem>();
      // guarda referencia para o transform do jogador
      char_transform_ = this.transform;
   }

   // update logico do controller
   private void Update()
   {
      // determina se o jogador está ou nao grounded
      if (!char_controller_.isGrounded)
      {
         falling_time_ += char_system_.game_setting.TimeMultiplication();
         // caso o jogador nao esteja no chao, a gravidade é adicionada
         // ao vector de movimento
         gravity_motion_ += BaseGravityAcceleration * Mathf.Pow(falling_time_, 2f);
      }
      // caso esteja no chao, a gravidade é anulada
      else { gravity_motion_ = Vector3.zero; falling_time_ *= 0f; }

      // caso exista valor de salto
      if (calculated_vertical_dir_ != Vector3.zero)
      {
         // adiciona o deslocamento
         gravity_motion_ += calculated_vertical_dir_;
         // reseta o valor do deslocamento adicionado anteriormento
         calculated_vertical_dir_ = Vector3.zero;
      }

      // adiciona o deslocamento vertical
      calculated_motion_ += gravity_motion_ * char_system_.game_setting.TimeMultiplication();

      // adiciona o deslocamento causado pelo deslocamento
      calculated_motion_ += char_transform_.forward * player_speed_ *
                  char_system_.game_setting.TimeMultiplication();

      // jogador continua a mover-se, com base no resultado
      if (calculated_motion_ != Vector3.zero) char_controller_.Move(calculated_motion_);

      // vector resultante do movimento é colocado a 0
      calculated_motion_ = Vector3.zero;

      // debug do custom charController
      CustomCharControllerDebug();
   }


   // deloca o jogador
   public void AikeBaseMove(float acceleration)
   {
      // deve calcular o mvimento a ser realizado
      player_currentFrame_acceleration_ = acceleration;

      // confirma a velocidade com base na aceleraçao introduzida
      VelocityControl();
   }

   // handler para salto do Aike
   public void AikeBaseJump(Vector3 jumpDir)
   {
      // o salto apena é possivel se o jogador estiver em contacto com uma superficie
      // ou seja, se esteja apoiado segundo a sua base
      // ** para ja grounded
      if (char_controller_.isGrounded)
      {
         // é adicionado uma força contraria á gravidade com a intensidade de salto
         calculated_vertical_dir_ += jumpDir.normalized * char_system_.char_infor.AikeJumpForce;
      }
   }

   // metodo para analizar a velocidade do jogador
   private void VelocityControl()
   {
      if (char_controller_.isGrounded)
      {
         // determina se nao existe aceleraçao e o jogador continua com velocidade
         if (player_currentFrame_acceleration_ == 0 && player_speed_ > 0)
         {
            // se passar esta condiçao, a velocidade de deslocamento deve 
            // abrandar de acordo com o drag determinado nas definiçoes do jogador
            player_speed_ -= char_system_.char_infor.AikeDrag *
                  char_system_.game_setting.TimeMultiplication();
         }
         //caso tenha ocorrido input e exista aceleraçao
         else
            player_speed_ += player_currentFrame_acceleration_;
      }


      // para que a velocidade nao cresça infinitamente, cada forma tem uma velocidade maxima
      // determinada
      player_speed_ = Mathf.Clamp(player_speed_, 0f, char_system_.char_infor.AikeMaxSpeed);
   }

   // linhas de debug do char controller
   private void CustomCharControllerDebug()
   {
      //Debug.Log("Valor do deslocamento vertical: " + gravity_motion_);
      // linha de debug, que demonstra a velocidade verticas
      Debug.DrawLine(char_transform_.position, char_transform_.position + gravity_motion_.normalized, Color.magenta);
   }


   private void OnControllerColliderHit(ControllerColliderHit hit)
   {
      Debug.Log(hit.point);
   }
}
