using UnityEngine;

public class ItemRecolectable : MonoBehaviour
{
    [Header("=== DATOS DEL ITEM ===")]
    [SerializeField] private ItemData itemData;

    [Header("=== CONFIGURACIÓN ===")]
    [SerializeField] private float rangoDeteccion = 2f;
    [SerializeField] private KeyCode teclaRecolectar = KeyCode.E;

    [Header("=== UI OPCIONAL ===")]
    [SerializeField] private bool mostrarTextoProximidad = true;

    [Header("=== DEBUG ===")]
    [SerializeField] private bool mostrarGizmosEnGame = false;
    [SerializeField] private bool logsDetallados = false;

    private Transform jugador;
    private bool jugadorCerca = false;
    private bool yaRecolectado = false;

    private void Start()
    {
        // Validación crítica de itemData
        if (itemData == null)
        {
            Debug.LogError($"[ItemRecolectable] ❌ ERROR CRÍTICO: '{gameObject.name}' no tiene ItemData asignado. Este objeto NO funcionará.");
            enabled = false;
            return;
        }

        if (string.IsNullOrEmpty(itemData.itemID))
        {
            Debug.LogError($"[ItemRecolectable] ❌ ERROR: ItemData '{itemData.name}' no tiene itemID asignado.");
        }

        // Buscar al jugador
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            jugador = playerObj.transform;

            if (logsDetallados)
            {
                Debug.Log($"[ItemRecolectable] '{gameObject.name}' inicializado. Item: {itemData.itemID}");
            }
        }
        else
        {
            Debug.LogWarning($"[ItemRecolectable] No se encontró jugador con tag 'Player'. Item: {gameObject.name}");
        }

        if (GameManager.Instance != null)
        {
            string identificador = ObtenerIdentificadorUnico();
            GameManager.Instance.RegistrarItemEnMundo(identificador, this);

            if (logsDetallados)
            {
                Debug.Log($"[ItemRecolectable] Registrado en GameManager: {identificador}");
            }
        }
        else
        {
            Debug.LogWarning($"[ItemRecolectable] GameManager no encontrado. El sistema de persistencia no funcionará para '{gameObject.name}'");
        }

        // Verificar si este item ya fue recolectado en un checkpoint anterior
        VerificarEstadoInicial();
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            string identificador = ObtenerIdentificadorUnico();
            GameManager.Instance.DesregistrarItemEnMundo(identificador);

