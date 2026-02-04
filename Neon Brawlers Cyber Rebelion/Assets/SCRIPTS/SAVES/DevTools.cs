using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class DevTools : MonoBehaviour
{
    [Header("=== HERRAMIENTAS DE DESARROLLO ===")]
    [SerializeField] private bool mostrarLogsDetallados = true;

    [Header("=== OPCIONES DE RESETEO ===")]
    [SerializeField] private bool resetearAlIniciarJuego = false;
    [Tooltip("Si está activo, resetea automáticamente cuando das Play en Unity")]

    #region BOTONES EN INSPECTOR
    
    [Header("=== BOTONES DE RESETEO ===")]
    [SerializeField] private bool _botonPlaceholder = false;

    #endregion

    #if UNITY_EDITOR
    private void Start()
    {
        // Auto-resetear si está configurado
        if (resetearAlIniciarJuego)
        {
            Debug.LogWarning("[DevTools] ⚠️ Reseteo automático activado - Limpiando todo al iniciar...");
            ResetearTodo();
        }
        else
        {
            Debug.Log("[DevTools] ✅ DevTools activo. Usa los botones del Inspector o el menú Tools.");
        }
    }
    #endif

    // ========================================
    // MÉTODOS PÚBLICOS (llamados desde Inspector)
    // ========================================

    /// <summary>
    /// 🔄 RESETEA TODO: Checkpoint, Inventario y Progreso de Misiones
    /// </summary>
    [ContextMenu("🔄 RESETEAR TODO")]
    public void ResetearTodo()
    {
        #if UNITY_EDITOR
        Debug.Log("=== [DevTools] RESETEANDO TODO ===");
        
        ResetearCheckpoint();
        ResetearInventario();
        ResetearMisiones();
        ResetearItemsDelMundo();
        
        Debug.Log("=== [DevTools] ✅ RESETEO COMPLETO FINALIZADO ===");
        
        EditorUtility.DisplayDialog(
            "Reseteo Completo", 
            "✅ Se ha reseteado:\n\n• Checkpoint guardado\n• Inventario\n• Progreso de misiones\n• Items del mundo\n\n¡Todo listo para comenzar desde cero!", 
            "OK"
        );
        #else
        Debug.LogWarning("[DevTools] ⚠️ Esta función solo funciona en el Editor de Unity");
        #endif
    }

    /// <summary>
    /// 💾 RESETEA solo el Checkpoint guardado
    /// </summary>
    [ContextMenu("💾 Resetear Checkpoint")]
    public void ResetearCheckpoint()
    {
        #if UNITY_EDITOR
        if (GameManager.Instance != null)
        {
            // Borrar checkpoint
            GameManager.Instance.BorrarCheckpoint();
            
            if (mostrarLogsDetallados)
            {
                Debug.Log("[DevTools] 💾 Checkpoint reseteado");
            }
        }
        else
        {
            Debug.LogWarning("[DevTools] ⚠️ GameManager no encontrado en la escena");
        }
        #endif
    }

    /// <summary>
    /// 🎒 RESETEA solo el Inventario
    /// </summary>
    [ContextMenu("🎒 Resetear Inventario")]
    public void ResetearInventario()
    {
        #if UNITY_EDITOR
        if (InventoryUIManager.Instance != null)
        {
            InventoryUIManager.Instance.LimpiarInventarioCompleto();
            
            if (mostrarLogsDetallados)
            {
                Debug.Log("[DevTools] 🎒 Inventario limpiado");
            }
        }
        else
        {
            Debug.LogWarning("[DevTools] ⚠️ InventoryUIManager no encontrado en la escena");
        }
        #endif
    }

    /// <summary>
    /// 📋 RESETEA solo el Progreso de Misiones
    /// </summary>
    [ContextMenu("📋 Resetear Misiones")]
    public void ResetearMisiones()
    {
        #if UNITY_EDITOR
        if (ObjetivoManager.Instance != null)
        {
            // Volver a la misión 0
            ObjetivoManager.Instance.CargarMision(0);
            
            if (mostrarLogsDetallados)
            {
                Debug.Log("[DevTools] 📋 Progreso de misiones reseteado a Misión #0");
            }
        }
        else
        {
            Debug.LogWarning("[DevTools] ⚠️ ObjetivoManager no encontrado en la escena");
        }
        #endif
    }

    /// <summary>
    /// 🌍 RESETEA Items del Mundo (reactiva todos los items recolectables)
    /// </summary>
    [ContextMenu("🌍 Resetear Items del Mundo")]
    public void ResetearItemsDelMundo()
    {
        #if UNITY_EDITOR
        // Buscar TODOS los ItemRecolectable en la escena
        ItemRecolectable[] todosLosItems = FindObjectsOfDay<ItemRecolectable>(true);
        
        int itemsReactivados = 0;
        
        foreach (ItemRecolectable item in todosLosItems)
        {
            item.ResetearEstado();
            item.gameObject.SetActive(true);
            itemsReactivados++;
        }
        
        if (mostrarLogsDetallados)
        {
            Debug.Log($"[DevTools] 🌍 {itemsReactivados} items del mundo reseteados y reactivados");
        }
        
        // También limpiar el registro en GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LimpiarRegistroItems();
        }
        #endif
    }

    /// <summary>
    /// 🔍 MUESTRA información de debug del estado actual
    /// </summary>
    [ContextMenu("🔍 Mostrar Estado Actual")]
    public void MostrarEstadoActual()
    {
        #if UNITY_EDITOR
        Debug.Log("=== [DevTools] ESTADO ACTUAL DEL JUEGO ===");
        
        // GameManager
        if (GameManager.Instance != null)
        {
            Debug.Log($"✅ GameManager: Activo");
            Debug.Log($"   - Checkpoint guardado: {(PlayerPrefs.HasKey("CheckpointGuardado") ? "SÍ" : "NO")}");
        }
        else
        {
            Debug.Log($"❌ GameManager: NO ENCONTRADO");
        }
        
        // Inventario
        if (InventoryUIManager.Instance != null)
        {
            Debug.Log($"✅ InventoryUIManager: Activo");
            Debug.Log($"   - Items en inventario: [Revisar en el Inspector del InventoryUIManager]");
        }
        else
        {
            Debug.Log($"❌ InventoryUIManager: NO ENCONTRADO");
        }
        
        // Misiones
        if (ObjetivoManager.Instance != null)
        {
            Debug.Log($"✅ ObjetivoManager: Activo");
            int misionActual = ObjetivoManager.Instance.ObtenerIndiceMisionActual();
            Debug.Log($"   - Misión actual: #{misionActual}");
        }
        else
        {
            Debug.Log($"❌ ObjetivoManager: NO ENCONTRADO");
        }
        
        // Items en el mundo
        ItemRecolectable[] items = FindObjectsOfDay<ItemRecolectable>(true);
        int itemsActivos = 0;
        int itemsInactivos = 0;
        
        foreach (ItemRecolectable item in items)
        {
            if (item.gameObject.activeSelf)
                itemsActivos++;
            else
                itemsInactivos++;
        }
        
        Debug.Log($"🌍 Items en el mundo:");
        Debug.Log($"   - Activos (disponibles): {itemsActivos}");
        Debug.Log($"   - Inactivos (recolectados): {itemsInactivos}");
        Debug.Log($"   - TOTAL: {items.Length}");
        
        Debug.Log("=== FIN DEL REPORTE ===");
        #endif
    }

    /// <summary>
    /// ⚡ RECARGAR ESCENA (útil para probar rápidamente)
    /// </summary>
    [ContextMenu("⚡ Recargar Escena Actual")]
    public void RecargarEscena()
    {
        #if UNITY_EDITOR
        if (mostrarLogsDetallados)
        {
            Debug.Log("[DevTools] ⚡ Recargando escena...");
        }
        
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
        #endif
    }

    /// <summary>
    /// 🗑️ BORRAR TODOS LOS PLAYERPREFS (PELIGROSO - úsalo con cuidado)
    /// </summary>
    [ContextMenu("🗑️ BORRAR TODOS LOS PLAYERPREFS (¡PELIGROSO!)")]
    public void BorrarTodosLosPlayerPrefs()
    {
        #if UNITY_EDITOR
        bool confirmar = EditorUtility.DisplayDialog(
            "⚠️ ADVERTENCIA",
            "Esto borrará TODOS los PlayerPrefs del proyecto.\n\n¿Estás seguro?",
            "Sí, borrar todo",
            "Cancelar"
        );
        
        if (confirmar)
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            
            Debug.LogWarning("[DevTools] 🗑️ TODOS los PlayerPrefs han sido BORRADOS");
            
            EditorUtility.DisplayDialog(
                "PlayerPrefs Borrados",
                "✅ Todos los PlayerPrefs han sido eliminados.\n\nReinicia el juego para que los cambios surtan efecto.",
                "OK"
            );
        }
        else
        {
            Debug.Log("[DevTools] Cancelado - No se borraron los PlayerPrefs");
        }
        #endif
    }

    // ========================================
    // MÉTODOS AUXILIARES
    // ========================================

    /// <summary>
    /// Encuentra objetos incluyendo los inactivos
    /// </summary>
    private T[] FindObjectsOfDay<T>(bool includeInactive = true) where T : Object
    {
        #if UNITY_EDITOR
        return Resources.FindObjectsOfTypeAll<T>();
        #else
        return FindObjectsByType<T>(FindObjectsSortMode.None);
        #endif
    }

    // ========================================
    // VALIDACIÓN
    // ========================================

    private void OnValidate()
    {
        // Solo permitir que el script funcione en el Editor
        #if !UNITY_EDITOR
        enabled = false;
        Debug.LogWarning("[DevTools] Este script solo funciona en el Editor de Unity");
        #endif
    }
}

