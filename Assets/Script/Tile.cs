using UnityEngine;
using UnityEngine.UI;
using TMPro; // Bắt buộc phải có dòng này

public class Tile : MonoBehaviour
{
    // Đổi từ Text sang TMP_Text
    public TMP_Text numberText; 
    public Image backgroundImage;

    public void SetValue(int value)
    {
        // Hiển thị text
        numberText.text = value == 0 ? "" : value.ToString();
        
        // Đổi màu nền (ví dụ)
        backgroundImage.color = (value == 0) ? new Color(0.7f, 0.7f, 0.7f) : new Color(0.9f, 0.7f, 0.5f);
    }
}