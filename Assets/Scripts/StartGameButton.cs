using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameButton : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("CombatScene");
    }

    public void GoToLevelScene()
    {
        Debug.Log("GoToLevelScene");
        SceneManager.LoadScene("LevelScene");
    }
}
