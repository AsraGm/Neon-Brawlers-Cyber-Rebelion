using UnityEngine;

/// <summary>
/// D1 - Sistema de Salud del Jugador (MÍNIMO FUNCIONAL)
/// Solo lo necesario para conectarse con GameManager
/// TODO: Expandir con D2, D3, D4 según el documento
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("Vida del Jugador")]
    public float vidaMaxima = 100f;
    public float vidaActual;

    private void Start()
    {
        // Inicializar vida al máximo
        vidaActual = vidaMaxima;
    }

    /// <summary>
    /// D1 - Recibir daño (básico)
    /// TODO: Expandir con integración a PostProcessManager (D2)
    /// </summary>
    public void RecibirDanio(float cantidad)
    {
        vidaActual -= cantidad;
        vidaActual = Mathf.Clamp(vidaActual, 0f, vidaMaxima);

        Debug.Log($"[PlayerHealth] Vida actual: {vidaActual}/{vidaMaxima}");

        // TODO: Descomentar cuando exista PostProcessManager (D2)
        /*
        if (PostProcessManager.Instance != null)
        {
            PostProcessManager.Instance.SetDamageVignette(vidaActual / vidaMaxima);
        }
        */

        // Verificar muerte
        if (vidaActual <= 0f)
        {
            Morir();
        }
    }

    /// <summary>
    /// D3 - Sistema de muerte
    /// TODO: Expandir con QTE (D4)
    /// </summary>
    private void Morir()
    {
        Debug.Log("[PlayerHealth] Jugador muerto");

        // TODO: Descomentar cuando exista PostProcessManager (D3)
        /*
        if (PostProcessManager.Instance != null)
        {
            PostProcessManager.Instance.ActivarVignetteMuerte();
        }
        */

        // Notificar al GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.JugadorMuerto();
        }
    }

    // ============================================
    // MÉTODOS PARA DEBUG (Eliminar en build final)
    // ============================================
#if UNITY_EDITOR
    private void Update()
    {
        // Tecla para probar daño
        if (Input.GetKeyDown(KeyCode.K))
        {
            RecibirDanio(20f);
        }
        
        // Tecla para probar muerte
        if (Input.GetKeyDown(KeyCode.L))
        {
            RecibirDanio(vidaActual);
        }
    }
#endif
}