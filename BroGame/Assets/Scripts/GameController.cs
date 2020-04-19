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
  public int currentDay { get; set; }

  public OrderedDictionary attributes = new OrderedDictionary();

  public PlayerAttribute brometer = new PlayerAttribute(Constants.BrometerAttribute, maxValue_: 100);

  private Dictionary<string, PlayerAttrReward[]> rewards = new Dictionary<string, PlayerAttrReward[]>();

  private void Awake()
  {
    Debug.Log("GC Started");

    PlayerAttribute[] attrs =
    {
      new PlayerAttribute(Constants.CashAttribute, value_: 50, maxValue_: 100, color_: new Color32(80, 200, 20, 255)),
      new PlayerAttribute(Constants.NutritionAttribute, maxValue_: 100, color_: new Color32(200, 150, 20, 255)),
      new PlayerAttribute(Constants.ManlinessAttribute, maxValue_: 100, color_: new Color32(20, 50, 200, 255)),
      new PlayerAttribute(Constants.FanbaseAttribute, visible_: false, maxValue_: 100),
      new PlayerAttribute(Constants.WillpowerAttribute, maxValue_: 100, color_: new Color32(200, 20, 200, 255)),
    };

    foreach (var attr in attrs)
    {
      attributes.Add(attr.name, attr);
    }

    rewards[Constants.PlannedActionEat] = new PlayerAttrReward[] {
      new PlayerAttrReward(Constants.CashAttribute, 20),
    };
    rewards[Constants.PlannedActionSleep] = new PlayerAttrReward[] {
      new PlayerAttrReward(Constants.NutritionAttribute, 20),
    };
    rewards[Constants.PlannedActionWorkout] = new PlayerAttrReward[] {
      new PlayerAttrReward(Constants.ManlinessAttribute, 20),
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

    currentDay += 1;
  }

  public List<PlayerAttribute> GetVisibleAttributes()
  {
    return attributes.Values.Cast<PlayerAttribute>().Where(attr => attr.visible).ToList();
  }
}
