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

  private string[] weekdays = { "7 days", "6 days", "5 days", "4 days", "3 days", "2 days", "1 day" };

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

    // setup stats
    var attributes = GC.GetVisibleAttributes();
    for (int i = 0; i < attributes.Count; i++)
    {
      var attr = attributes[i];
      var statBar = Instantiate(statsPrefab);

      var statBarImgs = statBar.GetComponentsInChildren<Image>();

      if (attr.iconName != null)
      {
        statBarImgs[0].sprite = Resources.Load<Sprite>(attr.iconName);
      }

      statBarImgs[1].color = attr.color;
      statBars.Add(statBar);

      statBar.transform.SetParent(statsPanel.transform, false);
    }

    // update stat bars
    UpdateAttrs();

    // enable/disable buttons
    UpdateActionBtns();

    // evolve char if needed
    UpdateCharImg();

    calendarTxt.text = weekdays[(GC.currentDay - 1) % 7];
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

  private void UpdateAttrs()
  {
    List<PlayerAttribute> attributes = GC.GetVisibleAttributes();
    for (int i = 0; i < attributes.Count; i++)
    {
      var attr = attributes[i];
      var img = statBars[i].GetComponentsInChildren<Image>()[1];
      img.fillAmount = attr.percentValue;
      statBars[i].GetComponentInChildren<TextMeshProUGUI>().text = (attr.value < 10 ? "0" : "") + attr.value.ToString();
    }
  }

  private void UpdateActionBtns()
  {
    Button[] btns = { eatBtn, poseBtn, punchBtn };
    PlayerAttrReward[][] rewards = { GC.rewards[Constants.EatActivity], GC.rewards[Constants.PoseActivity],
                    GC.rewards[Constants.PunchActivity] };
    var attrs = GC.attributes;
    var simAttrs = GC.attrs.ToDictionary(attr => attr.name, attr => attr.value);

    Debug.Log("-------");

    // apply rewards to simulation
    for (int i = 0; i < slots.Count; i++)
    {
      string slotName = slots[i];
      var pars = GC.rewards[slotName];
      foreach (var par in pars)
      {
        simAttrs[par.attr] += par.reward;
        Debug.Log($"0 {par.attr} {simAttrs[par.attr]}");
      }
    }

    for (int i = 0; i < btns.Count(); i++)
    {
      // for each button, check if applying its next reward would mean disabling it
      Debug.Log($"i = {i}");
      foreach (var par in rewards[i])
      {
        simAttrs[par.attr] += par.reward;
        Debug.Log($"1 {par.attr} {simAttrs[par.attr]}");
      }

      btns[i].interactable = true;
      foreach (var simAttr in simAttrs.Values)
      {
        if (simAttr < 0)
        {
          btns[i].interactable = false;
        }
      }

      foreach (var par in rewards[i])
      {
        simAttrs[par.attr] -= par.reward;
      }
    }
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