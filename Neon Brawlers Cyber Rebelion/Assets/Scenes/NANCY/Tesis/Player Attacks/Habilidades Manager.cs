using UnityEngine;
using UnityEngine.InputSystem;

public class HabilidadesManager : MonoBehaviour
{
    public static HabilidadesManager instance { get; private set; }

    private int selectedAbility = 0;
    [SerializeField] private bool[] unlockedAbilities = { false, false, false };

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            selectedAbility = 0;
            Debug.Log("Habilidad 1 seleccionada");
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            selectedAbility = 1;
            Debug.Log("Habilidad 2 seleccionada");
        }
        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            selectedAbility = 2;
            Debug.Log("Habilidad 3 seleccionada");
        }

        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            if (unlockedAbilities[selectedAbility])
            {
                ExecuteAbility();
            }
            else
            {
                Debug.Log("Esta habilidad aun no esta desbloqueada");
            }
        }
    }

    void ExecuteAbility()
    {
        switch (selectedAbility)
        {
            case 0:
                Debug.Log("Se uso habilidad 1");
                break;
            case 1:
                GetComponent<ElectromagneticWave>()?.ActivarOnda();
                Debug.Log("Se uso habilidad 2");
                break;
            case 2:
                Debug.Log("Se uso habilidad 3");
                break;
        }
    }
    //para desbloquear habilidades
    public void UnlockAbility(int abilityIndex)
    {
        if (abilityIndex >= 0 && abilityIndex < 3)
        {
            unlockedAbilities[abilityIndex] = true;
            Debug.Log($"Se desbloqueo la habilidad: {abilityIndex + 1}");
        }
    }

    //verificar si la habilidad esta desbloqueada
    public bool IsAbilityUnlocked(int abilityIndex)
    {
        return unlockedAbilities[abilityIndex];
    }
}
