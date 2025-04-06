using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public string urlToOpen = "https://www.example.com"; 
    public string sceneToLoad = "NewScene"; 
    public GameObject objectToToggle;
    private void Start()
    {
        Cursor.visible = true;
    }
    public void OpenLink()
    {
        Application.OpenURL(urlToOpen);
    }


    public  void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }


    public void ToggleObject()
    {
        if (objectToToggle != null)
        {
            objectToToggle.SetActive(!objectToToggle.activeSelf);
        }
        else
        {
            Debug.LogWarning("Object to toggle is not assigned.");
        }
    }
}
