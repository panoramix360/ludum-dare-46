using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class BedroomController : SingletonDestroyable<BedroomController>
{
  [SerializeField] private Button goBtn;
  [SerializeField] private Button calendarBtn;
  [SerializeField] private Button slotBtn0;
  [SerializeField] private Button slotBtn1;
  [SerializeField] private Button slotBtn2;
  [SerializeField] private Button slotBtn3;
  private List<string> slots = new List<string>();

  private const int MAX_SLOTS = 4;

  private List<Button> slotBtns;

  private void Awake()
  {
    slotBtns = new List<Button> { slotBtn0, slotBtn1, slotBtn2, slotBtn3 };
  }

  private void UpdateSlots()
  {
    for (int i = 0; i < slots.Count; i++)
    {
      slotBtns[i].GetComponentInChildren<Text>().text = slots[i];
    }

    for (int i = slots.Count; i < MAX_SLOTS; i++)
    {
      slotBtns[i].GetComponentInChildren<Text>().text = "";
    }
  }


  public void OnClickGoBtn()
  {
    if (slots.Count() < MAX_SLOTS)
    {
      // TODO: indicate that we can't go unless all slots are filled.
      return;
    }

    GameController.Instance.NextDay();
    calendarBtn.GetComponentInChildren<Text>().text = GameController.Instance.currentDay.ToString();
    slots.Clear();
    UpdateSlots();

    // TODO: go to the next scene
  }

  public void OnClickPlanningAction(string actionName)
  {
    if (slots.Count < MAX_SLOTS)
    {
      slotBtns[slots.Count].GetComponentInChildren<Text>().text = actionName;
      slots.Add(actionName);
    }
  }

  public void OnClickSlot(int idx)
  {
    // empty slots don't do anything
    if (idx < slots.Count)
    {
      slots.RemoveAt(idx);
    }

    UpdateSlots();
  }
}