using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI redPoints;
    [SerializeField] private TextMeshProUGUI bluePoints;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public void SetPoints(int value,TeamTag teamTag)
    {
        switch(teamTag)
        {
            case TeamTag.Blue:
                bluePoints.text = value.ToString(); 
                break;
            case TeamTag.Red:
                redPoints.text = value.ToString(); 
                break;
        }
    }
}