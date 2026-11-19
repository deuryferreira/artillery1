using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>Controla el menú, la pausa y el fin de partida del prototipo.</summary>
public sealed class InterfazJuego : MonoBehaviour
{
    [SerializeField] private Slider sliderFuerza;
    [SerializeField] private GameObject panelMenu;
    [SerializeField] private Text textoTitulo;
    [SerializeField] private Text textoDisparos;
    private enum Estado { Inicio, Jugando, Pausa, Fin }
    private Estado estado = Estado.Inicio;
    private Canon canon;

    public void Configurar(Slider slider, GameObject menu, Text disparos)
    { sliderFuerza = slider; panelMenu = menu; textoDisparos = disparos; }
    public void ConfigurarTitulo(Text titulo) { textoTitulo = titulo; ActualizarMenu(); }

    private void Start()
    {
        if (AdministradorJuego.Instancia != null)
            AdministradorJuego.Instancia.ReiniciarDisparos();
        canon = FindObjectOfType<Canon>();
        if (sliderFuerza != null)
        {
            sliderFuerza.minValue = 5f;
            sliderFuerza.maxValue = 40f;
            sliderFuerza.value = 18f;
            sliderFuerza.onValueChanged.AddListener(ModificarFuerza);
            ModificarFuerza(sliderFuerza.value);
        }
        Time.timeScale = 0f;
        ActualizarMenu();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (estado == Estado.Jugando) { estado = Estado.Pausa; Time.timeScale = 0f; ActualizarMenu(); }
            else if (estado == Estado.Pausa) Jugar();
        }
        if (estado == Estado.Jugando && AdministradorJuego.Instancia != null && !AdministradorJuego.Instancia.QuedanDisparos)
        { estado = Estado.Fin; Time.timeScale = 0f; ActualizarMenu(); }
        if (textoDisparos != null && AdministradorJuego.Instancia != null)
            textoDisparos.text = "Disparos: " + AdministradorJuego.Instancia.DisparosRealizados + "/" + AdministradorJuego.Instancia.DisparosPorJuego;
    }

    public void ModificarFuerza(float valor) { if (canon != null) canon.ModificarFuerza(valor); }
    public void Jugar() { estado = Estado.Jugando; Time.timeScale = 1f; ActualizarMenu(); }
    public void Reiniciar() { Time.timeScale = 1f; SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
    public void Salir() { Application.Quit(); }

    private void ActualizarMenu()
    {
        if (panelMenu != null) panelMenu.SetActive(estado != Estado.Jugando);
        if (textoTitulo != null) textoTitulo.text = estado == Estado.Inicio ? "ARTILLERY" : estado == Estado.Pausa ? "PAUSA" : estado == Estado.Fin ? "FIN DE PARTIDA" : "";
    }
    private void OnDestroy() { if (sliderFuerza != null) sliderFuerza.onValueChanged.RemoveListener(ModificarFuerza); Time.timeScale = 1f; }
}
