using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] int sceneIndex;
    public void PlayGame()
    {
        SceneManager.LoadScene(sceneIndex);
    }

}
