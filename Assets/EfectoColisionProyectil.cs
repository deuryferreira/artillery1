using UnityEngine;

/// <summary>Emite un efecto al chocar el proyectil y luego lo elimina.</summary>
[RequireComponent(typeof(Collider))]
public sealed class EfectoColisionProyectil : MonoBehaviour
{
    [SerializeField] private GameObject efectoExplosion;
    [SerializeField, Min(0f)] private float tiempoAntesDeDestruir = 5f;
    [SerializeField] private AudioClip sonidoImpacto;

    private bool yaImpacto;

    public void Configurar(GameObject prefabExplosion, AudioClip clipImpacto)
    {
        efectoExplosion = prefabExplosion;
        sonidoImpacto = clipImpacto;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (yaImpacto)
            return;

        yaImpacto = true;

        if (sonidoImpacto != null)
        {
            Vector3 posicionAudio = collision.contactCount > 0
                ? collision.GetContact(0).point
                : transform.position;
            AudioSource.PlayClipAtPoint(sonidoImpacto, posicionAudio, 0.8f);
        }

        // Solo se destruyen los objetos marcados como obstáculos; el suelo y el cañón permanecen.
        if (collision.collider.CompareTag("Obstaculo"))
            Destroy(collision.collider.gameObject);

        if (efectoExplosion != null)
        {
            Vector3 posicion = collision.contactCount > 0
                ? collision.GetContact(0).point
                : transform.position;
            Quaternion rotacion = collision.contactCount > 0
                ? Quaternion.LookRotation(collision.GetContact(0).normal)
                : Quaternion.identity;

            GameObject efecto = Instantiate(efectoExplosion, posicion, rotacion);
            Destroy(efecto, tiempoAntesDeDestruir);
        }

        Destroy(gameObject);
    }
}
