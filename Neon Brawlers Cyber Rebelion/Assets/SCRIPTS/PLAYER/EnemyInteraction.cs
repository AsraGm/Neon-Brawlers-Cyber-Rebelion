using UnityEngine;

public class EnemyInteraction : MonoBehaviour
{
    [Header("Referencias")]
    public Transform enemyLookAt;     // Empty hijo del enemigo

    [Header("Cámara")]
    [SerializeField] private float enemyFOV = 30f;

    ThirdPersonCam cam;
    ObstacleInteraction obstacleInteraction;

    bool enemyInside;

    void Awake()
    {
        cam = FindFirstObjectByType<ThirdPersonCam>();
        obstacleInteraction = GetComponent<ObstacleInteraction>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsEnemy(other)) return;
        if (!obstacleInteraction.PlayerInObstacle) return;

        enemyInside = true;
        EnterEnemyMode();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsEnemy(other)) return;

        enemyInside = false;
        ExitEnemyMode();
    }

    bool IsEnemy(Collider other)
    {
        return other.gameObject.layer == LayerMask.NameToLayer("Enemy");
    }

    void EnterEnemyMode()
    {
        if (cam == null || enemyLookAt == null) return;

        cam.SetCustomFollow(enemyLookAt, enemyFOV);
    }

    public void ExitEnemyMode()
    {
        if (cam == null) return;
        if (!obstacleInteraction.PlayerInObstacle) return;

        cam.ReturnToObstacleFollow();
    }

    // llamado desde ObstacleInteraction si el jugador se va
    public void ForceCancel()
    {
        if (!enemyInside) return;

        enemyInside = false;
        ExitEnemyMode();
    }
}

