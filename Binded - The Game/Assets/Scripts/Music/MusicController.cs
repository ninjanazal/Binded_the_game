using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    private static MusicController _instance;  // declaraçao de um singleton estatico
    public static MusicController Instance { get { return _instance; } }
    // variaveis publicas
    public AudioClip[] music_;  // musicas do jogo

    // variaveis privadas
    private AudioSource audioSource;

    // Start is called before the first frame update
    private void OnEnable()
    {
        // indica que o objecto nao deve ser destroido ao carregar uma scena
        Object.DontDestroyOnLoad(this);

        //  confirmaçao do singleton
        if (MusicController.Instance == null)
            _instance = this;
        else if (MusicController.Instance != this)
            Destroy(this.gameObject);

        // inicia o player 
        audioSource = GetComponent<AudioSource>();

        // inicia uma musica
        audioSource.clip = GetRandomClip();
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        // caso o player nao esteja a tocar
        if (!audioSource.isPlaying)
        {
            // determina uma musica aleatoria e toca
            audioSource.clip = GetRandomClip();
            audioSource.Play();
        }
    }

    // funçoes privadas
    // retorna uma musica aleatoria da lista
    private AudioClip GetRandomClip() { return music_[Random.Range(0, music_.Length)]; }

}