// ========================================
// EDITOR PERSONALIZADO (BOTONES EN INSPECTOR)
// ========================================

#if UNITY_EDITOR
[CustomEditor(typeof(DevTools))]
public class DevToolsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        DevTools devTools = (DevTools)target;
        
        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("ACCIONES DE DESARROLLO", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "⚠️ Estos botones SOLO funcionan en el Editor de Unity.\nNo estarán disponibles en la build final.",
            MessageType.Info
        );
        
        EditorGUILayout.Space(10);
        
        // Botón principal - Resetear todo
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("🔄 RESETEAR TODO", GUILayout.Height(40)))
        {
            devTools.ResetearTodo();
        }
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Reseteos Individuales", EditorStyles.boldLabel);
        
        // Botones individuales
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("💾 Checkpoint", GUILayout.Height(30)))
        {
            devTools.ResetearCheckpoint();
        }
        if (GUILayout.Button("🎒 Inventario", GUILayout.Height(30)))
        {
            devTools.ResetearInventario();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("📋 Misiones", GUILayout.Height(30)))
        {
            devTools.ResetearMisiones();
        }
        if (GUILayout.Button("🌍 Items Mundo", GUILayout.Height(30)))
        {
            devTools.ResetearItemsDelMundo();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Utilidades", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("🔍 Ver Estado", GUILayout.Height(30)))
        {
            devTools.MostrarEstadoActual();
        }
        if (GUILayout.Button("⚡ Recargar Escena", GUILayout.Height(30)))
        {
            devTools.RecargarEscena();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space(10);
        
        // Botón peligroso
        GUI.backgroundColor = new Color(1f, 0.3f, 0.3f);
        if (GUILayout.Button("🗑️ BORRAR PLAYERPREFS (¡PELIGROSO!)", GUILayout.Height(30)))
        {
            devTools.BorrarTodosLosPlayerPrefs();
        }
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.Space(10);
        EditorGUILayout.HelpBox(
            "💡 TIP: También puedes usar el menú:\nTools > DevTools > [acción]",
            MessageType.Info
        );
    }
}

