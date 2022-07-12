using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using TMPro;

public class CutsceneHandler : MonoBehaviour
{
    [Header("GameObjects")]
   
    [SerializeField] PlayableDirector cutscene;
    [SerializeField] private double _skipIntro = 40f;
    [SerializeField] GameObject cutsceneCamera;
    [SerializeField] GameObject secondCutsceneCamera;
    [SerializeField] GameObject mainCamera;
    [SerializeField] GameObject playerReference;
    [SerializeField] GameObject gameManagerReference;
    [SerializeField] GameObject enemies;

    [Header("References")]
    private PlayerMovement1 _player;
    private PlayerCamMovement _playerCam;
    private GameManager _gameManager;

    [Header("UI")]
    public TMP_Text skipIntroText;

    private void Start()
    {
        skipIntroText.text = "Press F to skip";
        //getting references for player and cam to deactivate them
        _player = playerReference.GetComponent<PlayerMovement1>();
        _playerCam = mainCamera.GetComponent<PlayerCamMovement>();
        _gameManager = gameManagerReference.GetComponent<GameManager>();
        enemies.SetActive(false);
        _player.enabled = false;
        _player.animator.enabled = false;
        _playerCam.enabled = false;
        cutscene = GetComponent<PlayableDirector>();
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
            skipIntroText.enabled = false;
            _gameManager.canStartTimer = true;
            enemies.SetActive(true);
        }
        //if cutscene is done, enable scripts and game + disable the extra cameras
        else if (cutscene.state != PlayState.Playing)
        {
            _player.enabled = true;
            _player.animator.enabled = true;
            cutsceneCamera.SetActive(false);
            _playerCam.enabled = true;
            Debug.Log("done");
            skipIntroText.enabled = false;
            _gameManager.canStartTimer = true;
            enemies.SetActive(true);
            if (secondCutsceneCamera != null)
            {
                secondCutsceneCamera.SetActive(false);
            }
        }
    }

}
