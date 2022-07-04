using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    [Header("GameObjects")]
    [SerializeField] GameObject quitGame;
    [SerializeField] GameObject startGame;
    [SerializeField] GameObject loadingUI;
    [SerializeField] Image loadingImage;



    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        //SceneManager.LoadScene(1);
        StartCoroutine(LoadAsync(1));
    }

    IEnumerator LoadAsync(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        loadingUI.SetActive(true);

        while (operation.isDone == false)
        {
            float barProgress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingImage.fillAmount = barProgress;
            yield return null;
        }
    }

    public void NextLvl()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Options()
    {

    }

    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }
}
