using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Punto de integración del cañón con el límite global de disparos.
/// </summary>
public sealed class Canon : MonoBehaviour
{
    [Header("Control y disparo")]
    [SerializeField] private Transform pivoteCanon;
    [SerializeField, Min(1f)] private float fuerzaDisparo = 18f;
    [SerializeField, Min(0.05f)] private float radioBala = 0.22f;
    [SerializeField] private GameObject efectoExplosion;
    [SerializeField] private Camera camara;
    [SerializeField] private AudioClip sonidoDisparo;
    [SerializeField] private AudioClip sonidoImpacto;

    [Header("Efecto al disparar")]
    [SerializeField] private GameObject efectoDisparo;
    [SerializeField] private Transform puntoDisparo;

    private void Awake()
    {
        AudioSource fuente = GetComponent<AudioSource>();
        if (fuente == null)
            fuente = gameObject.AddComponent<AudioSource>();
        fuente.playOnAwake = false;
        fuente.spatialBlend = 0f;

        if (pivoteCanon == null)
        {
            Transform encontrado = transform.Find("PivoteCanon");
            if (encontrado != null)
                pivoteCanon = encontrado;
        }

        if (camara == null)
            camara = Camera.main;
    }

    private void Update()
    {
        if (Time.timeScale == 0f)
            return;
        ApuntarConRaton();

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            Disparar();
        else if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            Disparar();
    }

    private void ApuntarConRaton()
    {
        if (pivoteCanon == null || camara == null || Mouse.current == null)
            return;

        Vector2 posicionRaton = Mouse.current.position.ReadValue();
        float distanciaCamara = Mathf.Abs(camara.transform.position.z - pivoteCanon.position.z);
        Vector3 objetivo = camara.ScreenToWorldPoint(
            new Vector3(posicionRaton.x, posicionRaton.y, distanciaCamara));
        Vector2 direccion = objetivo - pivoteCanon.position;

        if (direccion.sqrMagnitude > 0.001f)
        {
            float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg - 90f;
            pivoteCanon.rotation = Quaternion.Euler(0f, 0f, angulo);
        }
    }

    public void Disparar()
    {
        if (!IntentarDisparar())
            return;

        Transform direccionCanon = pivoteCanon != null ? pivoteCanon : transform;
        Vector3 direccion = direccionCanon.up;
        Vector3 origen = puntoDisparo != null
            ? puntoDisparo.position
            : direccionCanon.position + direccion * 1.25f;
        // Mantiene la bala en el plano de los obstáculos del nivel.
        origen.z = 1.2f;

        GameObject bala = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bala.name = "BalaCanon";
        bala.transform.position = origen;
        bala.transform.localScale = Vector3.one * (radioBala * 2f);

        Rigidbody cuerpo = bala.AddComponent<Rigidbody>();
        cuerpo.mass = 0.5f;
        cuerpo.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        cuerpo.interpolation = RigidbodyInterpolation.Interpolate;
        cuerpo.AddForce(direccion * fuerzaDisparo, ForceMode.Impulse);

        EfectoColisionProyectil colision = bala.AddComponent<EfectoColisionProyectil>();
        colision.Configurar(efectoExplosion, sonidoImpacto);
        Destroy(bala, 12f);
    }

    /// <summary>Actualiza la fuerza desde el Slider de la interfaz.</summary>
    public void ModificarFuerza(float valor)
    {
        fuerzaDisparo = Mathf.Max(1f, valor);
    }

    /// <summary>
    /// Llama este método justo antes de crear el proyectil. Devuelve false al alcanzar el límite.
    /// </summary>
    public bool IntentarDisparar()
    {
        bool disparoRegistrado = AdministradorJuego.Instancia != null &&
                                 AdministradorJuego.Instancia.IntentarRegistrarDisparo();

        if (disparoRegistrado && sonidoDisparo != null)
        {
            AudioSource fuente = GetComponent<AudioSource>();
            if (fuente != null)
                fuente.PlayOneShot(sonidoDisparo);
        }

        if (disparoRegistrado && efectoDisparo != null)
        {
            Transform direccionCanon = pivoteCanon != null ? pivoteCanon : transform;
            Vector3 posicion = puntoDisparo != null
                ? puntoDisparo.position
                : direccionCanon.position + direccionCanon.up * 1.25f;
            posicion.z = 1.2f;
            GameObject efecto = Instantiate(efectoDisparo, posicion,
                Quaternion.LookRotation(Vector3.forward, pivoteCanon != null ? pivoteCanon.up : transform.up));
            Destroy(efecto, 5f);
        }

        return disparoRegistrado;
    }
}
