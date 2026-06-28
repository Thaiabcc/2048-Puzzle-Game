using UnityEngine;

public class GameCheater : MonoBehaviour
{
    private GridManager gridManager;

    private void Start()
    {
        gridManager = FindFirstObjectByType<GridManager>();
    }

    private void Update()
    {
        if (gridManager == null) return;
        if (Input.GetKeyDown(KeyCode.W))
        {
            gridManager.KeepPlaying();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            gridManager.RestartGame();
        }
    }
}