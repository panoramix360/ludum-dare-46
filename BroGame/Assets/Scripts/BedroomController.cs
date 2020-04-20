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
  [SerializeField] private GameObject statsPrefab;
  [SerializeField] private GameObject slotPanel;
  [SerializeField] private GameObject statsPanel;
  [SerializeField] private GameObject brometer;
  [SerializeField] private Image playerImg;

  private List<string> slots = new List<string>();

  private const int MAX_SLOTS = 4;

  private List<GameObject> slotBtns = new List<GameObject>();
  private List<GameObject> statBars = new List<GameObject>();

  private GameController GC { get { return GameController.Instance; } }

  private void Awake()
  {
    // setup slots
    for (int i = 0; i < MAX_SLOTS; i++)
    {
      var slot = Instantiate(slotPrefab, new Vector2(i * 90, 0), Quaternion.identity);
      slotBtns.Add(slot);

      var slotBtn = slot.GetComponent<Button>();
      int tempIdx = i;
      slotBtn.onClick.AddListener(() => OnClickSlot(tempIdx));

      slot.transform.SetParent(slotPanel.transform, false);
    }

    // setup stats
    var attributes = GC.GetVisibleAttributes();
    for (int i = 0; i < attributes.Count; i++)
    {
      var attr = attributes[i];
      var statBar = Instantiate(statsPrefab);
      statBar.GetComponent<Image>().color = attr.color;
      statBars.Add(statBar);

      statBar.transform.SetParent(statsPanel.transform, false);
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
    List<PlayerAttribute> attributes = GC.GetVisibleAttributes();
    for (int i = 0; i < attributes.Count; i++)
    {
      var attr = attributes[i];
      statBars[i].GetComponent<Image>().fillAmount = (float)attr.value / attr.maxValue;
    }
  }

  public void UpdateCharImg()
  {
    Sprite playerSrcImg = Resources.Load<Sprite>($"{GC.character}_0{GC.currentPlayerLevel}");
    playerImg.sprite = playerSrcImg;
  }

  public void OnClickGoBtn()
  {
    if (slots.Count() < MAX_SLOTS)
    {
      // TODO: indicate that we can't go unless all slots are filled.
      return;
    }

    GC.ExecuteSchedule(slots);

    // evolve char if needed
    UpdateCharImg();

    calendarBtn.GetComponentInChildren<Text>().text = GC.currentDay.ToString();
    slots.Clear();
    UpdateSlots();

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