using UnityEngine;

/// <summary>
/// Este script controla las animaciones del personaje.
/// Se comunica con el PlayerController para obtener el estado del jugador (velocidad, salto, etc.)
/// y actualiza los parámetros del Animator Controller correspondiente.
/// </summary>
[RequireComponent(typeof(Animator))]
public class PlayerAnimatorController : MonoBehaviour
{
    private Animator animator;
    private PlayerController playerController;

    void Awake()
    {
        // Obtener referencias a los componentes necesarios
        animator = GetComponent<Animator>();
        playerController = GetComponentInParent<PlayerController>();

        if (playerController == null)
        {
            Debug.LogError("No se encontró el PlayerController en el personaje. Asegúrate de que el script PlayerController esté en el mismo objeto o en un objeto padre.");
            this.enabled = false; // Desactivar este script si no encuentra el controlador
        }
    }

    void Update()
    {
        if (playerController == null) return;

        // --- Actualizar Parámetros del Animator ---

        // 1. Velocidad (Speed)
        // Obtiene la velocidad actual del PlayerController y la pasa al Animator.
        // Esto controla la transición entre Idle y Walk/Run.
        animator.SetFloat("Speed", playerController.CurrentSpeed);

        // 2. Salto (IsJumping)
        // Le dice al Animator si el personaje acaba de iniciar un salto.
        // Esto dispara la animación de salto.
        animator.SetBool("IsJumping", playerController.IsJumping);

        // 3. En el suelo (IsGrounded)
        // Informa al Animator si el personaje está tocando el suelo.
        // Es útil para controlar la transición desde la animación de salto/caída a Idle/Walk.
        animator.SetBool("IsGrounded", playerController.IsGrounded);
    }
}
