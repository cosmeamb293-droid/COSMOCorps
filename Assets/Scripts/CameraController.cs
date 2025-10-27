
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Configuración de Cámara")]
    public float sensibilidadMouse = 100f;
    public Transform cuerpoDelJugador;

    private float rotacionX = 0f;

    void Start()
    {
        // Bloquea el cursor en el centro de la pantalla y lo hace invisible
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // --- LECTURA DEL INPUT DEL MOUSE ---
        float mouseX = Input.GetAxis("Mouse X") * sensibilidadMouse * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadMouse * Time.deltaTime;

        // --- ROTACIÓN VERTICAL (ARRIBA/ABAJO) ---
        // Restamos mouseY para que el movimiento sea natural (subir el ratón = mirar arriba)
        rotacionX -= mouseY;
        
        // Limitamos la rotación vertical para no dar una voltereta
        rotacionX = Mathf.Clamp(rotacionX, -90f, 90f);

        // Aplicamos la rotación vertical solo a la cámara
        transform.localRotation = Quaternion.Euler(rotacionX, 0f, 0f);

        // --- ROTACIÓN HORIZONTAL (IZQUIERDA/DERECHA) ---
        // Aplicamos la rotación horizontal al cuerpo completo del jugador
        cuerpoDelJugador.Rotate(Vector3.up * mouseX);
    }
}