            if (logsDetallados)
            {
                Debug.Log($"[ItemRecolectable] Desregistrado de GameManager: {identificador}");
            }
        }
    }

    private void VerificarEstadoInicial()
    {
        if (GameManager.Instance == null) return;

        string identificador = ObtenerIdentificadorUnico();

        if (GameManager.Instance.ItemFueRecolectado(itemData.itemID, gameObject.name))
        {
            yaRecolectado = true;
            gameObject.SetActive(false);

            if (logsDetallados)
            {
                Debug.Log($"[ItemRecolectable] Item '{identificador}' ya estaba recolectado, desactivando");
            }
        }
    }

    private void Update()
    {
        if (yaRecolectado || jugador == null) return;

        // Detectar proximidad
        float distancia = Vector3.Distance(transform.position, jugador.position);
        bool estabaCerca = jugadorCerca;
        jugadorCerca = distancia <= rangoDeteccion;

        // Log cuando el jugador entra/sale del rango
        if (logsDetallados && jugadorCerca != estabaCerca)
        {
            if (jugadorCerca)
                Debug.Log($"[ItemRecolectable] Jugador entró en rango de '{gameObject.name}'");
            else
                Debug.Log($"[ItemRecolectable] Jugador salió del rango de '{gameObject.name}'");
        }

        // Recolectar
        if (jugadorCerca && Input.GetKeyDown(teclaRecolectar))
        {
            Recolectar();
        }
    }

    private void Recolectar()
    {
        yaRecolectado = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegistrarItemRecolectado(itemData.itemID, gameObject.name);
        }

        // Agregar al inventario
        if (InventoryUIManager.Instance != null)
        {
            InventoryUIManager.Instance.AgregarItem(itemData);
            Debug.Log($"[ItemRecolectable] ✅ ¡Recolectado! {itemData.nombreDisplay} ({itemData.itemID})");
        }
        else
        {
            Debug.LogError("[ItemRecolectable] ❌ No existe InventoryUIManager en la escena");
        }

        // Desactivar el objeto
        gameObject.SetActive(false);
    }

    public string ObtenerIdentificadorUnico()
    {
        if (itemData == null)
        {
            Debug.LogError($"[ItemRecolectable] ItemData es null en '{gameObject.name}'");
            return $"NULL_{gameObject.name}";
        }

        return $"{itemData.itemID}_{gameObject.name}";
    }

    public void ResetearEstado()
    {
        yaRecolectado = false;
        jugadorCerca = false;

        if (logsDetallados)
        {
            Debug.Log($"[ItemRecolectable] Estado reseteado: {gameObject.name}");
        }
    }

    [ContextMenu("Reactivar Item (DEBUG)")]
    public void Reactivar()
    {
        yaRecolectado = false;
        jugadorCerca = false;
        gameObject.SetActive(true);

        if (logsDetallados)
        {
            Debug.Log($"[ItemRecolectable] Item '{gameObject.name}' reactivado MANUALMENTE");
        }
    }

    public void EstablecerItemData(ItemData nuevoItem)
    {
        if (nuevoItem == null)
        {
            Debug.LogWarning($"[ItemRecolectable] Intentando asignar ItemData null a '{gameObject.name}'");
            return;
        }

        itemData = nuevoItem;

        if (logsDetallados)
        {
            Debug.Log($"[ItemRecolectable] ItemData cambiado a: {nuevoItem.itemID}");
        }
    }

    public ItemData ObtenerItemData()
    {
        return itemData;
    }

    public bool EstaRecolectado()
    {
        return yaRecolectado;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = jugadorCerca ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * (rangoDeteccion * 1.5f));
    }

    private void OnDrawGizmos()
    {
        if (!mostrarGizmosEnGame) return;

        Color color = jugadorCerca ? Color.green : Color.yellow;
        color.a = 0.2f;
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, rangoDeteccion);

        if (mostrarTextoProximidad && jugadorCerca && !yaRecolectado)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, rangoDeteccion * 0.5f);
        }
    }

    [ContextMenu("Validar Configuración")]
    private void ValidarConfiguracion()
    {
        Debug.Log("=== VALIDANDO ITEM RECOLECTABLE ===");

        bool esValido = true;

        // Validar ItemData
        if (itemData == null)
        {
            Debug.LogError($"❌ '{gameObject.name}': ItemData NO asignado");
            esValido = false;
        }
        else
        {
            Debug.Log($"✅ ItemData: {itemData.name} (ID: {itemData.itemID})");

            if (string.IsNullOrEmpty(itemData.itemID))
            {
                Debug.LogError($"❌ ItemData '{itemData.name}' no tiene itemID");
                esValido = false;
            }
        }

        // Validar Jugador
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogWarning($"⚠️ No se encuentra objeto con tag 'Player' en la escena");
        }
        else
        {
            Debug.Log($"✅ Jugador encontrado: {playerObj.name}");
        }

        // Validar GameManager
        if (GameManager.Instance == null)
        {
            Debug.LogWarning($"⚠️ GameManager no encontrado - el sistema de persistencia no funcionará");
        }
        else
        {
            Debug.Log($"✅ GameManager encontrado");
            Debug.Log($"📝 Identificador único: {ObtenerIdentificadorUnico()}");

            // Verificar si está registrado
            bool registrado = GameManager.Instance.ItemFueRecolectado(itemData.itemID, gameObject.name);
            Debug.Log($"📌 Estado en GameManager: {(registrado ? "RECOLECTADO" : "DISPONIBLE")}");
        }

        // Validar InventoryUIManager
        if (InventoryUIManager.Instance == null)
        {
            Debug.LogWarning($"⚠️ InventoryUIManager no encontrado");
        }
        else
        {
            Debug.Log($"✅ InventoryUIManager encontrado");
        }

        // Resumen
        if (esValido)
        {
            Debug.Log($"✅ '{gameObject.name}' está correctamente configurado");
        }
        else
        {
            Debug.LogError($"❌ '{gameObject.name}' tiene errores de configuración");
        }
    }

    [ContextMenu("Mostrar Info Detallada")]
    private void MostrarInfoDetallada()
    {
        Debug.Log("=== INFORMACIÓN DETALLADA ===");
        Debug.Log($"GameObject: {gameObject.name}");
        Debug.Log($"Activo: {gameObject.activeSelf}");
        Debug.Log($"Recolectado: {yaRecolectado}");
        Debug.Log($"Jugador cerca: {jugadorCerca}");

        if (itemData != null)
        {
            Debug.Log($"ItemData: {itemData.name}");
            Debug.Log($"ItemID: {itemData.itemID}");
            Debug.Log($"Nombre Display: {itemData.nombreDisplay}");
            Debug.Log($"Tipo: {itemData.tipo}");
        }
        else
        {
            Debug.Log("ItemData: NULL");
        }

        Debug.Log($"Identificador Único: {ObtenerIdentificadorUnico()}");
    }
}