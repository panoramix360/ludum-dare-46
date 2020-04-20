using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunGameController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject broCoachDirectionsContainer;
    [SerializeField] private GameObject playerCommandsContainer;
    [SerializeField] private Countdown countdown;

    [SerializeField] private Sprite arrowUp;
    [SerializeField] private Sprite arrowDown;
    [SerializeField] private Sprite arrowLeft;
    [SerializeField] private Sprite arrowRight;

    [SerializeField] private Direction directionPrefab;
    [SerializeField] private Direction directionLargePrefab;

    [SerializeField] private GameObject posePlayer;

    [Header("Contest details")]
    [SerializeField] private List<Contest> contests;

    [Header("Dont change this fields!")]
    [SerializeField] private int currentContest;
    [SerializeField] private List<DirectionEnum> runningContestDirections;
    [SerializeField] private int directionsPressedCount;
    [SerializeField] private DirectionEnum lastDirectionPressed;

    [SerializeField] private bool isContestRunning = false;

    private DirectionEnum[] possibleDirections =
    {
        DirectionEnum.LEFT,
        DirectionEnum.RIGHT,
        DirectionEnum.UP,
        DirectionEnum.DOWN
    };

    private void Awake()
    {
        Random.InitState(System.DateTime.Now.Millisecond);

        runningContestDirections = new List<DirectionEnum>();
    }

    private void Start()
    {
        StartCoroutine(GeneratePoseContest());
    }

    private void Update()
    {
        bool upArrowPressed = Input.GetKeyDown(KeyCode.UpArrow);
        bool downArrowPressed = Input.GetKeyDown(KeyCode.DownArrow);
        bool leftArrowPressed = Input.GetKeyDown(KeyCode.LeftArrow);
        bool rightArrowPressed = Input.GetKeyDown(KeyCode.RightArrow);

        if (isContestRunning)
        {
            if (upArrowPressed || downArrowPressed || leftArrowPressed || rightArrowPressed)
            {
                if (upArrowPressed)
                {
                    CreateDirectionUI(directionLargePrefab, arrowUp, playerCommandsContainer.transform);
                    lastDirectionPressed = DirectionEnum.UP;
                }

                if (downArrowPressed)
                {
                    CreateDirectionUI(directionLargePrefab, arrowDown, playerCommandsContainer.transform);
                    lastDirectionPressed = DirectionEnum.DOWN;
                }

                if (leftArrowPressed)
                {
                    CreateDirectionUI(directionLargePrefab, arrowLeft, playerCommandsContainer.transform);
                    lastDirectionPressed = DirectionEnum.LEFT;
                }

                if (rightArrowPressed)
                {
                    CreateDirectionUI(directionLargePrefab, arrowRight, playerCommandsContainer.transform);
                    lastDirectionPressed = DirectionEnum.RIGHT;
                }

                directionsPressedCount++;

                if (CheckIfKeyPressedIsWrong() || CheckIfContestEnded())
                {
                    NextContest();
                }
            }
        }
    }

    private IEnumerator GeneratePoseContest()
    {
        yield return new WaitForSeconds(3f);

        RemoveAllChildren(broCoachDirectionsContainer);
        RemoveAllChildren(playerCommandsContainer);

        GenerateContestDirections();

        yield return new WaitForSeconds(1f);

        ShowContestDirections(runningContestDirections);

        yield return new WaitForSeconds(5f);

        HideContestDirections();

        BeginCountdown();

        isContestRunning = true;
    }

    private void GenerateContestDirections()
    {
        runningContestDirections.Clear();
        Contest runningContest = contests[currentContest];

        for (int i = 0; i < runningContest.difficulty; i++)
        {
            int randomDirection = Random.Range(0, possibleDirections.Length);
            runningContestDirections.Add(possibleDirections[randomDirection]);
        }
    }

    private void ShowContestDirections(List<DirectionEnum> directionsToShow)
    {
        broCoachDirectionsContainer.gameObject.SetActive(true);

        foreach (DirectionEnum direction in directionsToShow)
        {
            switch(direction)
            {
                case DirectionEnum.LEFT:
                    CreateDirectionUI(directionPrefab, arrowLeft, broCoachDirectionsContainer.transform);
                    break;
                case DirectionEnum.RIGHT:
                    CreateDirectionUI(directionPrefab, arrowRight, broCoachDirectionsContainer.transform);
                    break;
                case DirectionEnum.UP:
                    CreateDirectionUI(directionPrefab, arrowUp, broCoachDirectionsContainer.transform);
                    break;
                case DirectionEnum.DOWN:
                    CreateDirectionUI(directionPrefab, arrowDown, broCoachDirectionsContainer.transform);
                    break;
            }
        }
    }

    private void HideContestDirections()
    {
        broCoachDirectionsContainer.gameObject.SetActive(false);
    }

    private void CreateDirectionUI(Direction prefab, Sprite sprite, Transform parent)
    {
        Direction directionObj = Instantiate(prefab, parent);
        directionObj.SetImage(sprite);
    }

    private bool CheckIfKeyPressedIsWrong()
    {
        Debug.Log("Direction WRONG!");
        return lastDirectionPressed != runningContestDirections[directionsPressedCount - 1];
    }

    private bool CheckIfContestEnded()
    {
        Debug.Log("Contest ended!");
        bool contestEnded = runningContestDirections.Count == directionsPressedCount;
        if (contestEnded)
        {
            int randomPose = Random.Range(1, 7);
            posePlayer.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"pose_0{randomPose}");
        }
        return contestEnded;
    }

    public void NextContest()
    {
        isContestRunning = false;
        countdown.gameObject.SetActive(false);

        if (currentContest == contests.Count - 1)
        {
            Debug.Log("Minigame ended");
            return;
        }

        Debug.Log("next contest!");
        currentContest++;
        directionsPressedCount = 0;

        StartCoroutine(GeneratePoseContest());
    }

    private void RemoveAllChildren(GameObject gameObject)
    {
        foreach(Transform child in gameObject.transform) {
            Destroy(child.gameObject);
        }
    }

    private void BeginCountdown()
    {
        countdown.SetCountdown(10);
        countdown.gameObject.SetActive(true);
    }

    private enum DirectionEnum
    {
        LEFT, RIGHT, UP, DOWN
    }

    [System.Serializable]
    public class Contest
    {
        [SerializeField] public string level;
        [SerializeField] public int difficulty;
    }
}
