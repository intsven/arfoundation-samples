using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Scripts
{
    public class SceneLoaderButton : MonoBehaviour
    {
        [SerializeField] private string _scene;

        protected bool IsOnAndroid { get { return Application.platform == RuntimePlatform.Android; } }
        protected bool IsOnIOS { get { return Application.platform == RuntimePlatform.IPhonePlayer; } }
        protected bool IsOnMobile { get { return IsOnAndroid || IsOnIOS; } }

        void Update()
        {
            if (DidTouchBegin() || Input.GetMouseButton(0))
            {
                Vector3 touchPosition;

                if (IsOnMobile && DidTouchBegin())
                    touchPosition = Input.GetTouch(0).position;
                else
                    touchPosition = Input.mousePosition;

                var ray = Camera.main.ScreenPointToRay(touchPosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject == gameObject)
                        HandleTouch();
                }
            }
        }

        public void GoToScene(string scene)
        {
            SceneManager.LoadScene(scene);
        }

        private bool DidTouchBegin()
        {
            return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
        }

        private void HandleTouch()
        {
            GoToScene(_scene);
        }
    }
}