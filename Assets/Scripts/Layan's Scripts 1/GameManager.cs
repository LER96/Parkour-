using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("GameObjects")]
    [SerializeField] GameObject quitGame;
    [SerializeField] GameObject startGame;
    [SerializeField] PlayableDirector cutscene;
    [SerializeField] private double _skipIntro = 40f;
    [SerializeField] GameObject cutsceneCamera;
    [SerializeField] GameObject mainCamera;
    [SerializeField] GameObject playerReference;
    [SerializeField] GameObject loadingUI;
    [SerializeField] Slider loadingSlider;


    [Header("References")]
    private PlayerMovement1 player;
    private PlayerCamMovement playerCam;
    private void Start()
    {
        player = playerReference.GetComponent<PlayerMovement1>();
        playerCam = mainCamera.GetComponent<PlayerCamMovement>();
        player.enabled = false;
        player.animator.enabled = false;
        playerCam.enabled = false;
        cutscene = GetComponent<PlayableDirector>();
        loadingUI.SetActive(false);
    }

    private void Update()
    {
        CutsceneDone();
    }

    private void CutsceneDone()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            cutscene.time = _skipIntro;
        }
        else if (cutscene.state != PlayState.Playing)
        {
            
            player.enabled = true;
            player.animator.enabled = true;
            cutsceneCamera.SetActive(false);
            playerCam.enabled = true;
            Debug.Log("done");
        }
    }

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
            loadingSlider.value = barProgress;
            yield return null;
        }
    }

    public void NextLvl()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
