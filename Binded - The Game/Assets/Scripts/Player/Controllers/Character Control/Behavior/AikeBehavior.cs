using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AikeBehavior
{
   private CharacterSystem _char_system;      // referencia para o character system
   private Transform _player_transform;    // referencia ao transform do jogador
   private CharacterInfo _char_info;    // referencia para as informaçoes do player
   private GameSettings _game_settings;    // referencia para as definiçoes do jogo
   private CustomCharController _custom_char_controller;    // referencia ao custom Char controller

   // variaveis do comportamento
   private float playerAcceleration_;    // aceleraçao calculado
   private Vector3 target_direction_;    // direcçao alvo 


   // construtor da classe
   public AikeBehavior(CharacterSystem charSystem)
   {
      // guarda referencia ao character system
      _char_system = charSystem;
      // guarda referencia para 
      _player_transform = _char_system.GetPlayerTransform();
      // guarda referencia para as informaçoes do jogador
      _char_info = _char_system.char_infor;
      // guarda referencia para as definiçoes de sistema
      _game_settings = _char_system.game_setting;
      // guarda referencia para o customCharController do jogador
      _custom_char_controller = _char_system.GetCustomController();
   }

   // comportamento da forma
   public void Behavior() => InputHandler();    // gera o input do jogador

   // manager de input
   private void InputHandler()
   {
      // avalia o input direcional (Vertical)
      // caso exista input vertical
      if (Input.GetAxis("Vertical") != 0f)
      {
         // determinar a direcçao do jogador quando esta na forma de Aike
         // varia a aceleraçao utilizando o inout system do Unity
         playerAcceleration_ = _char_info.AikeAceleration *
               Mathf.Abs(Input.GetAxis("Vertical")) *
               _game_settings.TimeMultiplication();

         // define a direcçao alvo igual á direclao da camera
         target_direction_ += _char_system.ProjectDirection() *
              Input.GetAxis("Vertical");

      }

      // caso exista input horizontal
      if (Input.GetAxis("Horizontal") != 0f)
      {
         // determina a direcçao do jogador quando esta na forma de AIke
         playerAcceleration_ = _char_info.AikeAceleration *
               Mathf.Abs(Input.GetAxis("Horizontal")) *
               _game_settings.TimeMultiplication();

         // define a direcçao alvo resultante da direcçao
         target_direction_ += Quaternion.Euler(0f, 90f, 0f)
         * _char_system.ProjectDirection() * Input.GetAxis("Horizontal");
      }
      // normaliza a direcçao
      target_direction_.Normalize();

      // caso exista input, o jogador deve rodar
      if (playerAcceleration_ != 0)
         // aponta o jogador para a direcçao resultante
         _player_transform.forward = Vector3.Lerp(_player_transform.forward, target_direction_,
               _char_info.AikeRotationSpeed * _game_settings.TimeMultiplication());

      // chama o movimento no caracterController
      _custom_char_controller.AikeBaseMove(playerAcceleration_);

      // call for visual debug
      DebugCalls();

      // reseta a aceleraçao determinada e direcçao
      playerAcceleration_ = 0f;
      target_direction_ *= 0f;
   }

   // metodo com calls de debug visual
   private void DebugCalls()
   {
      // desenha a direçao da frent do objecto
      Debug.DrawLine(_player_transform.position, _player_transform.position
          + _player_transform.forward, Color.red);
      // desenha a direcçao obtida atraves da direcçao da camera
      Debug.DrawLine(_player_transform.position, _player_transform.position +
          _char_system.ProjectDirection(), Color.green);
      //desenha a direcçao alvo do jogador
      Debug.DrawLine(_player_transform.position, _player_transform.position +
        target_direction_, Color.blue);
   }
}
