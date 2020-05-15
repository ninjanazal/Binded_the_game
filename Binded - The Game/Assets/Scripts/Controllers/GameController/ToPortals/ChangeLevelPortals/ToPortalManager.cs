using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToPortalManager : MonoBehaviour
{
    [Header("Variaveis do portal")]
    public GameState game_state_;   // referencia para o controlador de estado do jogogo
    public KLevelName teleportTo;   // varialvel regista para que nivel o portal teleporta
    public LevelInfo level_infor_;  // informaçao do nivel em que o portal se encontra
    public int orbDelivered = 0;   // indica o valor de orbs recebidas
    [Header("Informaçao de orbs entregues")]
    public GameObject diplay_orb;   // objecto que é utilizado para mostrar as orbs em falta
    public float distance_to_center;    // distancia ao centro do objecto

    // variaveis internas
    private bool isActivated = false;   // variavel interna para determinar se o protal está activo
    private OrbTriggerManager orb_trigger_manager_; // referencia para o manager de orbs
    [SerializeField] private Transform display_point_center;   // posiçao do centro para mostra as orbs que faltam
    private float angle_between_orbs;   // angulo entre orbs
    private GameObject[] display_orbs;  // array de orbs a serem mostradas

    // ao iniciar regista regista o portal de orbs
    private void Start()
    {
        // guarda referencia para o trigger manager de orbs
        orb_trigger_manager_ = GetComponentInChildren<OrbTriggerManager>();
        orb_trigger_manager_.RegistPortal(this);    // regista o portal
        // confirma o contador de orbs entregues no asset de nivel
        level_infor_.DeliveredEnergy = 0;

        // prepara o display de orbs
        SetUpdDisplayOrbSystem();
    }

    // metodo chamado quando o jogador entrou no portal
    public void OnPlayerEnterCallBack()
    {
        // ao ser chamado, deve iniciar a transiçao para a cena de teleportTO
        Debug.Log("PlayerEntered");
        Debug.Log($"Teleporting to {teleportTo}");

        // indica ao controlador do jogo que o nivel foi completo
        game_state_.CompletedLevel();
        // inicia a call para mudar de cena
        IEnumeratorCallBacks.Instance.LoadNewScene((int)teleportTo);
    }

    // metodo chamado quando uma orb entra no portal
    public void OnOrbEnterCallBack()
    {
        // muda a cor da orb de indicaçao
        display_orbs[orbDelivered].transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);

        // ao entregar uma orb, incrementa o valor
        orbDelivered++;
        level_infor_.DeliveredEnergy = orbDelivered;
        // determina se o valor de orbs entregues corresponde ao numero de activaçao
        isActivated = (orbDelivered >= level_infor_.EnergyRequired) ? ActivatePortal() : false;
    }
    // metodo privado para iniciar as orbs a serem mostradas
    private void SetUpdDisplayOrbSystem()
    {
        // determina o angulo entre orbs
        angle_between_orbs = 360f / level_infor_.EnergyRequired;
        // inicia os arrays das orbs
        display_orbs = new GameObject[level_infor_.EnergyRequired];
        // para cada uma das orbs determina a posiçao a instanciar
        for (int i = 0; i < display_orbs.Length; i++)
        {
            // instancia a orb 
            display_orbs[i] = GameObject.Instantiate(diplay_orb, this.transform);

            // para cada uma das orbs
            // determina a posiçao da orb com base na rotaçao entre cada uma delas e distancia ao centro
            display_orbs[i].transform.position = display_point_center.TransformPoint(
                Quaternion.AngleAxis(angle_between_orbs * i, display_point_center.up) * display_point_center.forward * distance_to_center);
        }

    }

    // metodos internos
    private bool ActivatePortal()
    {
        // remove o registo para receber orbs
        orb_trigger_manager_.UnregistPortal(this);
        // metodo ao ser chamado deve activar o portal, para tal basta registar o script
        GetComponentInChildren<PortalTriggerManager>().RegistPortalManager(this);
        // retorna que o portal foi activado
        return true;
    }
}
