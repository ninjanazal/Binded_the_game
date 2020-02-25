using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerShape { Aike, Arif }

[CreateAssetMenu(fileName = "CharacterInformation", menuName = "Binded/CharacterInformation")]
public class CharacterInfo : ScriptableObject
{
      // fields -----------------------------------------
      // input de direcçao do jogador pela camera
      private Vector3 _input_cam_dir;

      // informaçao sobre a forma do jogador
      [Header("Forma actual do jogador")]
      public PlayerShape shape;

      #region AikeVars
      // variavies relacionada com Aike
      [Header("Velocidade Maxima")]
      [Header("Aike")]
      public float AikeMaxSpeed = 3.0f;
      // aceleraçao
      [Header("Aceleraçao")]
      public float AikeAceleration = 3.0f;
      // velocidade de rotaçao
      [Header("Velocidade de rotaçao")]
      public float AikeRotationSpeed = 5f;      
      // drag de movimento
      [Header("Drag de movimento")]
      public float AikeDrag = 5f;
      #endregion

      #region ArifVars
      // variavies relacionada com Aike
      [Header("Velocidade Maxima")]
      [Header("Aike")]
      public float ArifMaxSpeed = 10.0f;
      // aceleraçao
      [Header("Aceleraçao")]
      public float ArifAceleration = 5.0f;
      // velocidade de rotaçao
      [Header("Velocidade de rotaçao")]
      public float ArifRotationSpeed = 4f;
      // drag de movimento
      [Header("Drag do jogador")]
      public float ArifDrag = 6f;
      #endregion

      // metodos  --------------------------------
      #region Methods
      // altera a forma do jogador
      /// <summary>
      /// Switch the player shaper
      ///</summary>
      public void changeShape()
      {
            // altera para a forma nao actual
            shape = (shape == PlayerShape.Aike) ? PlayerShape.Arif : PlayerShape.Aike;
      }

      // atribui e retorna valor de direcçao de input
      public void UpdateInputDir(Vector3 direction) => _input_cam_dir = direction;
      public Vector3 GetInputDir() { return _input_cam_dir; }

      #endregion
}
