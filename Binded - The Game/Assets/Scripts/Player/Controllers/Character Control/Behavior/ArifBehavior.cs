using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArifBehavior
{
      // referencia para o character system
      private CharacterSystem _char_system;
      public ArifBehavior(CharacterSystem charSystem) => _char_system = charSystem;

      // comportamento da forma
      public void Behavior()
      {
          GameObject player = _char_system.GetPlayerGO();
          // gera o input do jogador
          InputHandler();

      }

      private void InputHandler()
      {
            // avalia o input direcional (Vertical)
            // caso o w esteja pressionado
            if (Input.GetKey(KeyCode.W))
            {
                
            }
            // caso o s esteja pressionado
            else if (Input.GetKey(KeyCode.S))
            {

            }
            // avalia o input direcional (horizontal)
            // caso o A esteja pressionado
            if (Input.GetKey(KeyCode.A))
            {

            }
            // caso o D esteja pressionado
            else if (Input.GetKey(KeyCode.D))
            {

            }
      }
}
