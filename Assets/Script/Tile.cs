using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Tile : MonoBehaviour
{
    [SerializeField] private TMP_Text numberText;
    [SerializeField] private Image backgroundImage;

    [Header("Visual Configuration")]
    [SerializeField] private Color[] tileColors;

    private int currentValue = 0;
    private Tweener scaleTween;

    private void Awake()
    {
        InitializeColors();
    }

    private void InitializeColors()
    {
        if (tileColors == null || tileColors.Length < 13)
        {
            tileColors = new Color[13]
            {
                new Color(0.80f, 0.80f, 0.80f), 
                new Color(0.93f, 0.89f, 0.82f), 
                new Color(0.93f, 0.88f, 0.70f), 
                new Color(0.95f, 0.69f, 0.47f), 
                new Color(0.96f, 0.58f, 0.39f), 
                new Color(0.96f, 0.47f, 0.37f), 
                new Color(0.93f, 0.34f, 0.27f), 
                new Color(0.89f, 0.78f, 0.55f), 
                new Color(0.89f, 0.75f, 0.42f), 
                new Color(0.89f, 0.71f, 0.27f), 
                new Color(0.93f, 0.58f, 0.20f), 
                new Color(0.93f, 0.51f, 0.15f), 
                new Color(0.60f, 0.40f, 0.80f)  
            };
        }
    }

    public void SetValue(int value, bool isNewTile = false, bool isMerge = false)
    {
        bool wasEmpty = currentValue == 0;
        currentValue = value;

        if (numberText != null)
        {
            numberText.text = value == 0 ? "" : value.ToString();
            numberText.color = value >= 8 ? Color.white : new Color(0.3f, 0.3f, 0.3f);
        }

        if (backgroundImage != null && tileColors != null && tileColors.Length > 0)
        {
            int colorIndex = value == 0 ? 0 : Mathf.Min((int)Mathf.Log(value, 2), tileColors.Length - 1);
            backgroundImage.color = tileColors[colorIndex];
        }
        if (isNewTile && value != 0)
            PlaySpawnAnimation();
        else if (isMerge)
            PlayMergeAnimation();
        else if (wasEmpty && value != 0)
            PlaySpawnAnimation();
    }

    private void PlaySpawnAnimation()
    {
        KillTween();
        
        transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
        
        scaleTween = transform.DOScale(1f, 0.15f).SetEase(Ease.OutBack);
    }

    private void PlayMergeAnimation()
    {
        KillTween();
        transform.localScale = Vector3.one;
        scaleTween = transform.DOScale(1.25f, 0.1f)
            .OnComplete(() => transform.DOScale(1f, 0.15f));
    }

    private void KillTween()
    {
        if (scaleTween != null && scaleTween.IsActive())
            scaleTween.Kill();
    }

    private void OnDestroy() => KillTween();
}