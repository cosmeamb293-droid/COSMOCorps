using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cargar escenas

public class VictoriaTrigger : MonoBehaviour
{
    // Opcional: El nombre de la escena que se cargará al ganar.
    // Puedes asignarlo desde el Inspector de Unity.
    public string nombreDeLaEscenaDeVictoria;

    // Esta función se ejecuta automáticamente cuando otro objeto con un Collider
    // entra en el trigger de este objeto.
    private void OnTriggerEnter(Collider other)
    {
        // Verificamos si el objeto que entró tiene el tag "Player".
        // Es importante para asegurarnos de que solo el jugador active la victoria.
        if (other.CompareTag("Player"))
        {
            // Si es el jugador, ¡ha ganado!
            Debug.Log("¡Has ganado el juego!");

            // Aquí puedes añadir lo que quieras que pase al ganar:
            // Por ejemplo, mostrar un panel de "Victoria" en la UI, detener el tiempo, etc.

            // Ejemplo: Cargar una escena de victoria.
            // Asegúrate de que la escena esté añadida en Build Settings en Unity (File > Build Settings).
            if (!string.IsNullOrEmpty(nombreDeLaEscenaDeVictoria))
            {
                SceneManager.LoadScene(nombreDeLaEscenaDeVictoria);
            }
            else
            {
                // Si no se especifica una escena, simplemente puedes pausar el juego
                // o activar un UI de victoria aquí.
                Time.timeScale = 0f; // Pausa el juego
                // Debug.Log("No se especificó una escena de victoria, el juego se ha pausado.");
                // Aquí podrías activar un panel de UI que diga "¡Ganaste!"
            }
        }
    }
}
