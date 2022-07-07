using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using UnityEngine.UI;
using TMPro;

public class UIHandler : MonoBehaviour
{
    [Header("GameObjects")]
    [SerializeField] GameObject quitGame;
    [SerializeField] GameObject startGame;
    [SerializeField] GameObject backButton;
    [SerializeField] GameObject loadingUI;
    [SerializeField] Image loadingImage;

    [SerializeField] GameObject optionsCanvas;
    [SerializeField] GameObject mainMenuCanvas;
    [SerializeField] TMP_Dropdown resolutionsDropdown;


    public bool isGamePaused = false;
    Resolution[] resolutions;

    private void Start()
    {
        int currentResolutionIndex = 0;

        resolutions = Screen.resolutions;
        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].ToString();
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionsDropdown.AddOptions(options);
        resolutionsDropdown.value = currentResolutionIndex;
        resolutionsDropdown.RefreshShownValue();
    }

    private void Update()
    {
        Pause();
    }

    public void Pause()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (Input.GetKeyDown(KeyCode.Escape) && isGamePaused == false)
            {
                isGamePaused = true;
                mainMenuCanvas.SetActive(true);
                Cursor.lockState = CursorLockMode.Confined;
                Time.timeScale = 0;
                Debug.Log("paused");
            }
            else if (Input.GetKeyDown(KeyCode.Escape) && isGamePaused == true)
            {
                isGamePaused = false;
                mainMenuCanvas.SetActive(false);
                optionsCanvas.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Time.timeScale = 1;
            }
        }
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Tutorial()
    {
        StartCoroutine(LoadAsync(2));
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

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    public void NextLvl()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Options()
    {
        optionsCanvas.SetActive(true);
        mainMenuCanvas.SetActive(false);
    }

    public void Back()
    {
        optionsCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }

    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    public void FullScreen(bool isFullScreenOn)
    {
        Screen.fullScreen = isFullScreenOn;
    }
}