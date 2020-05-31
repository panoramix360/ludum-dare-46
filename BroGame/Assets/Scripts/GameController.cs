using System.Runtime.CompilerServices;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using UnityEngine.SceneManagement;

public class GameController : Singleton<GameController>
{
  public const bool DEBUG = false;
  public const int MAX_DAYS = 10;

  public const int MAX_PLAYER_LEVELS = 4;

  public int available_slots = 1;

  public int brometerLvl = 0;

  public const int MAX_SLOTS = 1;

  public int currentDay = 1;

  // player transformation level
  public int currentPlayerLevel = 1;

  public const string character = "apollo";

  public PlayerAttribute brometer = new PlayerAttribute(Constants.BrometerAttribute, maxValue_: MAX_DAYS);

  private Stack<string> currentActivities = new Stack<string>();
  private List<string> schedule;

  public bool awake = false;

  private void Awake()
  {
    base.Awake();
    Debug.Log("GC Started");
    awake = true;
  }

  public void NextActivity()
  {
    var activity = currentActivities.Pop();
    Debug.Log($"NEXT ACTIVITY {activity}");

    if (activity == Constants.HomeActivity)
    {
      if (OnDayEnd())
      {
        return;
      }
    }
    LoadScene(Constants.ActivityToScene[activity]);
  }

  public bool OnDayEnd()
  {
    if (brometer.percentValue < .2)
    {
      currentPlayerLevel = 1;
    }
    else if (brometer.percentValue < .35)
    {
      currentPlayerLevel = 2;
    }
    else if (brometer.value < brometer.maxValue)
    {
      currentPlayerLevel = 3;
    }
    else
    {
      currentPlayerLevel = 4;
    }

    currentDay += 1;

    // end the game
    Debug.Log($"current day: {currentDay}");
    if (currentDay >= MAX_DAYS)
    {
      LoadScene("GameEnd");
      return true;
    }

    return false;
  }

  public void ExecuteSchedule(List<string> activities)
  {
    schedule = activities;

    currentActivities.Push(Constants.HomeActivity);
    foreach (var activity in activities.Select(a => a).Reverse())
    {
      currentActivities.Push(activity);
    }
    Debug.Log(currentActivities.Count);

    NextActivity();
  }

  public void UpdateMinigameScore(string activity, int score)
  {
    this.brometer.Increment();
  }

  public void LoadScene(string sceneName)
  {
    SceneManager.LoadScene(sceneName);
  }
}
