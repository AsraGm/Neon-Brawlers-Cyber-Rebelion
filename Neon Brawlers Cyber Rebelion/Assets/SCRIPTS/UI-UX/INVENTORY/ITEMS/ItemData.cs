using UnityEngine;

[CreateAssetMenu(fileName = "Nuevo_Item", menuName = "Inventario/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Información General")]
    [Tooltip("ID único del item (ej: tarjeta_roja, bateria_01)")]
    public string itemID;

    [Tooltip("Nombre para mostrar en Base de Datos")]
    public string nombreDisplay;

    [Header("Visual")]
    [Tooltip("Sprite/PNG que se muestra en el slot")]
    public Sprite iconoItem;

    [Header("Tipo de Item")]
    [Tooltip("Selecciona en qué tab(s) aparece este item")]
    public TipoItem tipo = TipoItem.ItemNormal;

    [Header("Información Lore (solo para Base de Datos)")]
    [TextArea(3, 10)]
    [Tooltip("Texto que se muestra al seleccionar en Base de Datos")]
    public string descripcionLore;

    [Tooltip("Audio log que se reproduce al seleccionar")]
    public AudioClip audioLore;

    [Header("Modelo 3D (para inspección)")]
    [Tooltip("Prefab del modelo 3D para inspección 360°")]
    public GameObject modelo3D;

    public bool EsDeTipo(TipoItem tipoAVerificar)
    {
        return (tipo & tipoAVerificar) != 0;
    }
}

[System.Flags]
public enum TipoItem
{
    ItemNormal = 1 << 0,  // 1  - Aparece en tab LLAVES
    ItemLore = 1 << 1   // 2  - Aparece en tab BdD
}