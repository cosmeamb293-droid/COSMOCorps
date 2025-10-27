
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Configuración Principal")]
    public Transform objetivo; // El objeto que la cámara seguirá (el jugador)
    public float distancia = 5.0f;
    public float sensibilidadMouse = 200f;
    public Vector2 limitesDeInclinacion = new Vector2(10f, 85f); // Límites para mirar arriba y abajo

    [Header("Suavizado y Colisiones")]
    public float suavizadoDePosicion = 0.125f;
    public float suavizadoDeRotacion = 0.1f;
    public LayerMask capasDeObstaculos; // Capas con las que la cámara colisionará

    private float yaw = 0f;   // Rotación horizontal
    private float pitch = 30f;  // Rotación vertical (inclinación)

    void Start()
    {
        // Bloquea y oculta el cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // LateUpdate se ejecuta después de todos los Updates, ideal para cámaras
    void LateUpdate()
    {
        if (objetivo == null) return;

        // --- MANEJO DE INPUT Y ROTACIÓN ---
        yaw += Input.GetAxis("Mouse X") * sensibilidadMouse * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * sensibilidadMouse * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, limitesDeInclinacion.x, limitesDeInclinacion.y);

        // Calcula la rotación deseada de la cámara
        Quaternion rotacionDeseada = Quaternion.Euler(pitch, yaw, 0);

        // --- MANEJO DE POSICIÓN Y COLISIONES ---
        // Calcula la dirección y la posición deseada de la cámara sin obstáculos
        Vector3 direccionDeseada = new Vector3(0, 0, -distancia);
        Vector3 posicionDeseada = objetivo.position + rotacionDeseada * direccionDeseada;

        // Comprueba si hay obstáculos entre el jugador y la cámara
        RaycastHit hit;
        float distanciaCorregida = distancia;
        if (Physics.Linecast(objetivo.position, posicionDeseada, out hit, capasDeObstaculos))
        {
            // Si hay un obstáculo, acorta la distancia de la cámara
            distanciaCorregida = Vector3.Distance(objetivo.position, hit.point) - 0.2f; // Pequeño offset para no estar pegado
        }

        // Calcula la posición final de la cámara con la distancia corregida
        Vector3 direccionCorregida = new Vector3(0, 0, -distanciaCorregida);
        Vector3 posicionFinal = objetivo.position + rotacionDeseada * direccionCorregida;

        // --- APLICACIÓN DE MOVIMIENTO SUAVIZADO ---
        transform.position = Vector3.Lerp(transform.position, posicionFinal, suavizadoDePosicion);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, suavizadoDeRotacion);
    }
}
