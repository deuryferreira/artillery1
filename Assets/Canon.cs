using UnityEngine;

/// <summary>
/// Punto de integración del cañón con el límite global de disparos.
/// </summary>
public sealed class Canon : MonoBehaviour
{
    /// <summary>
    /// Llama este método justo antes de crear el proyectil. Devuelve false al alcanzar el límite.
    /// </summary>
    public bool IntentarDisparar()
    {
        return AdministradorJuego.Instancia != null &&
               AdministradorJuego.Instancia.IntentarRegistrarDisparo();
    }
}
