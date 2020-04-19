﻿using System.Linq;
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

  public PlayerAttribute brometer = new PlayerAttribute(Constants.AttributeBrometer, maxValue_: 100);

  private void Awake()
  {
    Debug.Log("GC Started");

    PlayerAttribute[] attrs =
    {
      new PlayerAttribute(Constants.AttributeCash, value_: 50, maxValue_: 100, color_: new Color32(80, 200, 20, 255)),
      new PlayerAttribute(Constants.AttributeNutrition, maxValue_: 100, color_: new Color32(200, 150, 20, 255)),
      new PlayerAttribute(Constants.AttributeManliness, maxValue_: 100, color_: new Color32(20, 50, 200, 255)),
      new PlayerAttribute(Constants.AttributeFanbase, visible_: false, maxValue_: 100),
      new PlayerAttribute(Constants.AttributeWillpower, maxValue_: 100, color_: new Color32(200, 20, 200, 255)),
    };

    foreach (var attr in attrs)
    {
      attributes.Add(attr.name, attr);
    }
  }

  public void NextDay()
  {
    currentDay += 1;
  }

  public List<PlayerAttribute> GetVisibleAttributes()
  {
    return attributes.Values.Cast<PlayerAttribute>().Where(attr => attr.visible).ToList();
  }
}
