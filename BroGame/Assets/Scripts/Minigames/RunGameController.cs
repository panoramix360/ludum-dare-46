using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunGameController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject broCoachDirectionsContainer;
    [SerializeField] private GameObject playerCommandsContainer;

    [SerializeField] private Sprite arrowUp;
    [SerializeField] private Sprite arrowDown;
    [SerializeField] private Sprite arrowLeft;
    [SerializeField] private Sprite arrowRight;

    [SerializeField] private Direction directionPrefab;

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
                    CreateDirectionUI(arrowUp, playerCommandsContainer.transform);
                    lastDirectionPressed = DirectionEnum.UP;
                }

                if (downArrowPressed)
                {
                    CreateDirectionUI(arrowDown, playerCommandsContainer.transform);
                    lastDirectionPressed = DirectionEnum.DOWN;
                }

                if (leftArrowPressed)
                {
                    CreateDirectionUI(arrowLeft, playerCommandsContainer.transform);
                    lastDirectionPressed = DirectionEnum.LEFT;
                }

                if (rightArrowPressed)
                {
                    CreateDirectionUI(arrowRight, playerCommandsContainer.transform);
                    lastDirectionPressed = DirectionEnum.RIGHT;
                }

                directionsPressedCount++;

                if (CheckIfKeyPressedIsWrong() || CheckIfContestEnded())
                {
                    isContestRunning = false;
                    NextContest();
                }
            }
        }
    }

    private IEnumerator GeneratePoseContest()
    {
        yield return new WaitForSeconds(2f);

        RemoveAllChildren(broCoachDirectionsContainer);
        RemoveAllChildren(playerCommandsContainer);

        GenerateContestDirections();

        isContestRunning = true;
    }

    private void GenerateContestDirections()
    {
        runningContestDirections.Clear();
        Contest runningContest = contests[currentContest];

        for(int i = 0; i < runningContest.difficulty; i++)
        {
            int randomDirection = Random.Range(0, possibleDirections.Length);
            runningContestDirections.Add(possibleDirections[randomDirection]);
        }

        ShowContestDirections(runningContestDirections);
    }

    private void ShowContestDirections(List<DirectionEnum> directionsToShow)
    {
        foreach(DirectionEnum direction in directionsToShow)
        {
            switch(direction)
            {
                case DirectionEnum.LEFT:
                    CreateDirectionUI(arrowLeft, broCoachDirectionsContainer.transform);
                    break;
                case DirectionEnum.RIGHT:
                    CreateDirectionUI(arrowRight, broCoachDirectionsContainer.transform);
                    break;
                case DirectionEnum.UP:
                    CreateDirectionUI(arrowUp, broCoachDirectionsContainer.transform);
                    break;
                case DirectionEnum.DOWN:
                    CreateDirectionUI(arrowDown, broCoachDirectionsContainer.transform);
                    break;
            }
        }
    }

    private void CreateDirectionUI(Sprite sprite, Transform parent)
    {
        Direction directionObj = Instantiate(directionPrefab, parent);
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
        return runningContestDirections.Count == directionsPressedCount;
    }

    private void NextContest()
    {
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