// ========================================
// MENÚ EN UNITY (Tools > DevTools)
// ========================================
public static class DevToolsMenu
{
    [MenuItem("Tools/DevTools/🔄 Resetear Todo #r", false, 1)]
    public static void ResetearTodo()
    {
        DevTools devTools = FindDevTools();
        if (devTools != null)
        {
            devTools.ResetearTodo();
        }
    }
    
    [MenuItem("Tools/DevTools/💾 Resetear Checkpoint", false, 11)]
    public static void ResetearCheckpoint()
    {
        DevTools devTools = FindDevTools();
        if (devTools != null)
        {
            devTools.ResetearCheckpoint();
        }
    }
    
    [MenuItem("Tools/DevTools/🎒 Resetear Inventario", false, 12)]
    public static void ResetearInventario()
    {
        DevTools devTools = FindDevTools();
        if (devTools != null)
        {
            devTools.ResetearInventario();
        }
    }
    
    [MenuItem("Tools/DevTools/📋 Resetear Misiones", false, 13)]
    public static void ResetearMisiones()
    {
        DevTools devTools = FindDevTools();
        if (devTools != null)
        {
            devTools.ResetearMisiones();
        }
    }
    
    [MenuItem("Tools/DevTools/🌍 Resetear Items del Mundo", false, 14)]
    public static void ResetearItemsMundo()
    {
        DevTools devTools = FindDevTools();
        if (devTools != null)
        {
            devTools.ResetearItemsDelMundo();
        }
    }
    
    [MenuItem("Tools/DevTools/🔍 Mostrar Estado Actual", false, 21)]
    public static void MostrarEstado()
    {
        DevTools devTools = FindDevTools();
        if (devTools != null)
        {
            devTools.MostrarEstadoActual();
        }
    }
    
    [MenuItem("Tools/DevTools/⚡ Recargar Escena", false, 22)]
    public static void RecargarEscena()
    {
        DevTools devTools = FindDevTools();
        if (devTools != null)
        {
            devTools.RecargarEscena();
        }
    }
    
    private static DevTools FindDevTools()
    {
        DevTools devTools = Object.FindFirstObjectByType<DevTools>();
        
        if (devTools == null)
        {
            Debug.LogWarning("[DevToolsMenu] No se encontró DevTools en la escena. Créalo primero.");
            
            bool crear = EditorUtility.DisplayDialog(
                "DevTools no encontrado",
                "No existe un GameObject con DevTools en la escena.\n\n¿Quieres crear uno automáticamente?",
                "Sí, crear",
                "No"
            );
            
            if (crear)
            {
                GameObject go = new GameObject("DevTools");
                devTools = go.AddComponent<DevTools>();
                Debug.Log("[DevToolsMenu] ✅ DevTools creado automáticamente");
            }
        }
        
        return devTools;
    }
}
#endif
