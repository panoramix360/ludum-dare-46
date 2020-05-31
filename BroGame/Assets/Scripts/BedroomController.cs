using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class BedroomController : SingletonDestroyable<BedroomController>
{
  [SerializeField] private Button goBtn;
  [SerializeField] private TextMeshProUGUI calendarTxt;
  [SerializeField] private GameObject slotPrefab;
  [SerializeField] private GameObject statsPrefab;
  [SerializeField] private GameObject slotPanel;
  [SerializeField] private GameObject statsPanel;
  [SerializeField] private Image brometer;
  [SerializeField] private Image playerImg;
  [SerializeField] private Button eatBtn;
  [SerializeField] private Button punchBtn;
  [SerializeField] private Button poseBtn;

  private List<string> slots = new List<string>();

  private List<GameObject> slotBtns = new List<GameObject>();

  private List<GameObject> statBars = new List<GameObject>();

  private GameController GC { get { return GameController.Instance; } }

  private void Awake()
  {
    Debug.Log($"awake {GC.awake}");
    // setup slots
    for (int i = 0; i < GameController.MAX_SLOTS; i++)
    {
      var slot = Instantiate(slotPrefab, new Vector2(i * 90, 0), Quaternion.identity);
      slotBtns.Add(slot);

      var slotBtn = slot.GetComponent<Button>();
      int tempIdx = i;
      slotBtn.onClick.AddListener(() => OnClickSlot(tempIdx));

      slot.transform.SetParent(slotPanel.transform, false);
      slotBtn.gameObject.SetActive(i < GC.available_slots);
    }

    // enable/disable buttons
    UpdateActionBtns();

    // evolve char if needed
    UpdateCharImg();

    var daysLeft = GameController.MAX_DAYS - (GC.currentDay - 1);
    calendarTxt.text = $"{GC.currentDay} / {daysLeft}";
    UpdateSlots();

    UpdateBrometer();
  }

  private void UpdateSlots()
  {
    for (int i = 0; i < slots.Count; i++)
    {
      slotBtns[i].GetComponentInChildren<Text>().text = slots[i];
    }

    for (int i = slots.Count; i < GameController.MAX_SLOTS; i++)
    {
      slotBtns[i].GetComponentInChildren<Text>().text = "";
    }

    for (int i = 0; i < GameController.MAX_SLOTS; i++)
    {
      slotBtns[i].gameObject.SetActive(i < GC.available_slots);
    }
  }

  private void UpdateActionBtns()
  {
    Button[] btns = { eatBtn, poseBtn, punchBtn };
    bool enabled = slots.Count() < GC.available_slots;
    foreach (var btn in btns)
    {
      btn.interactable = enabled;
    }
    return;
  }

  public void UpdateCharImg()
  {
    if (GC.currentPlayerLevel == 0)
    {
      // workaround
      return;
    }
    Sprite playerSrcImg = Resources.Load<Sprite>($"{GameController.character}_0{GC.currentPlayerLevel}");
    playerImg.sprite = playerSrcImg;
  }

  public void UpdateBrometer()
  {
    var children = brometer.GetComponentsInChildren<Image>();
    var face = children[1] as Image;
    var bar = children[2] as Image;
    bar.fillAmount = GC.brometer.percentValue;

    face.sprite = Resources.Load<Sprite>($"brometer_{GC.brometerLvl}");
  }

  public void OnClickGoBtn()
  {
    if (slots.Count() < GC.available_slots)
    {
      // TODO: indicate that we can't go unless all slots are filled.
      return;
    }

    GC.ExecuteSchedule(slots);

  }

  public void OnClickPlanningAction(string actionName)
  {
    if (slots.Count == GameController.MAX_SLOTS)
    {
      // ignore when the slots are full
      return;
    }

    slotBtns[slots.Count].GetComponentInChildren<Text>().text = actionName;
    slots.Add(actionName);

    UpdateActionBtns();
  }

  public void OnClickSlot(int idx)
  {
    // empty slots don't do anything
    if (idx < slots.Count)
    {
      slots.RemoveAt(idx);
    }

    UpdateSlots();
    UpdateActionBtns();
  }
}