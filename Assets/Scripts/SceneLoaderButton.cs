using UnityEngine;

/// <summary>
/// Este script se añade a un botón de UI para cargar una escena específica
/// a través del GameManager singleton. Esto evita problemas con referencias
/// de escena directas en el evento OnClick.
/// </summary>
public class SceneLoaderButton : MonoBehaviour
{
    [Tooltip("El nombre exacto de la escena que se debe cargar.")]
    public string sceneNameToLoad;

    /// <summary>
    /// Este método público será llamado por el evento OnClick del botón.
    /// </summary>
    public void LoadTargetScene()
    {
        // Valida que el nombre de la escena no esté vacío.
        if (string.IsNullOrEmpty(sceneNameToLoad))
        {
            Debug.LogError("SceneLoaderButton: El nombre de la escena (sceneNameToLoad) no ha sido especificado en el Inspector.");
            return;
        }

        // Busca la instancia única del GameManager.
        if (GameManager.Instance != null)
        {
            // Llama al método genérico para cargar la escena.
            GameManager.Instance.LoadScene(sceneNameToLoad);
        }
        else
        {
            Debug.LogError("SceneLoaderButton: No se pudo encontrar una instancia de GameManager en la escena.");
        }
    }
}
