using System;

public class Quest
{
  public string label;
  public string activity;

  public bool done = false;

  public Func<ActivityTracker, bool> completionFunc;

  public Quest(string activity, string label, Func<ActivityTracker, bool> completionFunc)
  {
    this.activity = activity;
    this.label = label;
    this.completionFunc = completionFunc;
  }
}