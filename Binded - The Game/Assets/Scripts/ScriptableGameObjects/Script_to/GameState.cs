using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// enumerdo dos niveis existentes
public enum KLevelName
{ Gaol, Hamr, HamrBeacon, Hugr, HugrBeacon, Fylgja, FylgjaBeacon, Hamingja, HamingjaBeacon }
// enumerado para nome dos portais existentes
public enum kPortals
{ Hamr, Hugr, Fylgja, Hamingja, Exit }

[CreateAssetMenu(fileName = "GameState", menuName = "Binded/GameState")]
public class GameState : ScriptableObject
{
    // variaveis de nivel
    // indicaçao em que nivel o jogador esta
    [SerializeField] private KLevelName current_level_ = KLevelName.Hamr;
    // indica qual o proximo nivel 
    [SerializeField] private kPortals current_portal_ = kPortals.Hamr;

    // getter publico para obter o nivel actual
    public KLevelName GetCurrentLevel { get { return current_level_; } }
    // getter para obter o portal actual
    public kPortals GetCurrentPortal { get { return current_portal_; } }


    // metodos publics para avançar nos niveis
    public void CompletedLevel()
    {
        // dependendo do nivel em que o jogo se encontra
        switch (current_level_)
        {
            case KLevelName.Hamr:
                // se o jogador completar o nivel Hamr, muda o nivel actual para o beacon
                current_level_ = KLevelName.HamrBeacon;  // altera o nivel actual para hamrBeacon
                break;
            case KLevelName.HamrBeacon:
                // se o jogador completar o beacon de Hamr, muda o nivel actual para Hugr, e altera tambem
                // o portal
                current_level_ = KLevelName.Hugr;    // altera o nivel atual para Hugr
                current_portal_ = kPortals.Hugr;    // altera o portal aberto para hugr
                break;
            case KLevelName.Hugr:
                // se o jogador completar o nivel hugr, muda o nivel actual para hugrBeacon
                current_level_ = KLevelName.HugrBeacon; // altera o nivel actual para HugrBeacon
                break;
            case KLevelName.HugrBeacon:
                // se o jogador completar o beacon de Hugr beacon, muda o nivel e o portal para Fylgja
                current_level_ = KLevelName.Fylgja;     // altera o nivel actual para fylgja
                current_portal_ = kPortals.Fylgja;      // altera o portal aberto para fylgja
                break;
            case KLevelName.Fylgja:
                // se o jogador completar o beacon de fylgja, muda o nivel para o beacon de fylgja
                current_level_ = KLevelName.FylgjaBeacon;   // altera o nivel para o beacon de fylgja
                break;
            case KLevelName.FylgjaBeacon:
                // caso o jogador tenho completo o beacon de filgja, muda o nivel e o actual para haminja
                current_level_ = KLevelName.Hamingja;   // altera o nivel actual para filgja
                current_portal_ = kPortals.Hamingja;    // altera o portal aberto para hamingja
                break;
            case KLevelName.Hamingja:
                // caso o jogador tenha completo o hamingja, o beacon fica disponivel
                current_level_ = KLevelName.HamingjaBeacon;
                break;
            case KLevelName.HamingjaBeacon:
                // caso o jogador tenha completo o beacon de hamingja, o jogo terminou, marca o nivel como o principal
                current_level_ = KLevelName.Gaol;
                current_portal_ = kPortals.Exit;
                break;
        }
    }
}
