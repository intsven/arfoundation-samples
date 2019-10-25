using UnityEngine;
using System.Runtime.InteropServices;

namespace Scripts
{
    public class IdDispatcherButton : ARButton
    {
        [SerializeField] private string _targetMethod = "onUnityButtonClicked";
        [SerializeField] private string _targetParameter = "buttonId";

#if UNITY_IOS && !UNITY_EDITOR
            [DllImport("__Internal")]
            private extern static void UnityButtonPressed(string name);
#else
        private void UnityButtonPressed(string name) { }
#endif

        void Awake()
        {
            AndroidManager.Initialize(); // FIXME: dirty, do not initialize here
        }

        protected override void HandleTouch()
        {
            Debug.Log("Button " + _targetParameter + " touched!");
            base.HandleTouch();

            if (IsOnAndroid)
            {
                AndroidManager.GetInstance().CallJavaFunc(_targetMethod, _targetParameter);
            }
            else if (IsOnIOS)
            {
                UnityButtonPressed(_targetParameter);
            }
            else
            {
                Debug.Log("Hit via Raycast");
            }       
            
        }
    }
}
