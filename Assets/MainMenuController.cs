using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {
    public void GoLevelSelect() {
        SceneManager.LoadScene(1);
    }
    public void GoAbout() {
        SceneManager.LoadScene(3);
    }
    public void GoExit() {
        Application.Quit();
    }
}
