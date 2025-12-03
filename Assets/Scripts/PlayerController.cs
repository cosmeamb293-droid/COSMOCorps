
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidadDeMovimiento = 5.0f;
    public float velocidadDeRotacion = 10f; // Velocidad con la que el personaje se gira
    public float fuerzaDeSalto = 8.0f;
    public float duracionDeLevantarse = 0.5f; // Tiempo que tarda el personaje en levantarse

    [Header("Referencias")]
    public Transform camara;

    [Header("Chequeo de Suelo")]
    public Transform chequeadorDeSuelo;
    public float radioDelChequeador = 0.4f;
    public LayerMask capaDelSuelo;

    public float velocidadDeLevantarse = 5f;

    [Header("Configuración de Corrida en Pared")]
    public LayerMask capaDeLaPared;
    public float tiempoMaximoCorriendoEnLaPared = 3f;
    public Color colorCorriendoEnLaPared = Color.blue;

    // Componentes y estado
        private Rigidbody rb;
        private Vector3 direccionDeInput;
        private bool estaEnElSuelo;
        private int contadorDeSaltos = 0;

        // Propiedades públicas para el Animator
        public float CurrentSpeed { get; private set; }
        public bool IsJumping { get; private set; }
        public bool IsGrounded { get; private set; }
    
        private float velocidadDeCarrera;
        private bool estaCorriendo = false;
        private float velocidadActual;
        private bool estaLevantandose = false;

        private bool estaCorriendoEnLaPared = false;
        private bool puedeCorrerEnLaPared = true;
        private float tiempoCorriendoEnLaPared = 0f;
        private Vector3 normalDeLaPared;
        private Vector3 puntoDeLaPared;
    
        // Variables para el indicador visual
        private Renderer renderizadorDelPersonaje;
        private Color colorOriginal;
    
        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            velocidadActual = velocidadDeMovimiento;
            velocidadDeCarrera = velocidadDeMovimiento * 1.6f;
    
            // Obtenemos el renderer y guardamos el color original
            renderizadorDelPersonaje = GetComponentInChildren<Renderer>();
            if (renderizadorDelPersonaje != null)
            {
                colorOriginal = renderizadorDelPersonaje.material.color;
            }
        }
    
        void Update()
        {
            // --- MANEJO DE INPUTS ---
            estaEnElSuelo = Physics.CheckSphere(chequeadorDeSuelo.position, radioDelChequeador, capaDelSuelo);
    
            // Actualizar propiedades públicas para el Animator
            IsGrounded = estaEnElSuelo;
            CurrentSpeed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;
            IsJumping = !estaEnElSuelo; // Si no está en el suelo, está saltando/cayendo

            if (estaEnElSuelo && rb.linearVelocity.y <= 0)
            {
                contadorDeSaltos = 0;
                puedeCorrerEnLaPared = true;
                if (estaCorriendoEnLaPared)
                {
                    DetenerCorridaEnLaPared();
                }
            }

            if (estaCorriendoEnLaPared)
            {
                tiempoCorriendoEnLaPared += Time.deltaTime;
                if (tiempoCorriendoEnLaPared > tiempoMaximoCorriendoEnLaPared)
                {
                    DetenerCorridaEnLaPared();
                }
            }
    
            // Input de movimiento (WASD)
            float movHorizontal = Input.GetAxis("Horizontal");
            float movVertical = Input.GetAxis("Vertical");
            direccionDeInput = new Vector3(movHorizontal, 0f, movVertical).normalized;
    
            // Input de correr (Shift)
            if (Input.GetKeyDown(KeyCode.LeftShift) && !estaCorriendo && !estaLevantandose)
            {
                estaCorriendo = true;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift) && estaCorriendo)
            {
                estaCorriendo = false;
            }
    
            // Input de salto (Barra espaciadora)
            if (Input.GetButtonDown("Jump") && !estaLevantandose)
            {
                if (estaCorriendoEnLaPared)
                {
                    SaltarDeLaPared();
                }
                else if (estaEnElSuelo)
                {
                    Saltar(fuerzaDeSalto);
                }
                else if (contadorDeSaltos < 1)
                {
                    Saltar(fuerzaDeSalto);
                }
            }
    
            // --- GESTIÓN DE VELOCIDAD ---
            if (estaLevantandose)
            {
                velocidadActual = velocidadDeMovimiento * 0.3f;
            }
            else if (estaCorriendo)
            {
                velocidadActual = velocidadDeCarrera;
                if (!Input.GetKey(KeyCode.LeftShift)) estaCorriendo = false; // Seguridad por si se suelta la tecla durante otra acción
            }
            else
            {
                velocidadActual = velocidadDeMovimiento;
            }
        }
    
        void FixedUpdate()
        {
            Vector3 camForward = Vector3.Scale(camara.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 direccionDeMovimiento = direccionDeInput.z * camForward + direccionDeInput.x * camara.right;

            if (!estaEnElSuelo && puedeCorrerEnLaPared && direccionDeInput.magnitude > 0.1f && ChequearPared(direccionDeMovimiento))
            {
                IniciarCorridaEnLaPared();
            }

            if (estaCorriendoEnLaPared)
            {
                if (!ChequearPared(direccionDeMovimiento) || rb.linearVelocity.magnitude < 0.1f)
                {
                    DetenerCorridaEnLaPared();
                    return;
                }

                Vector3 direccionDeCorrida = Vector3.Cross(normalDeLaPared, Vector3.up).normalized;
                float alineacion = Vector3.Dot(direccionDeCorrida, transform.forward);
                if (alineacion < 0)
                {
                    direccionDeCorrida = -direccionDeCorrida;
                }

                // Allow player to influence movement
                rb.linearVelocity = (direccionDeCorrida + direccionDeMovimiento * 0.5f).normalized * velocidadActual;
                rb.AddForce(normalDeLaPared * 20f);
            }
            else
            {
                rb.useGravity = true;
                // Si hay input de movimiento, calcula la dirección relativa a la cámara
                if (direccionDeInput.magnitude >= 0.1f)
                {
                    // --- CÁLCULO DE DIRECCIÓN RELATIVA A LA CÁMARA ---
        
                    // --- ROTACIÓN DEL PERSONAJE ---
                    if (!estaLevantandose)
                    {
                        Quaternion nuevaRotacion = Quaternion.LookRotation(direccionDeMovimiento);
                        rb.rotation = Quaternion.Slerp(rb.rotation, nuevaRotacion, Time.fixedDeltaTime * velocidadDeRotacion);
                    }
        
                    // --- MOVIMIENTO DEL PERSONAJE ---
                    Vector3 nuevaVelocidad = direccionDeMovimiento * velocidadActual;
                    rb.linearVelocity = new Vector3(nuevaVelocidad.x, rb.linearVelocity.y, nuevaVelocidad.z);
                }
                else
                {
                    // Si no hay input, el personaje deja de deslizarse y se endereza
                    rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
                }
            }

            if (estaEnElSuelo)
            {
                EnderezarPersonaje();
            }
        }
    
            void EnderezarPersonaje()
            {
                // Si está muy caído (más de 45 grados), inicia la rutina de levantarse.
                // Se ignora el chequeo de suelo aquí porque puede fallar si el personaje está de lado.
                if (Vector3.Angle(Vector3.up, transform.up) > 45f && !estaLevantandose)
                {
                    // Solo se activa si no está volando o cayendo a gran velocidad.
                    if (Mathf.Abs(rb.linearVelocity.y) < 0.5f)
                    {
                        StartCoroutine(RutinaDeLevantarse());
                    }
                }
                // Si está en el suelo y solo un poco inclinado, lo endereza suavemente.
                else if (estaEnElSuelo && !estaLevantandose)
                {
                    // Umbral pequeño para asegurar que se enderece por completo.
                    if (Vector3.Angle(Vector3.up, transform.up) > 1f)
                    {
                        // Usamos LookRotation para asegurar que quede perfectamente vertical.
                        Vector3 forward = transform.forward;
                        forward.y = 0;
                        forward.Normalize();
                        if (forward == Vector3.zero) { forward = Vector3.forward; }
        
                        Quaternion rotacionObjetivo = Quaternion.LookRotation(forward, Vector3.up);
                        rb.rotation = Quaternion.Slerp(rb.rotation, rotacionObjetivo, Time.fixedDeltaTime * velocidadDeRotacion);
                    }
                }
            }    
        System.Collections.IEnumerator RutinaDeLevantarse()
        {
                    estaLevantandose = true;
                    if (renderizadorDelPersonaje != null) renderizadorDelPersonaje.material.color = Color.red;
            
                    float tiempoPasado = 0f;
                    float duracion = duracionDeLevantarse;
            
                    Quaternion rotacionInicial = rb.rotation;    
            Vector3 forward = transform.forward;
            forward.y = 0;
            forward.Normalize();
            if (forward == Vector3.zero) { forward = Vector3.forward; }
            Quaternion rotacionObjetivo = Quaternion.LookRotation(forward, Vector3.up);
    
            while (tiempoPasado < duracion)
            {
                tiempoPasado += Time.fixedDeltaTime;
                float t = tiempoPasado / duracion;
                rb.rotation = Quaternion.Slerp(rotacionInicial, rotacionObjetivo, t);
                yield return new WaitForFixedUpdate();
            }
    
            rb.rotation = rotacionObjetivo; // Asegurar la rotación final
            if (renderizadorDelPersonaje != null) renderizadorDelPersonaje.material.color = colorOriginal;
            estaLevantandose = false;
        }    System.Collections.IEnumerator ImpulsoInicial()
    {
        float impulso = estaCorriendo ? 1.2f : 1.0f; // Impulso extra si está corriendo
        rb.AddForce(transform.forward * impulso, ForceMode.Impulse);
        yield return new WaitForSeconds(0.5f); // Duración del impulso
    }

    public void Saltar(float fuerza)
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * fuerza, ForceMode.Impulse);
        contadorDeSaltos++;
    }

    public void ResetearContadorDeSaltos()
    {
        contadorDeSaltos = 0;
    }

    public void EstablecerContadorDeSaltos(int valor)
    {
        contadorDeSaltos = valor;
    }

    void DetenerCorridaEnLaPared()
    {
        estaCorriendoEnLaPared = false;
        rb.useGravity = true;
        if (renderizadorDelPersonaje != null)
        {
            renderizadorDelPersonaje.material.color = colorOriginal;
        }
        puedeCorrerEnLaPared = false;
    }

    void SaltarDeLaPared()
    {
        DetenerCorridaEnLaPared();
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(normalDeLaPared * fuerzaDeSalto * 1.5f, ForceMode.Impulse);
        rb.AddForce(Vector3.up * fuerzaDeSalto, ForceMode.Impulse);
        puedeCorrerEnLaPared = false;
    }

    bool ChequearPared(Vector3 direccionDeMovimiento)
    {
        RaycastHit hit;
        float radioEsfera = 0.5f;
        float distanciaDeteccion = 1f;

        // Lanzar una esfera en la dirección del movimiento para detectar paredes
        if (Physics.SphereCast(transform.position, radioEsfera, direccionDeMovimiento, out hit, distanciaDeteccion, capaDeLaPared))
        {
            // Asegurarse de que no estamos detectando el suelo como una pared
            if (Vector3.Angle(hit.normal, Vector3.up) > 80f)
            {
                // Additional check to make sure it's not a ground object
                if ((capaDelSuelo.value & (1 << hit.collider.gameObject.layer)) == 0)
                {
                    normalDeLaPared = -hit.normal;
                    puntoDeLaPared = hit.point;
                    return true;
                }
            }
        }
        return false;
    }

    void IniciarCorridaEnLaPared()
    {
        estaCorriendoEnLaPared = true;
        rb.useGravity = false;
        tiempoCorriendoEnLaPared = 0f;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (renderizadorDelPersonaje != null)
        {
            renderizadorDelPersonaje.material.color = colorCorriendoEnLaPared;
        }
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
