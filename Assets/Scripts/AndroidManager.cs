using System;
using TMPro;
using UnityEngine;

namespace Scripts
{
    public class AndroidManager : MonoBehaviour
    {
        private AndroidJavaObject _currentActivity;
        static AndroidManager _instance;

        public static AndroidManager GetInstance()
        {
            Initialize();
            return _instance;
        }

        public static void Initialize()
        {
            if (_instance == null)
                _instance = new GameObject("AndroidManager").AddComponent<AndroidManager>();
        }

        void Awake()
        {

#if UNITY_ANDROID
             var javaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            _currentActivity = javaClass.GetStatic<AndroidJavaObject>("currentActivity");
#endif
        }

        public void CallJavaFunc(string strFuncName, string strTemp)
        {
            if (_currentActivity == null)
            {
                SetErrorText("Error: Current activity ( " + _currentActivity + " ) is null!");
                return;
            }
            _currentActivity.Call(strFuncName, strTemp);
        }

        public string RetrieveTargetSceneName()
        {
            return _currentActivity.Call<string>("getUnityScene");
        }

        public void HandleCallFromNative(string value)
        {
            Debug.Log("String received from native: " + value);
            SetOutputText("String received from native: " + value);
        }

        private void SetOutputText(string text)
        {
            var textObject = GameObject.Find("TextOutput").GetComponent<TMP_Text>();
            textObject.enabled = true;
            textObject.text = text;
        }

        private void SetErrorText(string text)
        {
            var textObject = GameObject.Find("ErrorOutput").GetComponent<TMP_Text>();
            textObject.enabled = true;
            textObject.text = text;
        }
    }
}