using UnityEngine;

/// <summary>
/// Estado global de la partida. Debe existir una sola instancia durante el juego.
/// </summary>
public sealed class AdministradorJuego : MonoBehaviour
{
    public static AdministradorJuego Instancia { get; private set; }

    [SerializeField, Min(0)]
    private int disparosPorJuego = 5;

    private int disparosRealizados;

    public int DisparosPorJuego
    {
        get => disparosPorJuego;
        set => disparosPorJuego = Mathf.Max(0, value);
    }

    public int DisparosRealizados => disparosRealizados;
    public bool QuedanDisparos => disparosRealizados < DisparosPorJuego;

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        Instancia = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>Registra un disparo si la partida todavía tiene disparos disponibles.</summary>
    public bool IntentarRegistrarDisparo()
    {
        if (!QuedanDisparos)
            return false;

        disparosRealizados++;
        return true;
    }

    /// <summary>Inicia una nueva sesión con el mismo límite configurado.</summary>
    public void ReiniciarDisparos()
    {
        disparosRealizados = 0;
    }

    private void OnDestroy()
    {
        if (Instancia == this)
            Instancia = null;
    }
}
