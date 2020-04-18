using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class BedroomController : SingletonDestroyable<BedroomController>
{
  [SerializeField] private Button goBtn;
  [SerializeField] private Button calendarBtn;

  public void OnClickGoBtn()
  {
    GameController.Instance.NextDay();
    calendarBtn.GetComponentInChildren<Text>().text = GameController.Instance.currentDay.ToString();
  }
}