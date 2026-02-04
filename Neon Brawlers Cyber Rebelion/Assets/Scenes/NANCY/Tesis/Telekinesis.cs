using UnityEngine;

public class Telekinesis : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private LayerMask objectLayer;
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private Transform holdPosition;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float throwForce = 20f;
    [SerializeField] private float cooldownTime = 2f;
    [SerializeField] private KeyCode manipulateKey = KeyCode.E;

    private Rigidbody currentRb;
    private float cooldownTimer;

    void Update()
    {
        cooldownTimer -= Time.deltaTime; // Reduce el temporizador de cooldown

        if (Input.GetKeyDown(manipulateKey) && cooldownTimer <= 0 && !currentRb) GrabNearestObject(); // Agarra el objeto más cercano si no hay ninguno agarrado y el cooldown ha terminado

        if (Input.GetKey(manipulateKey) && currentRb) MoveObject(); // Manteniendo la tecla, mueve el objeto hacia la posición de agarre

        if (Input.GetKeyUp(manipulateKey) && currentRb) ThrowObject(); // Al soltar la tecla, lanza el objeto
    }

    void GrabNearestObject()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, objectLayer);
        if (hits.Length == 0) return;

        Collider nearest = hits[0];
        float minDist = Vector3.Distance(Camera.main.transform.position, nearest.transform.position); // Calcular la distancia al objeto más cercano

        for (int i = 1; i < hits.Length; i++)
        {
            float dist = Vector3.Distance(Camera.main.transform.position, hits[i].transform.position); // Calcular la distancia al objeto
            if (dist < minDist)
            {
                minDist = dist; // Actualizar la distancia mínima
                nearest = hits[i]; // Actualizar el objeto más cercano
            }
        }

        currentRb = nearest.GetComponent<Rigidbody>(); // Obtener el Rigidbody del objeto más cercano

        if (currentRb) // Si el objeto tiene un Rigidbody, desactiva la gravedad y detener su movimiento
        {
            currentRb.useGravity = false;
            currentRb.linearVelocity = Vector3.zero;
        }
    }

    void MoveObject() // Mueve el objeto hacia la posición de agarre
    {
        currentRb.MovePosition(Vector3.Lerp(currentRb.position, holdPosition.position, Time.deltaTime * moveSpeed));
    }

    void ThrowObject() // Lanza el objeto en la dirección de la cámara
    {
        currentRb.useGravity = true;
        currentRb.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse);
        currentRb = null;
        cooldownTimer = cooldownTime;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}