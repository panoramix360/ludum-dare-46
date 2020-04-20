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
  [SerializeField] private Button eatBtn;
  [SerializeField] private Button punchBtn;
  [SerializeField] private Button poseBtn;

  private List<string> slots = new List<string>();

  private List<GameObject> slotBtns = new List<GameObject>();

  private List<GameObject> statBars = new List<GameObject>();

  private GameController GC { get { return GameController.Instance; } }

  private void Awake()
  {
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
      statBar.GetComponent<Image>().color = attr.color;
      statBars.Add(statBar);

      statBar.transform.SetParent(statsPanel.transform, false);
    }

    UpdateAttrs();
    UpdateActionBtns();
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
      var img = statBars[i].GetComponent<Image>();
      img.fillAmount = (float)attr.value / attr.maxValue;
      img.GetComponentInChildren<Text>().text = (attr.value < 10 ? "0" : "") + attr.value.ToString();
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
    Sprite playerSrcImg = Resources.Load<Sprite>($"{GameController.character}_0{GC.currentPlayerLevel}");
    playerImg.sprite = playerSrcImg;
  }

  public void OnClickGoBtn()
  {
    if (slots.Count() < GC.available_slots)
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