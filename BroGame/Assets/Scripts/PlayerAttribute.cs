using UnityEngine;

public class PlayerAttribute
{
  public string name { get; set; }
  public int value { get; set; }
  public int maxValue { get; set; }
  public float percentValue { get { return (float)value / maxValue; } }

  public Color32 color { get; set; }

  public string iconName { get; set; }

  public PlayerAttribute(string name_, int maxValue_, string iconName_ = null, Color32 color_ = default, int value_ = 0)
  {
    name = name_;
    value = value_;
    maxValue = maxValue_;
    color = color_;
    iconName = iconName_;
  }
}