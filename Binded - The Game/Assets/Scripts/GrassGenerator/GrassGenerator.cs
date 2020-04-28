﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer), (typeof(MeshFilter)))]
public class GrassGenerator : MonoBehaviour
{
    // determina se está no editor, este script nao deve correr em run Time
#if(UNITY_EDITOR)

    // public vars
    public int seed_ = 0; // seed para a aleatoriadade para posiçao dos vertices
    public Vector3 pad_sizer_start_ = Vector3.one; // tamanho do pad a ser calculado
    [Range(50, 100000)] public int grass_count_ = 50;  // numero de vertices da mesh
    public bool saveMesh = false;   // determina se guarda a mesh
    public string path_to_save_;    // caminha para guardar a mesh

    // private vars
    private Mesh generated_mesh_;   // mesh de vertices gerada
    private MeshFilter mesh_filter_; // mesh filter para aplicar o material com shader

    private Coroutine generation_routine;   // referencia á rotina
    private void Awake()
    {
        mesh_filter_ = GetComponent<MeshFilter>();  // guarda referencia ao mesh filter do objecto
        this.enabled = false;

    }
    private void OnEnable()
    {
        mesh_filter_ = GetComponent<MeshFilter>();  // guarda referencia ao mesh filter do objecto
        // inica a geraçao
        generation_routine = StartCoroutine(GenerateGrass());
    }

    private void OnDisable()
    {
        // ao desativar, para a routine
        StopCoroutine(generation_routine);
    }

    //metodo que determina os vertices da mesh
    private void FillMesh()
    {
        Debug.Log("Running task");
        // inicia o valor random seeded
        Random.InitState(seed_);
        // inicia lista de posiçoes com a quantidade definida para a quantidade de ervas
        List<Vector3> calculated_pos = new List<Vector3>(grass_count_);
        // lista dos uvs dos vertices
        List<Vector2> calculated_uvs = new List<Vector2>(grass_count_);

        // array dos indices dos vertices, necessario para o buffer da mesh
        List<int> calculated_index_ = new List<int>();
        //lista de normais dos vertices determinados
        List<Vector3> calculated_normals = new List<Vector3>(grass_count_);

        // para cada erva, determina a posiçao com base em colisao de ray com meshes
        for (int i = 0; i < grass_count_; i++)
        {
            // criaçao do ray de colisao
            Vector3 ray_start_ = this.transform.position;
            ray_start_.y += pad_sizer_start_.y * 0.5f;  // coloca o inico do ray na altura definida
            // distribui o valor horizontar do inicio do ray pelo tamanho do bloco definido
            ray_start_.x += pad_sizer_start_.x * Random.Range(-0.5000f, 0.5000f);
            ray_start_.z += pad_sizer_start_.z * Random.Range(-0.5000f, 0.5000f);

            // define o ray
            Ray grass_ray_ = new Ray(ray_start_, Vector3.down);
            if (Physics.Raycast(grass_ray_, out RaycastHit grass_ray_hit_, pad_sizer_start_.y))
            {
                // ao existir uma colisao valida, testa se é uma superficie que pode conter erva
                if (grass_ray_hit_.transform.CompareTag("GrassBase"))
                {
                    // se for, inicia a construçao do vertice
                    Vector3 grass_position = grass_ray_hit_.point - this.transform.position;
                    // define a posiçao do vertice locais
                    calculated_pos.Add(grass_position);
                    // determina a uv do vertice de acordo com a posiçao no rectangulo de projecçao
                    Vector2 vertexUvPos = new Vector2(Remap(grass_position.x, pad_sizer_start_.x * -0.5f, pad_sizer_start_.x * 0.5f,
                        0f, 1f), Remap(grass_position.z, pad_sizer_start_.z * -0.5f, pad_sizer_start_.z * 0.5f, 0f, 1f));

                    // adiciona o valor da uv á lista
                    calculated_uvs.Add(vertexUvPos);

                    calculated_index_.Add((int)calculated_index_.Count);
                    calculated_normals.Add(grass_ray_hit_.normal);
                }
            }
            print("Running");
        }
        // assim que estiver determinado todas as posiçoes de contacto, cria a mesh
        generated_mesh_ = new Mesh();
        generated_mesh_.name = "GrassVertex";
        generated_mesh_.SetVertices(calculated_pos);
        generated_mesh_.SetUVs(0, calculated_uvs);

        generated_mesh_.SetIndices(calculated_index_.ToArray(), MeshTopology.Points, 0);
        generated_mesh_.SetNormals(calculated_normals);

        mesh_filter_.mesh = generated_mesh_;    // atribui a mesh determinada ao filter

        Debug.Log("Finished");
    }


