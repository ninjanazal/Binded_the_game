using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// enumerdo dos niveis existentes
// este enumerado indica o nivel actual do jogo assim como qual cena deve ser carregada
public enum KLevelName
{
    Intro, MainMenu, FirstCutScene, Gaol, Hamr, HamrBeacon, Hugr, HugrBeacon, Fylgja, FylgjaBeacon, Hamingja, HamingjaBeacon,
    HamrGame, HarmBeaconGame, HugrGame, HugrBeaconGame, FylgjaGame, FylgjaBeaconGame, HamingjaGame, HamingjaBeaconGame, ExitCutScene
}

// enumerado para nome dos portais existentes
public enum kPortals
{ Hamr, Hugr, Fylgja, Hamingja, Exit }

[CreateAssetMenu(fileName = "GameState", menuName = "Binded/GameState")]
public class GameState : ScriptableObject
{
    public void OnEnable() { hideFlags = HideFlags.DontUnloadUnusedAsset; }

    // variaveis de nivel
    // indicaçao em que nivel o jogador esta
    public KLevelName current_level_ = KLevelName.Hamr;
    // indica qual o proximo nivel 
    private kPortals current_portal_;

    // getter publico para obter o nivel actual
    public KLevelName GetCurrentLevel { get { return current_level_; } }
    // setter public para o nivel actual
    public KLevelName SetCurrentLevel { set { current_level_ = value; } }

    // getter para obter o portal actual
    public kPortals GetCurrentPortal { get { DefinePortal(); return current_portal_; } }

    public void DefinePortal()
    {
        // determina qual o portal correcto de acordo com o proximo nivel
        if (current_level_ == KLevelName.Hamr || current_level_ == KLevelName.HamrBeacon)
            current_portal_ = kPortals.Hamr;
        else if (current_level_ == KLevelName.Hugr || current_level_ == KLevelName.HugrBeacon)
            current_portal_ = kPortals.Hugr;
        else if (current_level_ == KLevelName.Fylgja || current_level_ == KLevelName.FylgjaBeacon)
            current_portal_ = kPortals.Fylgja;
        else if (current_level_ == KLevelName.Hamingja || current_level_ == KLevelName.HamingjaBeacon)
            current_portal_ = kPortals.Hamingja;
        else if (current_level_ == KLevelName.Gaol)
            current_portal_ = kPortals.Exit;
    }

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
                break;
            case KLevelName.Hugr:
                // se o jogador completar o nivel hugr, muda o nivel actual para hugrBeacon
                current_level_ = KLevelName.HugrBeacon; // altera o nivel actual para HugrBeacon
                break;
            case KLevelName.HugrBeacon:
                // se o jogador completar o beacon de Hugr beacon, muda o nivel e o portal para Fylgja
                current_level_ = KLevelName.Fylgja;     // altera o nivel actual para fylgja
                break;
            case KLevelName.Fylgja:
                // se o jogador completar o beacon de fylgja, muda o nivel para o beacon de fylgja
                current_level_ = KLevelName.FylgjaBeacon;   // altera o nivel para o beacon de fylgja
                break;
            case KLevelName.FylgjaBeacon:
                // caso o jogador tenho completo o beacon de filgja, muda o nivel e o actual para haminja
                current_level_ = KLevelName.Hamingja;   // altera o nivel actual para filgja
                break;
            case KLevelName.Hamingja:
                // caso o jogador tenha completo o hamingja, o beacon fica disponivel
                current_level_ = KLevelName.HamingjaBeacon;
                break;
            case KLevelName.HamingjaBeacon:
                // caso o jogador tenha completo o beacon de hamingja, o jogo terminou, marca o nivel como o principal
                current_level_ = KLevelName.Gaol;
                break;
        }

        // determina qual o portal
        DefinePortal();
    }

    // retorna o index da scena actual
    public int GetCurrentLevelSceneIndex()
    {
        // retorna o index do nivel no enum, correspondendo assim ao index da cena
        return (int)current_level_;
    }
}
