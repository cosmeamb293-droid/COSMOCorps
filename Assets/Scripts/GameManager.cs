
using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para manejar escenas
using TMPro; // Necesario para TextMeshPro

public class GameManager : MonoBehaviour
{
    // --- SINGLETON PATTERN ---
    public static GameManager Instance { get; private set; }

    // --- ESTADOS DEL JUEGO ---
    public enum GameState { Playing, Paused, GameOver }
    public GameState currentState { get; private set; }

    // --- REFERENCIAS A UI ---
    [Header("UI Panels")]
    public GameObject panelDePausa;
    public GameObject panelDeGameOver;

    [Header("UI Text")]
    public TextMeshProUGUI textoDePuntaje;

    // --- VARIABLES DEL JUEGO ---
    private int puntaje = 0;

    // Awake se llama antes que cualquier método Start
    void Awake()
    {
        // Implementación del Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Si ya existe una instancia, destruye esta
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Opcional: Mantiene el GameManager al cambiar de escena
        }
    }

    void Start()
    {
        // Estado inicial del juego
        Time.timeScale = 1f; // Asegurarse que el tiempo corre
        currentState = GameState.Playing;
        ActualizarPuntaje(0);

        // Ocultar paneles al inicio
        if(panelDePausa != null) panelDePausa.SetActive(false);
        if(panelDeGameOver != null) panelDeGameOver.SetActive(false);
    }

    void Update()
    {
        // Chequeo para pausar/reanudar el juego
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Playing || currentState == GameState.Paused)
            {
                TogglePausa();
            }
        }
    }

    // --- MÉTODOS PÚBLICOS ---

    public void AñadirPuntaje(int puntos)
    {
        if (currentState != GameState.Playing) return; // No sumar puntos si no se está jugando
        
        puntaje += puntos;
        ActualizarPuntaje(puntaje);
    }

    public void TerminarPartida()
    {
        if (currentState == GameState.GameOver) return; // Evitar llamadas múltiples

        currentState = GameState.GameOver;
        Time.timeScale = 0f; // Congelar el juego
        if(panelDeGameOver != null) panelDeGameOver.SetActive(true);
        Debug.Log("Partida Terminada. Puntaje Final: " + puntaje);
    }

    public void ReiniciarPartida()
    {
        // Reinicia la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void TogglePausa()
    {
        if (currentState == GameState.GameOver) return;

        currentState = (currentState == GameState.Playing) ? GameState.Paused : GameState.Playing;

        if (currentState == GameState.Paused)
        {
            Time.timeScale = 0f; // Congelar el juego
            if(panelDePausa != null) panelDePausa.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f; // Reanudar el tiempo
            if(panelDePausa != null) panelDePausa.SetActive(false);
        }
    }

    // --- MÉTODOS PRIVADOS ---

    private void ActualizarPuntaje(int nuevoPuntaje)
    {
        puntaje = nuevoPuntaje;
        if (textoDePuntaje != null)
        {
            textoDePuntaje.text = "Puntaje: " + puntaje;
        }
    }
}
