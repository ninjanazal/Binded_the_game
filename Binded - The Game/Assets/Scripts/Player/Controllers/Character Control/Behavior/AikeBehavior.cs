using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AikeBehavior
{
      // referencia para o character system
      private CharacterSystem _char_system;
      // referencia ao transform do jogador
      private Transform _player_transform;
      // referencia para as informaçoes do player
      private CharacterInfo _char_info;
      // referencia para as definiçoes do jogo
      private GameSettings _game_settings;
      // referencia ao custom Char controller
      private CustomCharController _custom_char_controller;


      // variaveis do comportamento
      // aceleraçao calculado
      private float _playerAcceleration;
      // direcçao alvo 
      private Vector3 _target_direction;

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
      public void Behavior()
      {
            _player_transform = _char_system.GetPlayerTransform();
            // gera o input do jogador
            InputHandler();

            // call for visual debug
            DebugCalls();
      }

      private void InputHandler()
      {
            // avalia o input direcional (Vertical)
            // caso o w esteja pressionado

            // caso exista input vertical
            if (Input.GetAxis("Vertical") != 0f)
            {
                  // determinar a direcçao do jogador quando esta na forma
                  // de Aike
                  // varia a aceleraçao utilizando o inout system do Unity
                  _playerAcceleration = _char_info.AikeAceleration *
                        Mathf.Abs(Input.GetAxis("Vertical")) *
                        _game_settings.TimeMultiplication();

                  // define a direcçao alvo igual á direclao da camera
                  _target_direction = _char_system.ProjectDirection() *
                       Input.GetAxis("Vertical");


                  // move enquanto acerta a rotaçao alvo
                  _player_transform.forward = Vector3.Lerp(_player_transform.forward,
                    _target_direction,
                    _char_info.AikeRotationSpeed * _game_settings.TimeMultiplication());
            }

            // chama o movimento no caracterController
            _custom_char_controller.SimpleMove(_playerAcceleration);
            // reseta a aceleraçao determinada
            _playerAcceleration = 0f;
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
              _target_direction, Color.blue);
      }
}
