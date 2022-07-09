using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using UnityEngine.UI;
using TMPro;

public class UIHandler : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] GameObject loadingUI;
    [SerializeField] GameObject winPoints;
    [SerializeField] Image loadingImage;
    [SerializeField] Slider volumeSlider;
    [SerializeField] TMP_Dropdown resolutionsDropdown;
    [SerializeField] TMP_Dropdown qualityDropdown;

    [Header("Canvas")]
    [SerializeField] GameObject optionsCanvas;
    [SerializeField] GameObject mainMenuCanvas;
    [SerializeField] GameObject loseCanvas;

    [Header("Consts")]
    const string SAVE_VOLUME = "musicVolume";
    const string SAVE_QUALITY = "qualitySettings";
    const string ACTIVATION_TAG = "Player";

    [Header("Timer")]
    public float timeLeft;
    [SerializeField] TMP_Text timeUI;
    public bool over = false;

    public bool isGamePaused = false;
    Resolution[] resolutions;

    private void Start()
    {
        int currentResolutionIndex = 0;
        Time.timeScale = 1;

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

        if (!PlayerPrefs.HasKey(SAVE_VOLUME))
        {
            PlayerPrefs.SetFloat(SAVE_VOLUME, 1);
            LoadSettings();
        }
        else if (!PlayerPrefs.HasKey(SAVE_QUALITY))
        {
            PlayerPrefs.SetInt(SAVE_QUALITY, 2);
            LoadSettings();
        }
        else
        {
            LoadSettings();
        }
    }

    private void Update()
    {
        Pause();
        SaveSettings();
        if (timeUI != null)
        {
            Stoper();
            LoseScreen();
        }

        if (winPoints != null)
        {
            WinCondition();
        }
    }

    void Stoper()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft > 0 && timeLeft > 60)
        {
            var minutes = Mathf.FloorToInt(timeLeft / 60);
            var seconds = Mathf.FloorToInt(timeLeft % 60);
            if (seconds < 10)
            {
                timeUI.text = minutes + ":0" + seconds;
            }
            else
            {
                timeUI.text = minutes + ":" + seconds;
            }
            over = false;
        }
        else if (timeLeft < 60 && timeLeft > 0)
        {
            Debug.Log("test");
            var seconds = Mathf.FloorToInt(timeLeft % 60);
            timeUI.text = "" + seconds;
            over = false;
        }
        else if (timeLeft <= 0)
        {
            over = true;
            Debug.Log("over");
            timeLeft = 0;
            timeUI.text = "Times UP! Game OVER";
            //Time.timeScale = 0; 
        }
    }

    public void Pause()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0 && over == false)
        {
            if (Input.GetKeyDown(KeyCode.Escape) && isGamePaused == false)
            {
                isGamePaused = true;
                mainMenuCanvas.SetActive(true);
                Cursor.lockState = CursorLockMode.Confined;
                Time.timeScale = 0;
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

    //Receives values of what the user inputed
    private void LoadSettings()
    {
        volumeSlider.value = PlayerPrefs.GetFloat(SAVE_VOLUME);
        qualityDropdown.value = PlayerPrefs.GetInt(SAVE_QUALITY);

    }

    //Saves values of what the user inputed
    private void SaveSettings()
    {
        PlayerPrefs.SetFloat(SAVE_VOLUME, volumeSlider.value);
        PlayerPrefs.SetInt(SAVE_QUALITY, qualityDropdown.value);
    }
    
    private void SetVolume()
    {
        AudioListener.volume = volumeSlider.value;
    }

    private void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    
    private void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }
    
    private void QuitGame()
    {
        Application.Quit();
    }
    
    private void Tutorial()
    {
        StartCoroutine(LoadAsync(2));
    }
    
    private void StartGame()
    {
        StartCoroutine(LoadAsync(1));
    }

    public IEnumerator LoadAsync(int sceneIndex)
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

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }
    
    private void NextLvl()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    private void Options()
    {
        optionsCanvas.SetActive(true);
        mainMenuCanvas.SetActive(false);
    }
    
    private void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    
    private void Back()
    {
        optionsCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }
    
    private void FullScreen(bool isFullScreenOn)
    {
        Screen.fullScreen = isFullScreenOn;
    }
    
    private void LoseScreen()
    {
        if (over== true)
        {
            loseCanvas.SetActive(true);
            Debug.Log("lose canvas");
            Time.timeScale = 0;
        }
    }

    //general win condition
    private void WinCondition()
    { 
        if (winPoints.transform.tag == ACTIVATION_TAG)
        {
            if (SceneManager.GetActiveScene().buildIndex == 2)
            {
                StartCoroutine(LoadAsync(1));
            }
            else if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                StartCoroutine(LoadAsync(0));
            }
        }
    }
}

