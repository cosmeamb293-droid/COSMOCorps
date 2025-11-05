
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidadDeMovimiento = 5.0f;
    public float velocidadDeRotacion = 10f; // Velocidad con la que el personaje se gira
    public float fuerzaDeSalto = 8.0f;

    [Header("Referencias")]
    public Transform camara;

    [Header("Chequeo de Suelo")]
    public Transform chequeadorDeSuelo;
    public float radioDelChequeador = 0.4f;
    public LayerMask capaDelSuelo;

    // Componentes y estado
    private Rigidbody rb;
    private Vector3 direccionDeInput;
    private bool estaEnElSuelo;
    private int contadorDeSaltos = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) Debug.LogError("El personaje no tiene un componente Rigidbody.");
        if (camara == null) Debug.LogError("La referencia a la cámara no está asignada en el PlayerController.");
    }

    private float velocidadDeCarrera;
    private bool estaCorriendo = false;
    private float velocidadActual;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        velocidadActual = velocidadDeMovimiento;
        velocidadDeCarrera = velocidadDeMovimiento * 1.6f;
    }

    void Update()
    {
        // --- MANEJO DE INPUTS ---
        estaEnElSuelo = Physics.CheckSphere(chequeadorDeSuelo.position, radioDelChequeador, capaDelSuelo);

        if (estaEnElSuelo && rb.linearVelocity.y <= 0)
        {
            contadorDeSaltos = 0;
        }

        // Input de movimiento (WASD)
        float movHorizontal = Input.GetAxis("Horizontal");
        float movVertical = Input.GetAxis("Vertical");
        direccionDeInput = new Vector3(movHorizontal, 0f, movVertical).normalized;

        // Input de correr (Shift)
        if (Input.GetKeyDown(KeyCode.LeftShift) && !estaCorriendo)
        {
            estaCorriendo = true;
            velocidadActual = velocidadDeCarrera;
            StartCoroutine(ImpulsoInicial());
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) && estaCorriendo)
        {
            estaCorriendo = false;
            velocidadActual = velocidadDeMovimiento;
        }

        // Input de salto (Barra espaciadora)
        if (Input.GetButtonDown("Jump"))
        {
            if (estaEnElSuelo)
            {
                Saltar(fuerzaDeSalto);
            }
            else if (contadorDeSaltos < 2)
            {
                Saltar(fuerzaDeSalto / 2);
            }
        }
    }

    void FixedUpdate()
    {
        // Si hay input de movimiento, calcula la dirección relativa a la cámara
        if (direccionDeInput.magnitude >= 0.1f)
        {
            // --- CÁLCULO DE DIRECCIÓN RELATIVA A LA CÁMARA ---
            Vector3 camForward = Vector3.Scale(camara.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 direccionDeMovimiento = direccionDeInput.z * camForward + direccionDeInput.x * camara.right;

            // --- ROTACIÓN DEL PERSONAJE ---
            Quaternion nuevaRotacion = Quaternion.LookRotation(direccionDeMovimiento);
            rb.rotation = Quaternion.Slerp(rb.rotation, nuevaRotacion, Time.fixedDeltaTime * velocidadDeRotacion);

            // --- MOVIMIENTO DEL PERSONAJE ---
            Vector3 nuevaVelocidad = direccionDeMovimiento * velocidadActual;
            rb.linearVelocity = new Vector3(nuevaVelocidad.x, rb.linearVelocity.y, nuevaVelocidad.z);
        }
    }

    System.Collections.IEnumerator ImpulsoInicial()
    {
        float impulso = estaCorriendo ? 1.2f : 1.0f; // Impulso extra si está corriendo
        rb.AddForce(transform.forward * impulso, ForceMode.Impulse);
        yield return new WaitForSeconds(0.5f); // Duración del impulso
    }

    void Saltar(float fuerza)
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * fuerza, ForceMode.Impulse);
        contadorDeSaltos++;
    }

    void OnDrawGizmosSelected()
    {
        if (chequeadorDeSuelo != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(chequeadorDeSuelo.position, radioDelChequeador);
        }
    }
}
