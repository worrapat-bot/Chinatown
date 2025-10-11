using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class GoNextSceneOnVideoEnd : MonoBehaviour
{
    [SerializeField] private VideoPlayer player;     // ลาก VideoPlayer มาวางใน Inspector
    [SerializeField] private int nextSceneIndex = -1; // ถ้า -1 จะไปซีนถัดไปใน Build

    void Awake()
    {
        if (player == null) player = GetComponent<VideoPlayer>();
        if (player == null) { Debug.LogError("No VideoPlayer assigned."); return; }

        player.isLooping = false;                            // ต้องไม่ Loop
        player.loopPointReached += OnVideoEnd;               // ยิง event ตอนจบ
    }

    void OnDestroy()
    {
        if (player != null) player.loopPointReached -= OnVideoEnd;
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        if (nextSceneIndex >= 0)
            SceneManager.LoadScene(nextSceneIndex);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
