using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneHandler : MonoBehaviour
{
    [Header("GameObjects")]
   
    [SerializeField] PlayableDirector cutscene;
    [SerializeField] private double _skipIntro = 40f;
    [SerializeField] GameObject cutsceneCamera;
    [SerializeField] GameObject mainCamera;
    [SerializeField] GameObject playerReference;

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

}
