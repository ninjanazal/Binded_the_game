using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="LevelName",menuName ="Binded/Level")]
public class LevelInfo : ScriptableObject
{
    // defeniçao do nome do nivel
    public KLevelName LevelName;

    // energia para abrir o portal
    public int EnergyRequired;

}
