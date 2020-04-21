using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbSpawnerManager : MonoBehaviour
{
    // manager que controla o spawn de orbs
    // variaveis publicas
    [Header("Referencias gerais")]
    public LevelInfo level_infor_;  // referencia para as definiçoes do nivel
    public OrbSpawnArea[] spawn_Areas_; // array com as areas de spawn presentes
    [Space(5f)] public GameObject orb_to_spawn_;    // objecto a ser instanciado
    [Range(1f, 3f)] public float spawnAmountPercent = 1f;    //percentagem relacionada com as orbs necessarias 


    //variaveis internas
    private int total_inicial_spawn_count_; // numero inicial de spawns de orbs

    void Start()
    {
        // ao iniciar deve começar o spawn das orbs pelo mapa
        // determinar o numero de orbs relacionado com a percentagem inserida
        total_inicial_spawn_count_ = Mathf.RoundToInt(level_infor_.EnergyRequired * spawnAmountPercent);

        // inicia o spawn de orbs
        StartCoroutine(StartSpawningOrbs());
    }

    // call para spawnar uma nova particula
    public void SpawnNewOrb()
    {
        Debug.Log("New orb spawning");
        // instancia uma orb numa area e posiçao aleatoria dentro dessa area
        OrbBehaviour new_orb_ = Instantiate(orb_to_spawn_, spawn_Areas_[Random.Range(0, spawn_Areas_.Length)].GetRandomPosInside(),
           Quaternion.identity).GetComponent<OrbBehaviour>();

        // regista o spawner na nova orb
        new_orb_.RegistSpawnManager(this);
    }


    // coroutina para instanciaçao das orbs
    private IEnumerator StartSpawningOrbs()
    {
        // instancia o numero de orbs determinado
        for (int i = 0; i < total_inicial_spawn_count_; i++)
        {
            // instancia uma orb numa area e posiçao aleatoria dentro dessa area
            OrbBehaviour new_orb_ = Instantiate(orb_to_spawn_, spawn_Areas_[Random.Range(0, spawn_Areas_.Length)].GetRandomPosInside(),
                Quaternion.identity).GetComponent<OrbBehaviour>();

            // regista o spawner na nova orb
            new_orb_.RegistSpawnManager(this);

            // aguarda o proximo frame
            yield return null;
        }
    }

}
