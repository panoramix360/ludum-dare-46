using System.Collections.Generic;
static class Constants
{
  #region Activities
  public const string HomeActivity = "home";
  public const string EatActivity = "eat";
  public const string PoseActivity = "pose";
  public const string PunchActivity = "punch";
  #endregion

  #region Attributes
  public const string CashAttribute = "cash";
  public const string WillpowerAttribute = "willpower";
  public const string ManlinessAttribute = "manliness";
  public const string BrometerAttribute = "brometer";
  #endregion

  public static readonly Dictionary<string, string> ActivityToScene = new Dictionary<string, string> {
    { HomeActivity, "Bedroom" },
    { EatActivity, "MiniGameEat" },
    { PoseActivity, "MiniGamePose" },
    { PunchActivity, "MiniGameBoxe" },
  };
}