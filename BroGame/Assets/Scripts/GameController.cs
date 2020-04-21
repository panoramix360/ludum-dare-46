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
  public const bool DEBUG = true;
  public const int MAX_DAYS = 7;

  public const int MAX_PLAYER_LEVELS = 4;

  public int available_slots = 3;

  public int brometerLvl = 0;

  public const int MAX_SLOTS = 5;

  public int currentDay = 1;

  // player transformation level
  public int currentPlayerLevel = 1;

  public const string character = "apollo";

  public PlayerAttribute[] attrs =
  {
      new PlayerAttribute(Constants.CashAttribute, value_: 10, maxValue_: 70, color_: new Color32(41, 154, 10, 255), iconName_: "money_icon 1"),
      new PlayerAttribute(Constants.ManlinessAttribute, maxValue_: 70, color_: new Color32(254, 102, 0, 255), iconName_: "strength_icon 1"),
      new PlayerAttribute(Constants.WillpowerAttribute, maxValue_: 70, color_: new Color32(31, 226, 255, 255), iconName_: "willpower_icon 1"),
    };

  public OrderedDictionary attributes = new OrderedDictionary();

  public PlayerAttribute brometer = new PlayerAttribute(Constants.BrometerAttribute, maxValue_: 210);

  public Dictionary<string, PlayerAttrReward[]> rewards = new Dictionary<string, PlayerAttrReward[]>();

  private Stack<string> currentActivities = new Stack<string>();
  private List<string> schedule;

  public bool awake = false;

  private void Awake()
  {
    base.Awake();

    Debug.Log("GC Started");

    foreach (var attr in attrs)
    {
      attributes.Add(attr.name, attr);
    }

    rewards[Constants.EatActivity] = new PlayerAttrReward[] {
      new PlayerAttrReward(Constants.CashAttribute, -10),
      new PlayerAttrReward(Constants.WillpowerAttribute, +12),
    };
    rewards[Constants.PoseActivity] = new PlayerAttrReward[] {
      new PlayerAttrReward(Constants.CashAttribute, +20),
      new PlayerAttrReward(Constants.ManlinessAttribute, -5),
    };
    rewards[Constants.PunchActivity] = new PlayerAttrReward[] {
      new PlayerAttrReward(Constants.ManlinessAttribute, +10),
    };

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
    foreach (string activity in schedule)
    {
      var bonusList = rewards[activity];
      foreach (var bonus in bonusList)
      {
        ((PlayerAttribute)attributes[bonus.attr]).value += bonus.reward;
      }
    }

    var manlinessAttr = attributes[Constants.ManlinessAttribute] as PlayerAttribute;
    if (manlinessAttr.percentValue < .5)
    {
      currentPlayerLevel = 1;
    }
    else if (manlinessAttr.percentValue < .7)
    {
      currentPlayerLevel = 2;
    }
    else if (manlinessAttr.percentValue < .9)
    {
      currentPlayerLevel = 3;
    }
    else
    {
      currentPlayerLevel = 4;
    }

    var willpowerAttr = attributes[Constants.WillpowerAttribute] as PlayerAttribute;
    if (willpowerAttr.percentValue < .3)
    {
      available_slots = 3;
    }
    else if (willpowerAttr.percentValue < .6)
    {
      available_slots = 4;
    }
    else
    {
      available_slots = 5;
    }

    currentDay += 1;

    if (brometer.percentValue < .1)
    {
      brometerLvl = 0;
    }
    else if (brometer.percentValue < .3)
    {
      brometerLvl = 1;
    }
    else if (brometer.percentValue < .5)
    {
      brometerLvl = 2;
    }
    else if (brometer.percentValue < .85)
    {
      brometerLvl = 3;
    }
    else
    {
      brometerLvl = 4;
    }

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

  public List<PlayerAttribute> GetVisibleAttributes()
  {
    return attributes.Values.Cast<PlayerAttribute>().ToList();
  }

  public void UpdateMinigameScore(string activity, int score)
  {
    if (activity == Constants.EatActivity)
    {
      // 3 levels * 8 pancakes = 24 max points
      brometer.value += (int)(score * 10f / 24f);
    }
    else if (activity == Constants.PoseActivity)
    {
      // 2 + 4 + 5 + 6 = 17 max points
      brometer.value += (int)(score * 10f / 17f);
    }
    else if (activity == Constants.PunchActivity)
    {
      // 20 ?
      brometer.value += (int)(score * 10f / 20f);
    }
  }

  public void LoadScene(string sceneName)
  {
    SceneManager.LoadScene(sceneName);
  }
}
