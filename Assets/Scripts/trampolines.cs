
using UnityEngine;

public class trampolines : MonoBehaviour
{
    [Header("Configuración del Trampolín")]
    public float multiplicadorDeSalto = 5f;

    private void OnCollisionEnter(Collision collision)
    {
        // Asegurarse de que el objeto que colisiona es el jugador
        if (collision.gameObject.CompareTag("Player"))
        {
            // Intentar obtener el componente PlayerController
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();

            if (playerController != null)
            {
                // Calcular la nueva fuerza de salto
                float nuevaFuerzaDeSalto = playerController.fuerzaDeSalto * multiplicadorDeSalto;

                // Llamar al método Saltar del jugador con la nueva fuerza
                playerController.Saltar(nuevaFuerzaDeSalto);

                // Establecer el contador de saltos a 1 para permitir un solo salto en el aire
                playerController.EstablecerContadorDeSaltos(1);
            }
        }
    }
}