    // desenhar gizmos do gerador
    private void OnDrawGizmos()
    {
        // desenha a caixa de pad
        Gizmos.color = Color.green; // atribui a cor verde ao gizmos
        Gizmos.DrawWireCube(transform.position, pad_sizer_start_);
    }

    // funçao para remapear um valor de um determinado intervalo para outro
    private float Remap(float val, float from1, float to1, float from2, float to2)
    { return (val - from1) / (to1 - from1) * (to2 - from2) + from2; }


    // coroutina para geraçao da erva
    private IEnumerator GenerateGrass()
    {
        print("Running grassGenerator");


        // inicia o valor random seeded
        Random.InitState(seed_);
        // inicia lista de posiçoes com a quantidade definida para a quantidade de ervas
        List<Vector3> calculated_pos = new List<Vector3>(grass_count_);
        // lista dos uvs dos vertices
        List<Vector2> calculated_uvs = new List<Vector2>(grass_count_);

        // array dos indices dos vertices, necessario para o buffer da mesh
        List<int> calculated_index_ = new List<int>();
        //lista de normais dos vertices determinados
        List<Vector3> calculated_normals = new List<Vector3>(grass_count_);

        // numero de iteraçoes
        int bladeCount = 0;

        // para cada erva, determina a posiçao com base em colisao de ray com meshes
        for (int i = 0; i < grass_count_; i++)
        {
            // criaçao do ray de colisao
            Vector3 ray_start_ = this.transform.position;
            ray_start_.y += pad_sizer_start_.y * 0.5f;  // coloca o inico do ray na altura definida
            // distribui o valor horizontar do inicio do ray pelo tamanho do bloco definido
            ray_start_.x += pad_sizer_start_.x * Random.Range(-0.5000f, 0.5000f);
            ray_start_.z += pad_sizer_start_.z * Random.Range(-0.5000f, 0.5000f);

            // define o ray
            Ray grass_ray_ = new Ray(ray_start_, Vector3.down);
            if (Physics.Raycast(grass_ray_, out RaycastHit grass_ray_hit_, pad_sizer_start_.y))
            {
                // ao existir uma colisao valida, testa se é uma superficie que pode conter erva
                if (grass_ray_hit_.transform.CompareTag("GrassBase"))
                {
                    // se for, inicia a construçao do vertice
                    Vector3 grass_position = grass_ray_hit_.point - this.transform.position;
                    // define a posiçao do vertice locais
                    calculated_pos.Add(grass_position);
                    // determina a uv do vertice de acordo com a posiçao no rectangulo de projecçao
                    Vector2 vertexUvPos = new Vector2(Remap(grass_position.x, pad_sizer_start_.x * -0.5f, pad_sizer_start_.x * 0.5f,
                        0f, 1f), Remap(grass_position.z, pad_sizer_start_.z * -0.5f, pad_sizer_start_.z * 0.5f, 0f, 1f));

                    // adiciona o valor da uv á lista
                    calculated_uvs.Add(vertexUvPos);

                    calculated_index_.Add((int)calculated_index_.Count);
                    calculated_normals.Add(grass_ray_hit_.normal);
                }
            }
            Debug.Log("GrassBlace " + i);

            bladeCount++;
            if (bladeCount == 1000)
            {
                // espera o proximo ciclo
                yield return null;
                bladeCount = 0;
            }
        }

        // inicia uma nova mesh
        generated_mesh_ = new Mesh();
        // define o nome da mesh
        generated_mesh_.name = this.gameObject.name + "_generatedGrassPad";
        // passa os vertices determinados
        generated_mesh_.SetVertices(calculated_pos);
        // passa os uvs determinados
        generated_mesh_.SetUVs(0, calculated_uvs);
        // define os indices e o tipo de mesh
        generated_mesh_.SetIndices(calculated_index_.ToArray(), MeshTopology.Points, 0);
        // passa as normais calculadas
        generated_mesh_.SetNormals(calculated_normals);

        mesh_filter_.mesh = generated_mesh_;    // atribui a mesh determinada ao filter

        // assim que estiver determinado todas as posiçoes de contacto, cria a mesh
        print("New mesh done!");
        if (saveMesh)
        {
            string path = EditorUtility.SaveFilePanel("Save separate mesh asset", path_to_save_, generated_mesh_.name, "asset");
            path = FileUtil.GetProjectRelativePath(path);
            Mesh toSave = Object.Instantiate(mesh_filter_.sharedMesh);
            MeshUtility.Optimize(toSave);
            AssetDatabase.CreateAsset(toSave, path);
            AssetDatabase.SaveAssets();

            // define a mesh guardada
            mesh_filter_.mesh = toSave;
            // desativa o script, para que nao corra no inicio do ciclo
            this.enabled = false;
        }
    }
#endif
}
