using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

namespace Scripts
{
    public class SceneSwitch : MonoBehaviour
    {
        [SerializeField] private bool _doCheckOnStart;

        // ---
#if UNITY_IOS && !UNITY_EDITOR
            [DllImport("__Internal")]
            private extern static int UnityRequestScene();
#else
        private int UnityRequestScene() { return 0; }
#endif

        protected bool IsOnAndroid { get { return Application.platform == RuntimePlatform.Android; } }
        protected bool IsOnIOS { get { return Application.platform == RuntimePlatform.IPhonePlayer; } }
        protected bool IsOnMobile { get { return IsOnAndroid || IsOnIOS; } }
        // ---

        void Awake()
        {
            AndroidManager.Initialize();
        }

        void Start()
        {
            if (_doCheckOnStart)
                HandleSceneSwitch();
        }

        void OnApplicationPause(bool isPaused)
        {
            if (!isPaused)
                HandleSceneSwitch();
        }

        private void HandleSceneSwitch()
        {
            Debug.Log("Handle Scene Switch.");

            if (IsOnAndroid)
            {
                var sceneName = AndroidManager.GetInstance().RetrieveTargetSceneName();
                if (SceneManager.GetActiveScene().name == sceneName) return;

                if (!string.IsNullOrEmpty(sceneName))
                    SceneManager.LoadScene(sceneName);
                else
                {
                    Debug.LogError("Error: Could not retrieve target scene name from Android App!");
                    SceneManager.LoadScene("MainScene");
                }
            }
            else if (IsOnIOS)
            {
                int scene = UnityRequestScene();

                Debug.Log("Scene: " + scene);

                if (scene == 0) SceneManager.LoadScene("MainScene");
                else if (scene == 1) SceneManager.LoadScene("3DMotorScene");
                else if (scene == 2) SceneManager.LoadScene("TurntableScene");
                else SceneManager.LoadScene("MainScene");
            }
            else
            {
                Debug.Log("Neither Android, nor iOS detected.");
            }
        }
    }
}
