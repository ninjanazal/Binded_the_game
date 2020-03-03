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

   private ControllerColliderHit last_hit_;   // ground check
   private bool is_grounded_ = false; // indica se o jogador esta em contacto com o chao
   private bool jumping = false; // indica se o jogador está em salto ou nao
   #endregion

   private void Start()
   {
      // referencia para o transform local
      char_controller_ = this.GetComponent<CharacterController>();
      // guarda referencia para o sistema de personage
      char_system_ = this.GetComponent<CharacterSystem>();
      // guarda referencia para o transform do jogador
      char_transform_ = this.transform;
      // inicializa o ultimo toque com o collider
      last_hit_ = null;
   }

   // update logico do controller
   private void Update()
   {
      Debug.Log(jumping);
      // determina se o jogador está ou nao grounded
      if (!char_controller_.isGrounded || !is_grounded_)
      {
         falling_time_ += char_system_.game_settings_.TimeMultiplication();
         // caso o jogador nao esteja no chao, a gravidade é adicionada
         // ao vector de movimento
         gravity_motion_ += BaseGravityAcceleration * Mathf.Pow(falling_time_, 2f);
      }
      // caso esteja no chao, a gravidade é anulada
      else
      {
         // ao voltar a tocar numa superficie de contacto
         // repoem as variaveis de direcçao e tempo
         gravity_motion_ = Vector3.zero;
         falling_time_ *= 0f;
         jumping = false;
      }

      // caso exista valor de salto
      if (jumping && calculated_vertical_dir_ != Vector3.zero && AnyGroundConfirmation())
      {
         // adiciona o deslocamento
         gravity_motion_ += calculated_vertical_dir_;
         // reseta o valor do deslocamento adicionado anteriormento
         calculated_vertical_dir_ = Vector3.zero;
      }
      // adiciona o deslocamento vertical ao deslocamento geral
      calculated_motion_ += gravity_motion_ *
         char_system_.game_settings_.TimeMultiplication();

      // adiciona o deslocamento causado pelo deslocamento
      calculated_motion_ += CalculateMoveDir() * player_speed_ *
                  char_system_.game_settings_.TimeMultiplication();

      // jogador continua a mover-se, com base no resultado
      if (calculated_motion_ != Vector3.zero) char_controller_.Move(calculated_motion_);
      // vector resultante do movimento é colocado a 0
      calculated_motion_ = Vector3.zero;

      // debug do custom charController
      CustomCharControllerDebug();
   }



   #region public methods
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
      if (AnyGroundConfirmation() && !jumping)
      {
         // é adicionado uma força contraria á gravidade com a intensidade de salto
         calculated_vertical_dir_ = jumpDir.normalized * char_system_.char_infor.AikeJumpForce;
         // define a variavel para true, indicando que o jogador esta a saltar
         jumping = true;
      }
   }
   #endregion


   #region private methods
   // metodo para analizar a velocidade do jogador
   private void VelocityControl()
   {
      if (char_controller_.isGrounded || is_grounded_)
      {
         // determina se nao existe aceleraçao e o jogador continua com velocidade
         if (player_currentFrame_acceleration_ == 0 && player_speed_ > 0)
         {
            // se passar esta condiçao, a velocidade de deslocamento deve 
            // abrandar de acordo com o drag determinado nas definiçoes do jogador
            player_speed_ -= char_system_.char_infor.AikeDrag *
                  char_system_.game_settings_.TimeMultiplication();
         }
         //caso tenha ocorrido input e exista aceleraçao
         else
            player_speed_ += player_currentFrame_acceleration_;
      }


      // para que a velocidade nao cresça infinitamente, cada forma tem uma velocidade maxima
      // determinada
      player_speed_ = Mathf.Clamp(player_speed_, 0f, char_system_.char_infor.AikeMaxSpeed);
   }
   // determina se o jogador esta em contacto com o chao
   private Vector3 CalculateMoveDir()
   {
      // determina o direcçao correcta do deslocamento
      // caso o jogado nao tenha colidido com qualquer objecto, a melhor direcçao
      // é a direcçao actual do objecto
      if (last_hit_ == null) return char_transform_.forward;
      else
      {
         // caso exista colisao, é determinado o vector de direcçao atravez a normal do objecto
         return Vector3.Cross(char_controller_.transform.right, last_hit_.normal);
      }
   }
   // determina se em algum dos sistemas o objecto esta grounded
   private bool AnyGroundConfirmation() { return (char_controller_.isGrounded || is_grounded_); }

   // linhas de debug do char controller
   private void CustomCharControllerDebug()
   {
      // linha de debug, que demonstra a velocidade verticas
      Debug.DrawLine(char_transform_.position, char_transform_.position + gravity_motion_.normalized, Color.magenta);
      Debug.DrawLine(char_controller_.transform.position,
         char_controller_.transform.position + CalculateMoveDir().normalized, Color.yellow);
   }
   #endregion


   // chamada para interaçao com objectos ou detecçao de distancias
   private void OnControllerColliderHit(ControllerColliderHit hit)
   {
      // guarda o ultimo hit que o characterController obteve
      if (last_hit_ != hit)
      {
         // determina o angulo de colisao
         float coll_angle = Vector3.Angle(char_system_.transform.up, hit.normal);

         // valida se o hit entra em algum dos estados necessarios
         // calcula o angulo entre a normal do ponto de colisao
         // e a direcçao up do jogador
         if (coll_angle < char_controller_.slopeLimit)
         {
            // guarda que o jogador está em contacto com o chao
            is_grounded_ = true;
            // guarda o valor de hit
            last_hit_ = hit;
            // actualiza o valor de jumping
            jumping = false;
         }
         // caso o jogador tenha colidido com algo sobre si
         else if (coll_angle > 130)
         {
            // remove a velocidade positiva vertica
            gravity_motion_.y = (gravity_motion_.y > 0f) ? 0f : gravity_motion_.y;
         }
         else
         {
            // o jogador nao tenha colidido com algum objecto a baixo do angulo de slope
            // confimr se o char controller detectou se nao esta grounded
            is_grounded_ = false;
         }


      }

   }
}
