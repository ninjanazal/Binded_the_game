using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArifBehavior : MonoBehaviour
{
    // referencias internas
    private CharacterSystem _char_system_;   // referencia para o character system
    private Transform char_transform_;  // referencia para o transform do jogador
    private CharacterInfo char_info_;   // referencia para as informaçoes do jogador
    private GameSettings game_settings_;   // referencia para as definiçoes de jogo
    private CharacterController char_controller_;   // referencia para o controlador

    private float char_acceleration_;   // valor da aceleraçao calculada
    private Vector3 target_direction_;   // direcçao alvo calculada

    private float char_speed = 0f;   // velocidade do jogador
    private float char_roll_angle_ = 0f;   // angulo de rotaçao
    private Vector3 free_motion_ = Vector3.zero; // direcçao de deslocamento calculada
    private Vector3 vertical_motion_ = Vector3.zero;   // direcçao vertical do movimento
    private float calculated_roll_value_ = 0f;  // valor do angulo sobre o vector z 


    // inicia o comportamento de Arif
    public void ArifBehaviorLoad(CharacterSystem charSystem)
    {
        // guarda a referencia para o sistema de personagem
        _char_system_ = charSystem;
        // guarda referencia para o transform 
        char_transform_ = _char_system_.GetPlayerTransform();
        // guarda referencia para as definiçoes de sistema
        game_settings_ = _char_system_.game_settings_;
        // guarda referencias para as definiçoes do jogador
        char_info_ = _char_system_.char_infor;
        // guarda referencia para o character controller
        char_controller_ = _char_system_.GetCharController();

    }

    // comportamento da forma
    public void Behavior(ref float speed)
    {
        char_speed = speed;  // guarda o valor da velocidade partilhada
        InputManager();  // avalia o input no frame
        Movement(); //determina a direcçao do movimento resultante do input
        speed = char_speed;  // altera na referencia o valor da velocidade no final do loop

        DebugCalls();      // debug calls
    }


    #region BehaviorHandlers
    // Input Direcional---------
    // avalia input no frame
    private void InputManager()
    {
        // reseta variaveis de loop
        target_direction_ = Vector3.zero;
        char_acceleration_ = 0f;

        // avalia o input directional (Vertical)
        // caso exista input vertical
        if (Input.GetAxis("Vertical") != 0f)
            // define a direcçao alvo igual á direcçao da camera projectada para o
            // dependendo do input, incrementa ou decrementa a aceleraçao
            if (Input.GetAxis("Vertical") > 0f)
            {
                // personagem, Como o arif nao consegue se deslocar na direcçao da camera
                // a direcçao é sempre igual á da camera
                target_direction_ += _char_system_.ProjectDirection();

                // determinar a direcçao do jogador quando está na forma de Arif
                // varia a aceleraçao utilizando o input system do Unity
                char_acceleration_ += char_info_.ArifAceleration *
                   Mathf.Abs(Input.GetAxis("Vertical"));
            }
            // caso esteja a travar
            else
                // deve reduzir a velocidade de acordo com o valor de drag
                char_acceleration_ -= char_info_.ArifBreakSpeed * Mathf.Abs(Input.GetAxis("Vertical"));


        // caso exista input horizontal
        if (Input.GetAxis("Horizontal") != 0f)
        {
            // determina a direcçao causada por este input
            target_direction_ += (Quaternion.AngleAxis(90f, char_info_.GetInputUpDir()) *
              _char_system_.ProjectDirection() * Input.GetAxis("Horizontal")).normalized;
        }

        // normaliza a direcçao
        target_direction_.Normalize();

        // roda o jogador de acordo com a direcçao alvo determinada
        if (target_direction_ != Vector3.zero)
            char_transform_.forward = Vector3.Lerp(char_transform_.forward,
               target_direction_, char_info_.ArifRotationSpeed * game_settings_.PlayerTimeMultiplication());
        else
            // caso nao exista input, o modelo deve se orientar
            char_transform_.rotation = Quaternion.Lerp(char_transform_.rotation, Quaternion.LookRotation(
                Vector3.ProjectOnPlane(char_transform_.forward, Vector3.up), Vector3.up),
                char_info_.ArifRotationSpeed * game_settings_.PlayerTimeMultiplication());

        // determina o valor de roll de acordo com o angulo formado entre a direcçao alvo
        // e a direcçao do jogador
        // altera o roll do objecto de acordo com       
        calculated_roll_value_ =
               Vector3.SignedAngle(Vector3.ProjectOnPlane(
                   target_direction_, char_transform_.up), char_transform_.forward, char_transform_.up);

        // caso o valor do angulo for maior que a rotaçao no fram
        // calcula o valor intermedio para que a rotaçao seja mais homogenea
        char_roll_angle_ = Mathf.LerpAngle(
                char_roll_angle_, calculated_roll_value_,
                char_info_.ArifRollSpeed * game_settings_.PlayerTimeMultiplication());

        // adiciona á rotaçao, o valor de roll determinado anteriormente
        if (Mathf.Abs(char_roll_angle_) > char_info_.ArifRollSpeed * game_settings_.PlayerTimeMultiplication())
            char_transform_.rotation = Quaternion.Euler(char_transform_.rotation.eulerAngles.x,
                char_transform_.rotation.eulerAngles.y, char_roll_angle_);

    }

    // movimento horizontal ----------
    // reproduz o movimento calculado no input
    private void Movement()
    {
        // controlo de velocidade
        VelocityControl();
        // define a direcçao do movimento
        free_motion_ = (char_transform_.forward * char_speed) * game_settings_.PlayerTimeMultiplication();
        free_motion_ += GravityCompensation();

        // desloca o jogador de acordo com a direcçao calculada
        char_controller_.Move(free_motion_);
    }

    // valida o valor da velocidade
    private void VelocityControl()
    {
        if (char_acceleration_ == 0 && char_speed > 0)
            // caso o jogador nao esteja a acelarar, o player deve sofrer influencia do drag
            char_speed -= char_info_.ArifDrag * game_settings_.PlayerTimeMultiplication();
        // caso esteja a travar
        else if (char_acceleration_ < 0f) { char_speed += char_acceleration_ * game_settings_.PlayerTimeMultiplication(); }
        else
        {
            // define a velocidade maxima caso o jogador esteja com o shift pressionado
            if (Input.GetAxis("Run") != 0f)
            {
                // caso contrario é adicionada a velocidade
                char_speed += char_acceleration_ * game_settings_.PlayerTimeMultiplication();
                // limata o valor da velocidade ao maximo definido
                char_speed = Mathf.Clamp(char_speed, 0f, char_info_.ArifMaxSpeed);
            }
            else
            {
                // para impedir que ao alterar a velocidade simplemente seja alterada para a definida
                // sendo a velocidade actual superior á velocidade do estado
                if (char_speed > char_info_.ArifBaseSpeed)
                {
                    // lerp para a velocidade pretendida, ajustando a velocidade
                    char_speed = Mathf.Lerp(char_speed, char_info_.ArifBaseSpeed, char_info_.ArifDrag
                    * game_settings_.PlayerTimeMultiplication());
                }
                // caso contrario, o jogador vai á velocidade pretendida, é criado um clamp ao valor
                else
                {
                    // caso contrario é adicionada a velocidade
                    char_speed += char_acceleration_ * game_settings_.PlayerTimeMultiplication();
                    // limata o valor da velocidade ao maximo definido
                    char_speed = Mathf.Clamp(char_speed, 0f, char_info_.ArifMaxSpeed);

                }
            }

        }
        // para que a velocidade seja inferir á velocidade minima
        if (char_speed < char_info_.ArifMinSpeed)
            char_speed = char_info_.ArifMinSpeed;

    }

    // conpensaçao da gravidade
    private Vector3 GravityCompensation()
    {
        // retorna vector que representa o valor graviticional
        // caso o jogador esteja a planar
        return Vector3.up * (_char_system_.GravityValue * char_info_.ArifGravityReflexition) *
            game_settings_.PlayerTimeMultiplication();

    }
    #endregion

    // metodo de alteraçao de forma
    public void ArifToAikeChange()
    {
        // altera a forma para Aike
        char_info_.ChangeShape();
    }

    // debug call
    // metodo com calls de debug visual
    private void DebugCalls()
    {
        // desenha a direçao da frent do objecto
        Debug.DrawLine(char_transform_.position, char_transform_.position
            + char_transform_.forward, Color.red);
        // desenha a direcçao obtida atraves da direcçao da camera
        Debug.DrawLine(char_transform_.position, char_transform_.position +
            _char_system_.ProjectDirection(), Color.green);
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
