using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    [SerializeField] private AudioSource introMusic;
    [SerializeField] private Canvas mainCanvas;
    
    private VideoPlayer introCinematics;

    private void Start()
    {
        introCinematics = GetComponent<VideoPlayer>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            mainCanvas.gameObject.SetActive(false);
            introMusic.Stop();
            introCinematics.Play();
            introCinematics.loopPointReached += EndReached;
        }
    }

    private void EndReached(VideoPlayer vp)
    {
        SceneManager.LoadScene("Bedroom");
    }
}
