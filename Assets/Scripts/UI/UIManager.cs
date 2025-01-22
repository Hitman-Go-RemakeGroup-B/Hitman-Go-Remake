using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    public TMP_Dropdown dropDown;

    private void Start()
    {
        dropDown.value = QualitySettings.GetQualityLevel();
    }
    public void PlayLevel(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void RetryLevel()
    {
        string scene=SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(scene);
    }


    public void ExitGame()
    {
        Application.Quit();
    }

    public void ChangeQuality(int quality)
    {
        QualitySettings.SetQualityLevel(quality);
    }
}
