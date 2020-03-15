using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AikeBehavior : MonoBehaviour
{
    // variaveis de  informaçao
    private CharacterSystem char_system_;      // referencia para o character system
    private Transform char_transform_;    // referencia ao transform do jogador
    private CharacterInfo char_info_;    // referencia para as informaçoes do player
    private GameSettings game_settings_;    // referencia para as definiçoes do jogo
    private CharacterController char_controller_;   // referencia para o character Controller

    // variaveis do comportamento
    private float char_acceleration_;    // aceleraçao calculado
    private Vector3 target_direction_;    // direcçao alvo 

    // variaveis de deslocamento
    private float char_speed_ = 0f;  // velocidade do jogador
    private Vector3 horizontal_motion_ = Vector3.zero;   // direcçao horizontal do movimento
    private Vector3 vertical_motion_ = Vector3.zero;   // direcçao vertical do movimento

    // variaveis de colisao e estado
    private Transform collision_target_;   // referencia para o transform do marcador de teste
    private Vector3 climb_jump_dir_; // direcçao do salto em climb
    private bool is_climbing_ = false;  // define se o jogador esta num estado de subida ou nao
    private bool changing_shape_ = false;    // define se o jogador está na faze de mudança de forma

    // construtor da classe
    public void AikeBehaviorLoad(CharacterSystem charSystem)
    {
        // guarda referencia ao character system
        char_system_ = charSystem;
        // guarda referencia para o transform
        char_transform_ = char_system_.GetPlayerTransform();
        // guarda referencia para as informaçoes do jogador
        char_info_ = char_system_.char_infor;
        // guarda referencia para as definiçoes de sistema
        game_settings_ = char_system_.game_settings_;
        // guarda referencia para o charController
        char_controller_ = char_system_.GetCharController();
        // guarda a referencia para o marcador da posiçao do groundTest
        collision_target_ = transform.GetChild(0).transform;
    }

    // comportamento da forma   
    public void Behavior(ref float speed)
    {
        char_speed_ = speed;  // guarda o valor da velocidade no frame        
        if (changing_shape_)
            AikeToArifChange(); // caso esteja a mudar de forma, ignora o input
        else
            InputManager(); // avalia o input     

        Movement(); // determina direcçoes resultantes do movimento horizontal
        VerticalMovement();  // determina direcçoes resultantes do movimento vertical
        speed = char_speed_;  // altera na referencia o valor da velocidade no final do ciclo

        // call for visual debug
        DebugCalls();

    }

    #region BehaviorHandlers
    // Input -----------------------
    // avalia o input no frame
    private void InputManager()
    {
        // reseta a aceleraçao determinada e direcçao
        char_acceleration_ = 0f;
        target_direction_ = Vector3.zero;

        // avalia o input direcional (Vertical)
        // caso exista input vertical
        if (Input.GetAxis("Vertical") != 0f)
        {
            // determinar a direcçao do jogador quando esta na forma de Aike
            // varia a aceleraçao utilizando o inout system do Unity
            char_acceleration_ += char_info_.AikeAceleration *
                  Mathf.Abs(Input.GetAxis("Vertical"));

            // define a direcçao alvo igual á direclao da camera
            target_direction_ += char_system_.ProjectDirection() *
                 Input.GetAxis("Vertical");
        }

        // caso exista input horizontal
        if (Input.GetAxis("Horizontal") != 0f)
        {
            // determina a direcçao do jogador quando esta na forma de AIke
            char_acceleration_ += char_info_.AikeAceleration *
                  Mathf.Abs(Input.GetAxis("Horizontal"));

            // define a direcçao alvo resultante da direcçao
            target_direction_ += (Quaternion.AngleAxis(90f, this.transform.up)
              * char_system_.ProjectDirection() * Input.GetAxis("Horizontal")).normalized;
        }
        // normaliza a direcçao
        target_direction_.Normalize();
        AjustCharRotation(); // ajusta a rotaçao do personagem á normal do terreno

        // roda o player para a direcçao calculada
        // so deve mudar de direcçao se estiveer no chao
        if (target_direction_ != Vector3.zero && GroundedCheck())
            char_transform_.rotation = Quaternion.Lerp(char_transform_.rotation,
               Quaternion.LookRotation(Vector3.ProjectOnPlane(target_direction_, char_transform_.up),
               char_transform_.up), char_info_.AikeRotationSpeed * game_settings_.PlayerTimeMultiplication());
    }

    // movimento horizontal  -----------------
    // reproduz o movimento horizontal calculado no input
    private void Movement()
    {
        // controla a velocidade
        VelocityControl();
        // define a direcçao do movimento do frame
        horizontal_motion_ = (char_transform_.forward * char_speed_) * game_settings_.PlayerTimeMultiplication();

        // desloca o jogador de acordo com a direcçao calculado
        char_controller_.Move(horizontal_motion_);
    }

    //valida o valor da velocidade
    private void VelocityControl()
    {
        // se estiver no chao
        // caso exista velocidade mas nao haja aceleraçao
        if (GroundedCheck())
        {
            if (char_acceleration_ == 0 && char_speed_ > 0)
                // o player deve abrandar devido ao drag
                char_speed_ -= char_info_.AikeDrag * game_settings_.PlayerTimeMultiplication();
            else
                // caso nao seja o caso a aceleraçao deve ser adicionada á velocidade
                char_speed_ += char_acceleration_ * game_settings_.PlayerTimeMultiplication();

            // define a velocidade maxima caso esteja a correr ou a andar
            if (Input.GetAxis("Run") != 0f)
                char_speed_ = Mathf.Clamp(char_speed_, 0f, char_info_.AikeMaxSpeed);
            else
            {
                // para impedir que a velocidade ultrapasse a velocidade maxima
                if (char_speed_ > char_info_.AikeMaxWalkingSpeed)
                    // caso o jogador esteja a uma velocidade superior
                    char_speed_ = Mathf.Lerp(char_speed_, char_info_.AikeMaxWalkingSpeed, char_info_.AikeDrag *
                        game_settings_.PlayerTimeMultiplication());
                // limita a velocidade
                if (char_speed_ < 0)
                    char_speed_ = 0f;
            }
        }

    }

    // movimento vertical
    private void VerticalMovement()
    {
        // implementaçao de gravidade
        // determina se o jogador está grounded
        vertical_motion_.y += (char_system_.GravityValue * char_info_.AikeGravityReflexition) *
           game_settings_.PlayerTimeMultiplication();
        // verifica a existencia de input para saltp
        JumpCheck();

        // move o player na direcçao determinada
        // caso o jogador nao esteja a escalar
        if (!is_climbing_)
            // move o jogador normalmento
            char_controller_.Move((vertical_motion_ - (Vector3.up * 1f)) * game_settings_.PlayerTimeMultiplication());

        else
        {
            // caso esteja sobre um objecto que possa trepar
            // move o jogador na direcçao determinada ajustada á orientaçao da superficie
            // caso ja esteja em queda
            if (vertical_motion_.y <= 0f)
                char_controller_.Move(Quaternion.FromToRotation(-Vector3.up, -char_transform_.up) *
                   vertical_motion_ * game_settings_.PlayerTimeMultiplication());
            // caso ainda exista velocidade positiva, relativa ao salto
            else if (vertical_motion_.y > 0f)
                char_controller_.Move(Quaternion.FromToRotation(-Vector3.up, -climb_jump_dir_) *
                   vertical_motion_ * game_settings_.PlayerTimeMultiplication());
        }

        // determina se o jogador está grounded
        if (GroundedCheck() && vertical_motion_.y < 0f)
            // reseta o valor do movimento vertical
            vertical_motion_.y = 0f;
    }

    // verificaçao de salto
    private void JumpCheck()
    {
        // caso base do jogador esteja em contacto com uma superficies
        // verifica se existe input de salto
        if (Input.GetButtonDown("Jump") && GroundedCheck())
        {
            // determina a força de salto necessaria para atingir a altura definida
            vertical_motion_.y = Mathf.Sqrt(-2 * (char_system_.GravityValue) *
               char_info_.AikeJumpHeight);
            // caso esteja no modo de salto, guarda a direçao
            if (is_climbing_)
                // guarda a normal do jogador
                climb_jump_dir_ = char_transform_.up;
        }
    }

    // retorna adirecçao vertical do jogador
    private void AjustCharRotation()
    {
        // metodo para ajustar a rotaçao do jogador em relaçao ao terreno em que está apoiado
        // utiliza um ray para determinar a direcçao do terreno
        RaycastHit hit;      // hit de saida
        Quaternion target_rotation_ = char_transform_.rotation;  // rotaçao alvo
        Vector3 difined_normal_ = Vector3.up;   // normal definida

        // colisao com a frente do jogador
        if (Physics.Raycast(char_transform_.position, char_transform_.forward, out hit,
           (collision_target_.position - char_transform_.position).magnitude +
           char_system_.maxAikeFloorDistance * 2f, char_system_.GroundMask))
            // caso exista uma colisao valida, ajusta a direcçao de acordo com a normal            
            difined_normal_ = hit.normal;

        // caso nao exista na frente, testa na parte de tras
        else if (Physics.Raycast(char_transform_.position, -char_transform_.forward, out hit,
            (collision_target_.position - char_transform_.position).magnitude +
            char_system_.maxAikeFloorDistance * 2f, char_system_.GroundMask))
            // caso exista uma colisao valida, ajusta a direcçao de acordo com a normal
            difined_normal_ = hit.normal;

        // caso falha, testa se existe algo aos pes do jogador
        else if (Physics.Raycast(char_transform_.position, -char_transform_.up, out hit,
           (collision_target_.position - char_transform_.position).magnitude +
           char_system_.maxAikeFloorDistance * 2f, char_system_.GroundMask))
        {
            // caso exista uma colisao valida, ajusta a direcçao de acordo com a normal           
            difined_normal_ = hit.normal;
            // determina se o jogador está sobre um objecto que pode ser escalavel
            is_climbing_ = (hit.transform.tag == "Climbable") ? true : false;
        }
        // determina a target rotation
        target_rotation_ = (Quaternion.LookRotation(Vector3.ProjectOnPlane(
            char_transform_.forward, difined_normal_).normalized, difined_normal_));

        // ajusta gradualmente a rotaçao actual á rotaçao determinada
        char_transform_.rotation = Quaternion.LerpUnclamped(char_transform_.rotation, target_rotation_,
           char_info_.AikeRotationSpeed * 2f * game_settings_.PlayerTimeMultiplication());
    }

    // determina se o player está em contacto com chao
    private bool GroundedCheck()
    {
        // determina o contacto com o chao sobre uma projecçao de uma esfera sobre a posiçao
        // do marcador no player
        // caso exista colisao na esfera com um objecto sobre a layer, retorna valor
        return Physics.CheckSphere(collision_target_.position,
        char_system_.maxAikeFloorDistance, char_system_.GroundMask);
    }
    #endregion

    // funcçao chamada para comportamento de alteraçao da forma
    public void AikeToArifChange()
    {
        // determinar se o jogador está no chao
        if (GroundedCheck())
        {
            // inicia a alteraçao da forma
            changing_shape_ = true;
            // caso o jogador esteja no chao, deve saltar
            vertical_motion_.y = Mathf.Sqrt(-2 * (char_system_.GravityValue) *
            char_info_.AikeJumpHeight);
            // caso esteja no modo de salto, guarda a direçao
            if (is_climbing_)
                // guarda a normal do jogador
                climb_jump_dir_ = char_transform_.up;
        }
        // determina se o jogador ja atingiu a altura maxima de salto
        if (vertical_motion_.y <= 0f)
        {
            // caso tenha atingido o valor maximo do salto
            // reseta o valor da gravidade e altera a forma
            vertical_motion_.y = 0f;
            changing_shape_ = false;    // indica que terminou a logica de mudança de forma
            char_transform_.rotation = Quaternion.LookRotation(char_system_.ProjectDirection(),
                char_info_.GetInputUpDir());
            char_info_.changeShape(); // altera a foram
        }
        else
            changing_shape_ = true; // caso esteja a saltar incia o processo de alteraçao da foram
    }

    // metodo com calls de debug visual
    private void DebugCalls()
    {
        // desenha a direçao da frent do objecto
        Debug.DrawLine(char_transform_.position, char_transform_.position
            + char_transform_.forward, Color.red);
        // desenha a direcçao obtida atraves da direcçao da camera
        Debug.DrawLine(char_transform_.position, char_transform_.position +
            char_system_.ProjectDirection(), Color.green);
        //desenha a direcçao alvo do jogador
        Debug.DrawLine(char_transform_.position, char_transform_.position +
          target_direction_, Color.blue);
        // desenha o deslocamento vertical
        Debug.DrawLine(char_transform_.position, char_transform_.position + vertical_motion_.normalized,
         Color.magenta);

        // desenha linha da gravidade relativa á orientaçao do objecto
        Debug.DrawLine(char_transform_.position, char_transform_.position +
         (Quaternion.FromToRotation(-Vector3.up, -char_transform_.up) * vertical_motion_), Color.yellow);
    }
}
