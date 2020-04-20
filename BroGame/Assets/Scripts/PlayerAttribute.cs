using UnityEngine;

public class PlayerAttribute
{
  public string name { get; set; }
  public int value { get; set; }
  public int maxValue { get; set; }
  public bool visible { get; set; }
  public float percentValue { get { return (float)value / maxValue; } }

  public Color32 color { get; set; }

  public PlayerAttribute(string name_, int maxValue_, Color32 color_ = default, int value_ = 0, bool visible_ = true)
  {
    name = name_;
    value = value_;
    maxValue = maxValue_;
    color = color_;
    visible = visible_;
  }
}