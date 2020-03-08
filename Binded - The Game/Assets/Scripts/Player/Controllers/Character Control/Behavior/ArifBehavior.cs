using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArifBehavior : MonoBehaviour
{
   // referencia para o character system
   private CharacterSystem _char_system;

   // inicia o comportamento de Arif
   public void ArifBehaviorLoad(CharacterSystem charSystem)
   {
      _char_system = charSystem;
   }

   // comportamento da forma
   public void Behavior()
   {

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
