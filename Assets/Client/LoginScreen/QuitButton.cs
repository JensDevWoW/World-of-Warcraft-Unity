using UnityEngine;

public class QuitButton : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();
        // For testing in the editor, this line will work as a substitute.
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
