using UnityEngine;

public class ObstacleInteraction : MonoBehaviour
{
    [Header("Referencias")]
    public Transform obstacleLookAt;   // el empty
    public Transform snapPoint;         // punto donde se pega el player

    [Header("Ajustes")]
    public float snapSpeed = 10f;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger enter: " + other.name);

        if (!other.CompareTag("Player")) return;

        PlayerMovement player = other.GetComponentInParent<PlayerMovement>();
        ThirdPersonCam cam = FindFirstObjectByType<ThirdPersonCam>();

        if (player == null || cam == null)
        {
            Debug.Log("Player o Camara no encontrados");
            return;
        }

        EnterObstacle(player, cam);
    }

    void EnterObstacle(PlayerMovement player, ThirdPersonCam cam)
    {
        player.EnterObstacleMode(snapPoint, snapSpeed);
        cam.EnterObstacleMode(obstacleLookAt);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerMovement player = other.GetComponentInParent<PlayerMovement>();
        ThirdPersonCam cam = FindFirstObjectByType<ThirdPersonCam>();

        if (player == null || cam == null) return;

        ExitObstacle(player, cam);
    }

    void ExitObstacle(PlayerMovement player, ThirdPersonCam cam)
    {
        player.ExitObstacleMode();
        cam.ExitObstacleMode();
    }



}
