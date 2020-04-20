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
  public const int MAX_DAYS = 7;

  public const int MAX_PLAYER_LEVELS = 4;

  public int available_slots = 3;

  public const int MAX_SLOTS = 5;

  public int currentDay = 1;

  // player transformation level
  public int currentPlayerLevel = 1;

  public string character = "rocky";

  public PlayerAttribute[] attrs =
  {
      new PlayerAttribute(Constants.CashAttribute, value_: 10, maxValue_: 70, color_: new Color32(80, 200, 20, 255)),
      new PlayerAttribute(Constants.ManlinessAttribute, maxValue_: 70, color_: new Color32(20, 50, 200, 255)),
      new PlayerAttribute(Constants.WillpowerAttribute, maxValue_: 70, color_: new Color32(200, 20, 200, 255)),
    };

  public OrderedDictionary attributes = new OrderedDictionary();

  public PlayerAttribute brometer = new PlayerAttribute(Constants.BrometerAttribute, maxValue_: 100);

  public Dictionary<string, PlayerAttrReward[]> rewards = new Dictionary<string, PlayerAttrReward[]>();

  private void Awake()
  {
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
  }

  public void ExecuteSchedule(List<string> activities)
  {
    foreach (string activity in activities)
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
  }

  public List<PlayerAttribute> GetVisibleAttributes()
  {
    return attributes.Values.Cast<PlayerAttribute>().ToList();
  }
}
