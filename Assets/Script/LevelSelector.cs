using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    public Button level1Button;
    public Button level2Button;
    public Button level3Button;

    void Start()
    {
        // Check saved progress and unlock levels
        level2Button.interactable = PlayerPrefs.GetInt("Level2Unlocked", 0) == 1;
        level3Button.interactable = PlayerPrefs.GetInt("Level3Unlocked", 0) == 1;
    }

    public void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }

    public void ResetProgress()
    {
        PlayerPrefs.DeleteAll(); // Clears all saved data
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
