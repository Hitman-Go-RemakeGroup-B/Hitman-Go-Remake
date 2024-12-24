using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] int sceneIndex;

    public TMP_Dropdown dropDown;

    private void Start()
    {
        dropDown.value = QualitySettings.GetQualityLevel();
    }
    public void PlayGame()
    {
        SceneManager.LoadScene(sceneIndex);
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
