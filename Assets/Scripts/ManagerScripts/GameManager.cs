using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using UnityEngine.UI;
using TMPro;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameManager : MonoBehaviour
{
    [Header("UI *Objects")]
    [SerializeField] GameObject loadingUI;
    [SerializeField] Image loadingImage;
    [SerializeField] Slider volumeSlider;
    [SerializeField] TMP_Dropdown resolutionsDropdown;
    [SerializeField] TMP_Dropdown qualityDropdown;

    [Header("Canvas")]
    [SerializeField] GameObject optionsCanvas;
    [SerializeField] GameObject mainMenuCanvas;
    [SerializeField] GameObject loseCanvas;
    [SerializeField] GameObject winCanvas;

    [Header("Consts")]
    const string SAVE_VOLUME = "musicVolume";
    const string SAVE_QUALITY = "qualitySettings";
    const string ACTIVATION_TAG = "Player";

    [Header("Timer")]
    public float timeLeft;
    [SerializeField] TMP_Text timeUI;
    public bool over = false;

    [Header("Bools")]
    public bool playerWon = false;
    public bool canStartTimer = false;

    [SerializeField] string json;

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
    }

    void Stoper()
    {
        if (canStartTimer == true)
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
    }

    //pause option
    public void Pause()
    {
        //if scene isnt main menu, and player didnt win/lose, you can pause
        if (SceneManager.GetActiveScene().buildIndex != 0 && over == false && playerWon == false)
        {
            //if player presses escape and game isnt paused, pause
            if (Input.GetKeyDown(KeyCode.Escape) && isGamePaused == false)
            {
                isGamePaused = true;
                mainMenuCanvas.SetActive(true);
                Cursor.lockState = CursorLockMode.Confined;
                Time.timeScale = 0;
            }
            //if player presses escape and game is paused, unpause
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
    public void LoadSettings()
    {
        volumeSlider.value = PlayerPrefs.GetFloat(SAVE_VOLUME);
        qualityDropdown.value = PlayerPrefs.GetInt(SAVE_QUALITY);
    }

    //Saves values of what the user inputed
    public void SaveSettings()
    {
        PlayerPrefs.SetFloat(SAVE_VOLUME, volumeSlider.value);
        PlayerPrefs.SetInt(SAVE_QUALITY, qualityDropdown.value);
    }

    //general volume control
    public void SetVolume()
    {
        AudioListener.volume = volumeSlider.value;
    }

    //choosing resolution for the game
    public void SetResolution(int resolutionIndex)
    {
        //gets the index of the drop down and puts all the possible resolution
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    //setting quality
    public void SetQuality(int index)
    {
        //takes the index of the dropdown in unity
        QualitySettings.SetQualityLevel(index);
    }

    //quit button
    public void QuitGame()
    {
        Application.Quit();
    }
    //option to choose tutorial
    public void Tutorial()
    {
        //loads to tutorial with the corutine
        StartCoroutine(LoadAsync(3));
    }
    
    public void StartGame()
    {
        //loads the game with corutine
        StartCoroutine(LoadAsync(1));
    }

    //Loading 
    public IEnumerator LoadAsync(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        loadingUI.SetActive(true);

        //while the scene didnt finish loading yet
        while (operation.isDone == false)
        {
            //progress the bar based on the operation progress
            float barProgress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingImage.fillAmount = barProgress;
            yield return null;
        }
    }

    //options menu
    public void Options()
    {
        optionsCanvas.SetActive(true);
        mainMenuCanvas.SetActive(false);
    }

    //Json
    public static void BinarySave()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.nice";
        FileStream stream = new FileStream(path, FileMode.Create);

        Progress pro = new Progress();
        int y = SceneManager.GetActiveScene().buildIndex;
        pro.level = y;
        formatter.Serialize(stream, pro);
        stream.Close();

        //JsonProgress progress = new JsonProgress();
        //int y = SceneManager.GetActiveScene().buildIndex;
        //progress.level = y;
        //json = JsonUtility.ToJson(progress);
        ////File.WriteAllText(Application.persistentDataPath + "/JsonProgress.json", json);
        //Debug.Log(json);
    }

    public void BinaryLoad()
    {
        string path = Application.persistentDataPath + "/player.nice";
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            Progress pro= formatter.Deserialize(stream) as Progress;
            stream.Close();
            StartCoroutine(LoadAsync(pro.level));
        }
        else
        {
            Debug.LogError("Not Found" + path);
            //return null;
        }
        //Progress loadProgress = JsonUtility.FromJson<Progress>(json);
        //int y = loadProgress.level;
        //Debug.Log("Loaded");
        //StartCoroutine(LoadAsync(y));
    }
    
    //restart scene
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    //go to next level when you win
    public void NextLvl()
    {
        StartCoroutine(LoadAsync(SceneManager.GetActiveScene().buildIndex + 1));
    }

    //go back to main menu
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    //back button from options
    public void Back()
    {
        optionsCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }

    //set game to full screen
    public void FullScreen(bool isFullScreenOn)
    {
        Screen.fullScreen = isFullScreenOn;
    }

    //Win/Lose
    public void LoseScreen()
    {
        if (over== true)
        {
            Cursor.lockState = CursorLockMode.Confined;
            loseCanvas.SetActive(true);
            Debug.Log("lose canvas");
            Time.timeScale = 0;
        }
    }

    public void WinScreen()
    {
        Cursor.lockState = CursorLockMode.Confined;
        winCanvas.SetActive(true);
        Time.timeScale = 0;
        playerWon = true;
    }

}
[System.Serializable]
public class Progress
{
    public int level;

    //public Vector3 potision;
    //public Quaternion rotation;
}


