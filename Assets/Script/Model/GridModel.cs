using System;
using System.Collections.Generic;
using UnityEngine;

public class GridModel
{
    public int BoardSize { get; private set; }
    public int[,] Grid { get; private set; }
    public int CurrentScore { get; private set; }
    public int BestScore { get; private set; }

    public GridModel() { }

    public void InitBestScore()
    {
        BestScore = PlayerPrefs.GetInt("BestScore", 0);
    }

    public void Setup(int size)
    {
        BoardSize = size;
        Grid = new int[BoardSize, BoardSize];
    }

    public void ResetScore()
    {
        CurrentScore = 0;
    }

    private void UpdateScore(int points)
    {
        CurrentScore += points;
        if (CurrentScore > BestScore)
        {
            BestScore = CurrentScore;
            PlayerPrefs.SetInt("BestScore", BestScore);
        }
    }

    public bool SlideAndMerge(int[] line)
    {
        bool changed = false;
        List<int> cache = new List<int>();
        
        for (int i = 0; i < BoardSize; i++) 
            if (line[i] != 0) cache.Add(line[i]);

        for (int i = 0; i < cache.Count - 1; i++)
        {
            if (cache[i] == cache[i + 1])
            {
                cache[i] *= 2;
                UpdateScore(cache[i]);
                cache.RemoveAt(i + 1);
                changed = true;
            }
        }

        for (int i = 0; i < BoardSize; i++)
        {
            int newVal = i < cache.Count ? cache[i] : 0;
            if (newVal != line[i]) 
            { 
                line[i] = newVal; 
                changed = true; 
            }
        }
        return changed;
    }

    public void RotateGrid(int times)
    {
        int[,] temp = new int[BoardSize, BoardSize];
        for (int t = 0; t < times; t++)
        {
            for (int i = 0; i < BoardSize; i++)
                for (int j = 0; j < BoardSize; j++)
                    temp[j, (BoardSize - 1) - i] = Grid[i, j];

            Array.Copy(temp, Grid, Grid.Length);
        }
    }

    public void SpawnTile()
    {
        List<Vector2Int> empty = new List<Vector2Int>();
        for (int i = 0; i < BoardSize; i++)
            for (int j = 0; j < BoardSize; j++)
                if (Grid[i, j] == 0) empty.Add(new Vector2Int(i, j));

        if (empty.Count == 0) return;

        Vector2Int pos = empty[UnityEngine.Random.Range(0, empty.Count)];
        Grid[pos.x, pos.y] = UnityEngine.Random.value < 0.9f ? 2 : 4;
    }

    public bool CheckGameOver()
    {
        for (int i = 0; i < BoardSize; i++)
            for (int j = 0; j < BoardSize; j++)
                if (Grid[i, j] == 0) return false;

        for (int i = 0; i < BoardSize; i++)
            for (int j = 0; j < BoardSize - 1; j++)
                if (Grid[i, j] == Grid[i, j + 1]) return false;

        for (int i = 0; i < BoardSize - 1; i++)
            for (int j = 0; j < BoardSize; j++)
                if (Grid[i, j] == Grid[i + 1, j]) return false;

        return true;
    }

    public bool CheckWinCondition()
    {
        int target = 2048 << (BoardSize - 4);

        for (int i = 0; i < BoardSize; i++)
        for (int j = 0; j < BoardSize; j++)
            if (Grid[i, j] >= target)
                return true;

        return false;
    }
}