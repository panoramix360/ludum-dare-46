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
  private int _currPlayerLvl = 1;

  public int currentPlayerLevel { get { return _currPlayerLvl; } }

  public const string character = "apollo";

  public PlayerAttribute brometer = new PlayerAttribute(Constants.BrometerAttribute, maxValue_: 0);

  private Stack<string> currentActivities = new Stack<string>();

  private Dictionary<string, ActivityTracker> trackers = new Dictionary<string, ActivityTracker>
  {
    { Constants.EatActivity, new ActivityTracker() },
    { Constants.PoseActivity, new ActivityTracker() },
    { Constants.PunchActivity, new ActivityTracker() },
  };

  private List<Quest> quests = new List<Quest>
  {
    new Quest(Constants.EatActivity, "eat at least 10 pancakes in a game", (tracker) => tracker.lastScore >= 10),
    new Quest(Constants.EatActivity, "eat at least 30 pancakes in total", (tracker) => tracker.runningScore >= 30),
    new Quest(Constants.PunchActivity, "score at least 8 punches", (tracker) => tracker.lastScore >= 8),
    new Quest(Constants.PunchActivity, "score at least 50 punches", (tracker) => tracker.runningScore >= 50),
    new Quest(Constants.PunchActivity, "beat your best score on the punching bag after the fifth day",
      (tracker) => tracker.bestScore == tracker.lastScore && tracker.timesPlayed > 0 && tracker.currentDay >= 5),
    new Quest(Constants.PoseActivity, "score at least 1 pose", (tracker) => tracker.lastScore >= 1),
    new Quest(Constants.PoseActivity, "score at least 3 poses", (tracker) => tracker.lastScore >= 3),
  };

  private void Awake()
  {
    base.Awake();
    Debug.Log("GC Started");
    brometer.maxValue = quests.Count();
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
      _currPlayerLvl = 1;
    }
    else if (brometer.percentValue < .35)
    {
      _currPlayerLvl = 2;
    }
    else if (brometer.value < brometer.maxValue)
    {
      _currPlayerLvl = 3;
    }
    else
    {
      _currPlayerLvl = 4;
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
    var tracker = this.trackers[activity];
    tracker.lastScore = score;
    tracker.bestScore = Math.Max(tracker.bestScore, score);
    tracker.runningScore += score;
    tracker.timesPlayed += 1;
    tracker.currentDay = this.currentDay;

    // get quests that were not done yet for this activity
    var pendingQuests = quests.Where(q => !q.done && q.activity == activity).ToList();

    // check completed quests and update the brometer
    foreach (var quest in pendingQuests)
    {
      bool completed = quest.completionFunc(tracker);
      if (completed)
      {
        Debug.Log($"Finsihed quest \"{quest.label}\"");
        quest.done = true;
        this.brometer.Increment();
      }
    }

  }

  public void LoadScene(string sceneName)
  {
    SceneManager.LoadScene(sceneName);
  }
}
