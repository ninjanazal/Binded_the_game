using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerShape { rat, owl }

[CreateAssetMenu(fileName = "CharacterInformation", menuName = "Binded/CharacterInformation")]
public class CharacterInfo : ScriptableObject
{
      // input de direcçao do jogador pela camera
      private Vector3 _input_cam_dir;

      // informaçao sobre a forma do jogador
      [Header("Forma actual do jogador")]
      public PlayerShape shape;

      // altera a forma do jogador
      /// <summary>
      /// Switch the player shaper
      ///</summary>
      public void changeShape()
      {
            // altera para a forma nao actual
            shape = (shape == PlayerShape.rat) ? PlayerShape.rat : PlayerShape.rat;
      }

      // atribui e retorna valor de direcçao de input
      public void UpdateInputDir(Vector3 direction) => _input_cam_dir = direction;
      public Vector3 GetInputDir() { return _input_cam_dir; }
}
