using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// custom controller para o movimento de jogadores
public class CustomCharController : MonoBehaviour
{
   // variaveis gerais para o controllo do personagem
   #region publicVars
   [Header("Valores gerais do controlador")]
   public Vector3 BaseGravityAcceleration; // valor basico da aceleraçao da gravidade
   public LayerMask GroundMask;  // mascara para objectos a serem detectados como chao
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
   private Vector3 calculated_motion_;    // movimento resultante do frame
   private Vector3 calculated_vertical_dir_;  // vector de deslocamento de salto

   private ControllerColliderHit last_hit_;   // ground check
   private Transform ground_check_pos_;  // direçao relativa á posiçao do jogador
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
      // guarda referencia para o transform de posiçao
      ground_check_pos_ = this.transform.GetChild(1).GetComponent<Transform>();
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

   // logica de update de Aike
   // logica de update para Aike em contacto horizontal
   private void CustomAikeFloorBehavior()
   {
      // determinar se o jogador esta em contacto com o chao
      is_grounded_ = Physics.CheckSphere(ground_check_pos_.position, 0.2f, GroundMask);

      // adiciona o deslocamento causado pelo deslocamento
      calculated_motion_ += ((CalculateMoveDir() * player_speed_)) *
                  char_system_.game_settings_.TimeMultiplication();

      // jogador continua a mover-se, com base no resultado
      char_controller_.Move(calculated_motion_);

      // caso o jogador nao esteja no chao, a gravidade é adicionada
      // ao vector de movimento
      gravity_motion_ += BaseGravityAcceleration * Mathf.Pow(Time.deltaTime, 2f);

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

      // move verticalmente
      char_controller_.Move(gravity_motion_);

      // debug do custom charController
      CustomCharControllerDebug();

      // vector resultante do movimento é colocado a 0
      calculated_motion_ = Vector3.zero;

   }

   // comportamento de Aike quando esta num estado de climb
   private void CustomAikeClimbBehavior()
   {
      Debug.Log("Climbing");
      // neste estado a gravidade nao deverá entrar em consideraçao
      // estado entra se o jogador colidir numa determinada direcçao com um objecto
      // que possa ser escalavel
      // representaçao dos vectores de interaçao 
      Debug.DrawLine(last_hit_.point, last_hit_.point + last_hit_.normal.normalized, Color.magenta);
      Debug.DrawLine(last_hit_.point, last_hit_.point + Quaternion.Euler(0f, -90f, 0f) * last_hit_.normal, Color.magenta);
   }


   #region public methods
   // deloca o jogador
   public void AikeBaseMove(float acceleration, Vector3 targetDir)
   {
      // caso a forma nao esteja no modo climbing
      if (!climbing)
      { // deve calcular o mvimento a ser realizado
         player_currentFrame_acceleration_ = acceleration;
         // confirma a velocidade com base na aceleraçao introduzida
         VelocityControl();

         // caso exista input, o jogador deve rodar
         if (acceleration != 0)
            // aponta o jogador para a direcçao resultante
            char_transform_.forward = Vector3.Lerp(char_transform_.forward, targetDir,
                  char_system_.char_infor.AikeRotationSpeed * char_system_.game_settings_.TimeMultiplication());
      }
      else
      {
         // esteja no modo de climbing
      }
   }

   // handler para salto do Aike
   public void AikeBaseJump()
   {
      // o salto apena é possivel se o jogador estiver em contacto com uma superficie
      // ou seja, se esteja apoiado segundo a sua base
      // ** para ja grounded
      if (!jumping && is_grounded_)
      {
         // é adicionado uma força contraria á gravidade com a intensidade de salto
         calculated_vertical_dir_ = Vector3.up *
            Mathf.Sqrt(-2 * BaseGravityAcceleration.y * char_system_.char_infor.AikeJumpHeight);
         // ao saltar, o jogador de qualquer das formas deve sair de estado de climbing
         if (climbing) climbing = false;
         jumping = true;
      }
   }
   #endregion


   #region private methods
   // metodo para analizar a velocidade do jogador
   private void VelocityControl()
   {
      if (is_grounded_)
      {
         // determina se nao existe aceleraçao e o jogador continua com velocidade
         if (player_currentFrame_acceleration_ == 0 && player_speed_ > 0)
            // se passar esta condiçao, a velocidade de deslocamento deve 
            // abrandar de acordo com o drag determinado nas definiçoes do jogador
            player_speed_ -= char_system_.char_infor.AikeDrag *
                  char_system_.game_settings_.TimeMultiplication();
      }
      //caso tenha ocorrido input e exista aceleraçao
      else
         player_speed_ += player_currentFrame_acceleration_;

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
      // guarda a referencia para o hit   
      last_hit_ = hit;

      // guarda o ultimo hit que o characterController obteve
      // determina o angulo de colisao vertical
      float coll_angle_vertical_ = Vector3.Angle(char_system_.transform.up, hit.normal);
      // determina o angulo de colisao horizontal
      float coll_angle_horizontal_ = Vector3.Angle(-char_system_.transform.forward, hit.normal);

      // validaçao vertical da colisao
      // valida se o hit entra em algum dos estados necessarios
      // calcula o angulo entre a normal do ponto de colisao
      // e a direcçao up do jogador
      // validaçao horizontal da colisao
      if (coll_angle_horizontal_ <= char_system_.char_infor.AikeAngleGrabWall)
         // caso isto ocorra , a colisao está dentro do angulo da colisao
         // confirma se o objecto é trepavel
         if (hit.gameObject.tag == "Climbable" && !climbing)
         {
            // se sim, deve alterar o comportamento do jogador
            climbing = true;
            // ajustar a rotaçao do objecto

            // Ajusta rotaçao do jogador em relaçao ao ponto de contacto
            // Determina a rotaçao necessaria para corrigir a direcçao          
            Vector3 rotation = new Vector3(Vector3.SignedAngle(char_transform_.up, last_hit_.normal, char_transform_.right),
            0f, Vector3.SignedAngle(char_transform_.right, Quaternion.Euler(0f, -90f, 0f) * last_hit_.normal, char_transform_.up));
            // atribui a rotaçao do objecto
            char_transform_.Rotate(rotation);
         }


      if (coll_angle_vertical_ > 130)
         // remove a velocidade positiva vertica
         gravity_motion_.y = (gravity_motion_.y > 0f) ? 0f : gravity_motion_.y;
   }



   // ------------------------------------------- DEBUG
   // linhas de debug do char controller
   private void CustomCharControllerDebug()
   {
      // linha de debug, que demonstra a velocidade verticas
      Debug.DrawLine(char_transform_.position, char_transform_.position + gravity_motion_.normalized, Color.magenta);
      //linha de debug para a direcçao optima para o movimento
      Debug.DrawLine(char_controller_.transform.position,
         char_controller_.transform.position + CalculateMoveDir().normalized, Color.yellow);

   }
}
#endregion

