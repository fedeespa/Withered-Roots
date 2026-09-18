using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameButton : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("LevelScene");
    }


    public void RestartCombat()
    {
        SceneManager.LoadScene("CombatScene");
    }
}
