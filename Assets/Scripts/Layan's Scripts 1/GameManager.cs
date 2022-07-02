using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject quitGame;
    [SerializeField] GameObject startGame;

    [SerializeField] PlayableDirector cutscene;

    [SerializeField] GameObject cutsceneCamera;
    [SerializeField] GameObject mainCamera;
    [SerializeField]GameObject reference;
    private PlayerMovement1 player;
    private PlayerCamMovement playerCam;
    private void Start()
    {
        player = reference.GetComponent<PlayerMovement1>();
        playerCam = mainCamera.GetComponent<PlayerCamMovement>();
        player.enabled = false;
        player.animator.enabled = false;
        playerCam.enabled = false;
        cutscene = GetComponent<PlayableDirector>();
    }

    private void Update()
    {
        CutsceneDone();
    }

    private void CutsceneDone()
    {
        if (cutscene.state != PlayState.Playing)
        {
            Debug.Log("done");
            player.enabled = true;
            player.animator.enabled = true;
            cutsceneCamera.SetActive(false);
            playerCam.enabled = true;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void NextLvl()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
