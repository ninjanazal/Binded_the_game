using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// enumerdo dos niveis existentes
public enum LevelName
{ Gaol, Hamr, HamrBeacon, Hugr, HugrBeacon, Fylgja, FulgjaBeacon, Hamingja, HamingjaBeacon }


[CreateAssetMenu(fileName = "GameState", menuName = "Binded/GameState")]
public class GameState : ScriptableObject
{
    // variaveis de nivel
    // indicaçao em que nivel o jogador esta
    [SerializeField] private LevelName _CurrentLevel = LevelName.Gaol;
    // indica qual o proximo nivel 





    // getter publico para obter o nivel actual
    public LevelName CurrentLevel { get { return _CurrentLevel; } }
}
