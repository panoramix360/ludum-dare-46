public class PlayerAttribute
{
  public string name { get; set; }
  public int value { get; set; }
  public int maxValue { get; set; }
  public bool visible { get; set; }

  public PlayerAttribute(string name_, int maxValue_, int value_ = 0, bool visible_ = true)
  {
    name = name_;
    value = value_;
    maxValue = maxValue_;
    visible = visible_;
  }
}