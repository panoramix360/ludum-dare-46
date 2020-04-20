using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoxeGameController : MonoBehaviour
{
  [SerializeField] private TextMeshProUGUI broCoachTxt;
  [SerializeField] private TextMeshProUGUI comboTxt;
  [SerializeField] private GameObject boxeBag;
  [SerializeField] private Collider2D rightArmCollider;
  [SerializeField] private Collider2D leftArmCollider;
  [SerializeField] private Animator rightArmAnim;
  [SerializeField] private Animator leftArmAnim;

  [SerializeField] private Animator leftKeyAnimator;
  [SerializeField] private Animator rightKeyAnimator;

  [SerializeField] private int startLimitToChangeOption = 10;
  [SerializeField] private int endLimitToChangeOption = 30;
  [SerializeField] private AudioSource grunt1;
  [SerializeField] private AudioSource grunt2;
  [SerializeField] private AudioSource grunt3;

  [Header("Don't change this fields!")]
  [SerializeField] private float countdown = 60;

  [SerializeField] private int highestHitCombo;
  [SerializeField] private int hitCombo;
  [SerializeField] private State lastStatePressed;
  [SerializeField] private float hitTimer;
  [SerializeField] private float hitWindow = 1f;

  [SerializeField] private int broRandomOrder;
  [SerializeField] private State currentBroOrder;

  private string[] broCoachOptions =  {
        "LEFT BRO!",
        "LEFT AND RIGHT BRO!",
        "RIGHT BRO!"
    };

  private void Awake()
  {
    Random.InitState(System.DateTime.Now.Millisecond);
  }

  private void Start()
  {
    broRandomOrder = Random.Range(0, broCoachOptions.Length);

    SetBroOrder(broRandomOrder);

    StartCoroutine(StartCoaching());
  }

  private void Update()
  {
    if (countdown > 0)
    {
      countdown -= Time.deltaTime;
    }

    hitTimer += Time.deltaTime;
    if (hitTimer >= hitWindow)
    {
      hitCombo = 0;
      hitTimer = 0;
      comboTxt.text = "";

      int randomNum = Random.Range(0, 3);
      switch(randomNum)
      {
          case 0:
              grunt1.Play();
              break;
          case 1:
              grunt2.Play();
              break;
          case 2:
              grunt3.Play();
              break;
      }
    }

    bool leftKeyPressed = Input.GetKeyDown(KeyCode.LeftArrow);
    bool rightKeyPressed = Input.GetKeyDown(KeyCode.RightArrow);

    if (GameController.DEBUG && Input.GetKeyDown(KeyCode.D))
    {
      countdown = 3;
    }

    if (leftKeyPressed || rightKeyPressed)
    {
      if (leftKeyPressed)
      {
        leftArmAnim.SetBool("isMoving", true);

        if (lastStatePressed == State.RIGHT)
        {
          lastStatePressed = State.LEFT_RIGHT;
        }
        else
        {
          lastStatePressed = State.LEFT;
        }
      }

      if (rightKeyPressed)
      {
        rightArmAnim.SetBool("isMoving", true);

        if (lastStatePressed == State.LEFT)
        {
          lastStatePressed = State.LEFT_RIGHT;
        }
        else
        {
          lastStatePressed = State.RIGHT;
        }
      }
    }
    else
    {
      rightArmAnim.SetBool("isMoving", false);
      leftArmAnim.SetBool("isMoving", false);
    }
  }

  private IEnumerator StartCoaching()
  {
    yield return new WaitForSeconds(1f);

    while (countdown > 0)
    {
      UpdateBroCoachText(broCoachOptions[broRandomOrder]);

      yield return new WaitForSeconds(10f);

      GenerateNextBroOrder();
    }

    Debug.Log("Minigame Finish!");
    GameController.Instance.UpdateMinigameScore(Constants.PunchActivity, highestHitCombo);
    GameController.Instance.NextActivity();
  }

  private void SetBroOrder(int broOption)
  {
    switch (broOption)
    {
      case 0:
        currentBroOrder = State.LEFT;
        break;
      case 1:
        currentBroOrder = State.LEFT_RIGHT;
        break;
      case 2:
        currentBroOrder = State.RIGHT;
        break;
      default:
        break;
    }
  }

  private void GenerateNextBroOrder()
  {
    int lastCoachOption = broRandomOrder;
    do
    {
      broRandomOrder = Random.Range(0, broCoachOptions.Length);
    } while (lastCoachOption == broRandomOrder);

    SetBroOrder(broRandomOrder);
  }

  public void OnHitBoxe()
  {
    if (currentBroOrder == lastStatePressed && hitTimer < hitWindow)
    {
      hitCombo++;
      hitTimer = 0;
      comboTxt.text = hitCombo + "x";
      Debug.Log(hitCombo);

      if (hitCombo > highestHitCombo)
      {
        highestHitCombo = hitCombo;
      }
    }
  }

  private void UpdateBroCoachText(string text)
  {
    broCoachTxt.gameObject.SetActive(true);
    broCoachTxt.text = text;
  }

  private enum State
  {
    LEFT, RIGHT, LEFT_RIGHT
  }
}