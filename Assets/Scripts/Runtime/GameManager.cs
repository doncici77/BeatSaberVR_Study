using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameManager
{
    public static GameSession gameSession { get; private set; }

    [RuntimeInitializeOnLoadMethod]
    public static void Bootstrap()
    {
        SceneManager.LoadScene("MetaGame");
        gameSession = ScriptableObject.CreateInstance<GameSession>();
    }
}