using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Sistema de inspección 3D integrado con inventario
/// Usa RenderTexture para mostrar el modelo en UI
/// </summary>
public class InspectSystem : MonoBehaviour
{
    #region Singleton
    public static InspectSystem Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    #endregion

    [Header("=== REFERENCIAS UI ===")]
    [SerializeField] private GameObject panelInspector;
    [SerializeField] private RawImage imagenRender; // Donde se muestra el modelo
    [SerializeField] private TextMeshProUGUI textoNombreItem;
    [SerializeField] private Button botonCerrar;

    [Header("=== RENDER 3D ===")]
    [SerializeField] private Camera camaraRender;
    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private Transform puntoSpawn; // Donde aparece el modelo
    [SerializeField] private Light luzModelo; // Luz para iluminar el modelo

    [Header("=== ROTACIÓN ===")]
    [SerializeField] private float rotationSpeed = 100f;

    [Header("=== ZOOM ===")]
    [SerializeField] private float zoomMin = 2f;
    [SerializeField] private float zoomMax = 6f;
    [SerializeField] private float zoomSpeed = 2f;
    private float currentZoom = 4f;

    [Header("=== KEYBINDS ===")]
    [SerializeField] private KeyCode teclaCerrar = KeyCode.Escape;

    // Variables internas
    private GameObject modeloActual;
    private ItemData itemActual;
    private bool panelAbierto = false;
    private Vector3 previousMousePosition;

    private void Start()
    {
        CerrarPanel();

        // Configurar botón
        if (botonCerrar != null)
            botonCerrar.onClick.AddListener(CerrarPanel);

        // Configurar RenderTexture en la imagen
        if (imagenRender != null && renderTexture != null)
        {
            imagenRender.texture = renderTexture;
        }

        // Desactivar cámara de render al inicio
        if (camaraRender != null)
        {
            camaraRender.enabled = false;
        }

        // Desactivar luz al inicio
        if (luzModelo != null)
        {
            luzModelo.enabled = false;
        }
    }

    private void Update()
    {
        if (!panelAbierto) return;

        // Cerrar con tecla
        if (Input.GetKeyDown(teclaCerrar))
        {
            CerrarPanel();
        }

        // Rotación con mouse (tu lógica original)
        if (modeloActual != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                previousMousePosition = Input.mousePosition;
            }

            if (Input.GetMouseButton(0))
            {
                Vector3 deltaMousePosition = Input.mousePosition - previousMousePosition;
                float rotationX = deltaMousePosition.y * rotationSpeed * Time.deltaTime;
                float rotationY = -deltaMousePosition.x * rotationSpeed * Time.deltaTime;

                Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0f);
                modeloActual.transform.rotation = rotation * modeloActual.transform.rotation;

                previousMousePosition = Input.mousePosition;
            }
        }

        // Zoom con scroll
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f && camaraRender != null)
        {
            currentZoom -= scroll * zoomSpeed;
            currentZoom = Mathf.Clamp(currentZoom, zoomMin, zoomMax);

            // Mover cámara hacia adelante/atrás
            camaraRender.transform.localPosition = new Vector3(0, 0, -currentZoom);
        }
    }

    /// <summary>
    /// Abre el inspector con un item del inventario
    /// </summary>
    public void AbrirInspector(ItemData item)
    {
        if (item == null)
        {
            Debug.LogWarning("[InspectSystem] Item es null");
            return;
        }

        if (item.modelo3D == null)
        {
            Debug.LogWarning($"[InspectSystem] Item '{item.nombreDisplay}' no tiene modelo 3D asignado");
            return;
        }

        itemActual = item;
        panelAbierto = true;
        panelInspector.SetActive(true);

        // Mostrar nombre
        if (textoNombreItem != null)
        {
            textoNombreItem.text = item.nombreDisplay;
        }

        // ✅ OCULTAR HIGHLIGHT
        if (InventoryUIManager.Instance != null && InventoryUIManager.Instance.highlightObject != null)
        {
            InventoryUIManager.Instance.highlightObject.SetActive(false);
        }

        // Activar cámara y luz
        if (camaraRender != null)
        {
            camaraRender.enabled = true;
        }
        if (luzModelo != null)
        {
            luzModelo.enabled = true;
        }

        // Instanciar modelo
        if (puntoSpawn != null)
        {
            // Destruir modelo anterior si existe
            if (modeloActual != null)
            {
                Destroy(modeloActual);
            }

            modeloActual = Instantiate(item.modelo3D, puntoSpawn.position, Quaternion.identity);
            modeloActual.transform.SetParent(puntoSpawn);
            modeloActual.transform.localPosition = Vector3.zero;
            modeloActual.transform.localRotation = Quaternion.identity;

            Renderer[] renderers = modeloActual.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.receiveShadows = false;
            }

            // Resetear zoom
            currentZoom = 4f;
            if (camaraRender != null)
            {
                camaraRender.transform.localPosition = new Vector3(0, 0, -currentZoom);
            }
        }

        Debug.Log($"[InspectSystem] Inspector abierto para: {item.nombreDisplay}");
    }

    public void CerrarPanel()
    {
        panelAbierto = false;
        panelInspector.SetActive(false);

        // Desactivar cámara y luz
        if (camaraRender != null)
        {
            camaraRender.enabled = false;
        }
        if (luzModelo != null)
        {
            luzModelo.enabled = false;
        }

        // Destruir modelo
        if (modeloActual != null)
        {
            Destroy(modeloActual);
            modeloActual = null;
        }

        itemActual = null;

        // ✅ REACTIVAR HIGHLIGHT
        if (InventoryUIManager.Instance != null)
        {
            InventoryUIManager.Instance.ActualizarHighlightPublico();
        }

        Debug.Log("[InspectSystem] Inspector cerrado");
    }

    /// <summary>
    /// Verifica si el panel está abierto (para bloquear navegación)
    /// </summary>
    public bool PanelEstaAbierto()
    {
        return panelAbierto;
    }
}