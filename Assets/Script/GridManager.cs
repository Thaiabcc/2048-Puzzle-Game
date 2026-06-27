using UnityEngine;

public class GridManager : MonoBehaviour
{
    public Tile[] allTiles; 
    private int[,] grid = new int[4, 4];

    void Start()
    {
        // Thử spawn 2 số đầu tiên
        grid[0, 0] = 2;
        grid[1, 2] = 4;
        UpdateUI(); // Cập nhật màn hình lần đầu
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) Move(0, -1); // Ví dụ đơn giản
        // Ông bổ sung các hướng khác ở đây...
    }

    void Move(int dx, int dy)
    {
        // 1. Duyệt mảng, thực hiện dịch chuyển các số
        // 2. Sau khi mảng thay đổi:
        UpdateUI(); 
    }

    void UpdateUI()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                int index = i * 4 + j;
                allTiles[index].SetValue(grid[i, j]);
            }
        }
    }
}