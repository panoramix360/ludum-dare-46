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
  [SerializeField] private GameObject slotPrefab;
  [SerializeField] private GameObject slotPanel;
  [SerializeField] private Image cashBar;

  private List<string> slots = new List<string>();

  private const int MAX_SLOTS = 4;

  private List<GameObject> slotBtns;

  private void Awake()
  {
    slotBtns = new List<GameObject>();
    for (int i = 0; i < MAX_SLOTS; i++)
    {
      var slot = Instantiate(slotPrefab, new Vector2(50 + i * 60, 0), Quaternion.identity);
      Button slotBtn = slot.GetComponent<Button>();
      int tempIdx = i;
      slotBtn.onClick.AddListener(() => OnClickSlot(tempIdx));
      slotBtns.Add(slot);

      slot.transform.SetParent(slotPanel.transform, false);
    }
    UpdateAttrs();
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

  private void UpdateAttrs()
  {
    var cashAttr = GameController.Instance.attributes[Constants.AttributeCash] as PlayerAttribute;
    cashBar.fillAmount = (float)cashAttr.value / cashAttr.maxValue;
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

    UpdateAttrs();
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