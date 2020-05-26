using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// requer uma fonte de audio
[RequireComponent(typeof(AudioSource))]
public class PauseMenuManager : MonoBehaviour
{
    // public vars
    [Header("Menu Audio")]
    public AudioClip mouse_over_sound; // audio de over
    public AudioClip mouse_click;       // audio de click

    //private vars
    private AudioSource _source;    // fonte de audio do menu
    private GameObject menu_object;     // objeto de menu

    // controlo
    private bool cancel_pressed = false;    // indica se o botao ja foi carregado

    // Start is called before the first frame update
    void Start()
    {
        // inicia as variaveis
        InitializeVars();
    }

    // Update is called once per frame
    void Update()
    {
        // avalia o input do jogador
        InputConfirmation();
    }

    // metodos privados
    // iniciador das variaveis
    private void InitializeVars()
    {
        // guarda referencia para o audio
        _source = GetComponent<AudioSource>();

        // guarda referencia para o objecto de menu
        menu_object = this.transform.GetChild(0).gameObject;

        // ao iniciar o menu de pausa desliga o menu e esconde o rato
        menu_object.SetActive(false);
        // bloqueia o cursor e escond
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // avaliador de input
    private void InputConfirmation()
    {
        // verifica se o jogador carregou no "esc"
        if (Input.GetAxisRaw("Cancel") != 0 && !cancel_pressed)
        {
            // dependendo se o menu está aberto ou nao
            if (!menu_object.activeSelf)
                OpenPauseMenu();
            else
                ClosePauseMenu();
            // indica que o botao foi pressionado
            cancel_pressed = true;
        }
        // verifica quando o botao é largado
        else if (Input.GetAxisRaw("Cancel") == 0)
            // indica que o jogador largou o botao
            cancel_pressed = false;

    }

    // Resposta para o input
    // para abrir o menu
    private void OpenPauseMenu()
    {

        // ao abrir o menu mostra o menu
        menu_object.SetActive(true);
        // liberta o cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // para a escala do tempo 
        Time.timeScale = 0f;
    }

    // para fechar o menu
    private void ClosePauseMenu()
    {
        // esconde o menu
        menu_object.SetActive(false);
        // trava e esconde o cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }
    // metodos public
    // chamada quando o rato está sobre um botao
    public void OnMouseOverHandler()
    {
        // ao passar o rato sobre o botao deve iniciar o audio
        _source.PlayOneShot(mouse_over_sound);
    }

    // Chamada quando o botao continuar é pressionado
    public void OnContinuePressed() => ClosePauseMenu();
    // chamado quando o botao sair é chamado
    public void OnExitPressed()
    {
        // liberta as açoes do rato
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // restaura a escala do tempo
        Time.timeScale = 1f;

        // chama o inumerador para trocar a cena
        IEnumeratorCallBacks.Instance.LoadNewScene((int)KLevelName.MainMenu);
    }
}
