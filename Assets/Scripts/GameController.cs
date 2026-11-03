using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private static GameController instance;
    public static GameController Instance => instance;

    [SerializeField] private TextMeshProUGUI _chestText;
    private int _chestsOpened = 0;
    private const int _totalChests = 3;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnChestOpened()
    {
        _chestText.text = $"{++_chestsOpened} / {_totalChests}";
    }
}