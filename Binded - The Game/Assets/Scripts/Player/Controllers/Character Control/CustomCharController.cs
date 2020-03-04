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

   private bool climbing = false;   // guarda se o jogador esta num estado de climb ou nao
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
      switch (char_system_.char_infor.shape)
      {
         case PlayerShape.Aike:
            // logica de comportamento de controlo para Aike
            // caso o jogador esteja a subir um objecto
            if (climbing) CustomAikeClimbBehavior();
            else CustomAikeFloorBehavior();
            break;
         case PlayerShape.Arif:
            // logica de comportamento de comtrolo para Arif
            break;
      }
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
   public void AikeBaseJump()
   {
      // o salto apena é possivel se o jogador estiver em contacto com uma superficie
      // ou seja, se esteja apoiado segundo a sua base
      // ** para ja grounded
      if (!jumping)
      {
         // é adicionado uma força contraria á gravidade com a intensidade de salto
         if (calculated_vertical_dir_ == Vector3.zero)
            calculated_vertical_dir_ = Vector3.up *
               Mathf.Sqrt(0 - 2 * BaseGravityAcceleration.y * char_system_.char_infor.AikeJumpForce);
         // ao saltar, o jogador de qualquer das formas deve sair de estado de climbing
         if (climbing) climbing = false;
      }
   }
   #endregion


   #region private methods
   // logica de update de Aike
   // logica de update para Aike em contacto horizontal
   private void CustomAikeFloorBehavior()
   {
      // adiciona o deslocamento causado pelo deslocamento
      calculated_motion_ += ((CalculateMoveDir() * player_speed_) + gravity_motion_) *
                  char_system_.game_settings_.TimeMultiplication();

      // jogador continua a mover-se, com base no resultado
      if (calculated_motion_ != Vector3.zero) char_controller_.Move(calculated_motion_);

      // caso esteja no chao, a gravidade é anulada
      if (char_controller_.isGrounded && is_grounded_)
      {
         // ao voltar a tocar numa superficie de contacto
         // repoem as variaveis de direcçao e tempo
         gravity_motion_ = Vector3.zero;
         falling_time_ *= 0f;
         // guarda que o jogador nao esta a saltar visto que tocou no chao
         jumping = false;
      }
      // caso o jogador nao esteja no chao, a gravidade é adicionada
      // ao vector de movimento
      gravity_motion_ += BaseGravityAcceleration * Mathf.Pow(falling_time_, 2f);
      // determina se o jogador está ou nao grounded
      falling_time_ += char_system_.game_settings_.TimeMultiplication();

      // caso exista valor de salto
      if (!jumping && calculated_vertical_dir_ != Vector3.zero)
      {
         // define a variavel para true, indicando que o jogador esta a saltar
         jumping = true;
         // adiciona o deslocamento
         gravity_motion_ += calculated_vertical_dir_;
         // reseta o valor do deslocamento adicionado anteriormento
         calculated_vertical_dir_ = Vector3.zero;
      }

      // debug do custom charController
      CustomCharControllerDebug();

      // vector resultante do movimento é colocado a 0
      calculated_motion_ = Vector3.zero;

   }
   // comportamento de Aike quando esta num estado de climb
   private void CustomAikeClimbBehavior()
   {
      // neste estado a gravidade nao deverá entrar em consideraçao
      // estado entra se o jogador colidir numa determinada direcçao com um objecto
      // que possa ser escalavel
      // ajustar a rotaçao do objecto
      if (last_hit_ != null)
         char_transform_.up = Vector3.Lerp(char_transform_.up, last_hit_.normal, char_system_.char_infor.AikeRotationSpeed *
            char_system_.game_settings_.TimeMultiplication());


   }

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
         return Vector3.Cross(char_controller_.transform.right, last_hit_.normal).normalized;
      }
   }

   // chamada para interaçao com objectos ou detecçao de distancias
   private void OnControllerColliderHit(ControllerColliderHit hit)
   {
      // guarda o ultimo hit que o characterController obteve
      if (last_hit_ != hit)
      {
         // determina o angulo de colisao vertical
         float coll_angle_vertical_ = Vector3.Angle(char_system_.transform.up, hit.normal);
         // determina o angulo de colisao horizontal
         float coll_angle_horizontal_ = Vector3.Angle(-char_system_.transform.forward, hit.normal);

         // validaçao vertical da colisao
         // valida se o hit entra em algum dos estados necessarios
         // calcula o angulo entre a normal do ponto de colisao
         // e a direcçao up do jogador
         if (coll_angle_vertical_ < char_controller_.slopeLimit)
         {
            // guarda que o jogador está em contacto com o chao
            is_grounded_ = true;
            // guarda o valor de hit
            last_hit_ = hit;
            // actualiza o valor de jumping
            jumping = false;
         }
         // caso o jogador tenha colidido com algo sobre si
         else if (coll_angle_vertical_ > 130)
         {
            // remove a velocidade positiva vertica
            gravity_motion_.y = (gravity_motion_.y > 0f) ? 0f : gravity_motion_.y;
         }
         // o jogador nao tenha colidido com algum objecto a baixo do angulo de slope
         // confimr se o char controller detectou se nao esta grounded
         else is_grounded_ = false;

         // validaçao horizontal da colisao
         if (coll_angle_horizontal_ <= char_system_.char_infor.AikeAngleGrabWall)
         {
            // guarda a referencia para o hit   
            last_hit_ = hit;
            // caso isto ocorra , a colisao está dentro do angulo da colisao
            // confirma se o objecto é trepavel
            if (hit.gameObject.tag == "Climbable" && !climbing)
               // se sim, deve alterar o comportamento do jogador
               climbing = true;
         }
      }
   }
   #endregion

   // ------------------------------------------- DEBUG
   // linhas de debug do char controller
   private void CustomCharControllerDebug()
   {
      // linha de debug, que demonstra a velocidade verticas
      Debug.DrawLine(char_transform_.position, char_transform_.position + gravity_motion_.normalized, Color.magenta);
      Debug.DrawLine(char_controller_.transform.position,
         char_controller_.transform.position + CalculateMoveDir().normalized, Color.yellow);
      Debug.DrawLine(char_transform_.position, char_transform_.position + calculated_motion_.normalized, Color.black);
   }
}
