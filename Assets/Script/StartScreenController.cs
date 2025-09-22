using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenController : MonoBehaviour
{
    public void StartGame()
    {
        // Load the Maze Selection Screen when the button is clicked
        SceneManager.LoadScene("MazeSelection");
    }
}
