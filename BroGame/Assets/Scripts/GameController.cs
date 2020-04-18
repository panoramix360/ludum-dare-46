using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using UnityEngine.SceneManagement;

public class GameController : Singleton<GameController>
{
  public int currentDay = 1;

  private void Start()
  {
    Debug.Log("Started");
  }

  public void NextDay()
  {
    currentDay += 1;
  }
}
