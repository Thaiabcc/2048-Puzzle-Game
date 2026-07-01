using UnityEngine;
using System.Collections;

public class AutoTester : MonoBehaviour
{
    [SerializeField] private GridController controller;

    [Header("Auto Settings")]
    [SerializeField] private float actionDelay = 0.05f;

    [SerializeField] private bool autoStart = true;

    [Header("Testing")]
    [SerializeField] private int movesBeforeWin = 300;

    private int moveCount;
    private int patternIndex;

    // Left Down Left Down Left Down Right Up
    private readonly int[] movePattern =
    {
        0,
        1,
        0,
        1,
        0,
        1,
        2,
        3
    };

    private void Start()
    {
        if (controller == null)
            controller = FindObjectOfType<GridController>();

        if (autoStart)
            StartCoroutine(TestLoop());
    }

    IEnumerator TestLoop()
    {
        yield return new WaitForSeconds(1f);

        controller.PlayGameFromMenu();

        moveCount = 0;
        patternIndex = 0;

        while (true)
        {
            yield return new WaitForSeconds(actionDelay);

            int move = movePattern[patternIndex];

            patternIndex++;

            if (patternIndex >= movePattern.Length)
                patternIndex = 0;

            yield return StartCoroutine(CallMove(move));

            moveCount++;

            // ép thắng sau N lượt
            if (moveCount >= movesBeforeWin)
            {
                Debug.Log($"FORCE WIN ({moveCount} moves)");

                controller.ForceWin();

                yield return new WaitForSeconds(0.5f);
            }

            if (IsWinPanelOpen())
            {
                Debug.Log("WIN");

                yield return new WaitForSeconds(0.5f);

                controller.KeepPlaying();

                moveCount = 0;
                patternIndex = 0;

                yield return new WaitForSeconds(0.5f);
            }

            if (IsGameOverPanelOpen())
            {
                Debug.Log("GAME OVER");

                yield return new WaitForSeconds(0.5f);

                controller.RestartGame();

                moveCount = 0;
                patternIndex = 0;

                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    IEnumerator CallMove(int rot)
    {
        var method = typeof(GridController)
            .GetMethod(
                "MoveSequence",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

        IEnumerator move =
            (IEnumerator)method.Invoke(
                controller,
                new object[] { rot });

        yield return StartCoroutine(move);
    }

    bool IsWinPanelOpen()
    {
        GameObject panel = GameObject.Find("GameWinPanel");
        return panel != null && panel.activeSelf;
    }

    bool IsGameOverPanelOpen()
    {
        GameObject panel = GameObject.Find("GameOverPanel");
        return panel != null && panel.activeSelf;
    }
}