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

    [Header("Timer")]
    public float timeLeft;
    public bool over = false;
    [SerializeField] TMP_Text timeUI;

    [Header("Bools")]
    public bool playerWon = false;
    public bool canStartTimer = false;
    public bool isGamePaused = false;

    [SerializeField] string json;

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

            //if the resolution from list is the same as our current screen resolution then we say current resolution is i (from list)
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        //we add the list into dropdown, then the values are the current resolution index we got from earlier
        resolutionsDropdown.AddOptions(options);
        resolutionsDropdown.value = currentResolutionIndex;
        resolutionsDropdown.RefreshShownValue();

        //checks if the player never changed quality and volume yet, then sets those as default
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

    //sets the time to a more readable visualization
    //manage the time of the player in the game
    void Stoper()
    {

        if (canStartTimer == true)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft > 0 && timeLeft > 60)
            {
                var minutes = Mathf.FloorToInt(timeLeft / 60);
                var seconds = Mathf.FloorToInt(timeLeft % 60);
                //0
                if (seconds < 10)
                {
                    timeUI.text = minutes + ":0" + seconds;
                }
                //set 00:00
                else
                {
                    timeUI.text = minutes + ":" + seconds;
                }
                over = false;
            }
            //set from 00:00 to 00
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

    public void Pause()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0 && over == false && playerWon == false)
        {
            if (Input.GetKeyDown(KeyCode.Escape) && isGamePaused == false)
            {
                isGamePaused = true;
                mainMenuCanvas.SetActive(true);
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                Time.timeScale = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Escape) && isGamePaused == true)
            {
                isGamePaused = false;
                mainMenuCanvas.SetActive(false);
                optionsCanvas.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
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

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat(SAVE_VOLUME, volumeSlider.value);
        PlayerPrefs.SetInt(SAVE_QUALITY, qualityDropdown.value);
    }

    public void SetVolume()
    {
        AudioListener.volume = volumeSlider.value;
    }

    public void SetResolution(int resolutionIndex)
    {
        //gets the index of the drop down and puts all the possible resolution
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Tutorial()
    {
        StartCoroutine(LoadAsync(3));
    }
    
    public void StartGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        StartCoroutine(LoadAsync(1));
    }

    //Loading scenes 
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

    public void Options()
    {
        optionsCanvas.SetActive(true);
        mainMenuCanvas.SetActive(false);
    }

    //create a file inside unity, that holds the progress of the player, by the scene index
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

    }

    //Takes the file that we created, translated it to readable way
    //take the index that we saved and send it to the LoadAsync function
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
        }
    }
    
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    public void NextLvl()
    {
        Cursor.visible = false;
        StartCoroutine(LoadAsync(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Back()
    {
        optionsCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }

    public void FullScreen(bool isFullScreenOn)
    {
        Screen.fullScreen = isFullScreenOn;
    }

    public void LoseScreen()
    {
        if (over== true)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            loseCanvas.SetActive(true);
            Debug.Log("lose canvas");
            Time.timeScale = 0;
        }
    }

    public void WinScreen()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        winCanvas.SetActive(true);
        Time.timeScale = 0;
        playerWon = true;
    }

}

//we create a class who holds on the position in the game (progress)
[System.Serializable]
public class Progress
{
    public int level;

}


