using UnityEngine;
using UnityEngine.UI;

namespace Scripts
{
    public class ToggleARObjectVisibility : MonoBehaviour
    {
        public GameObject ARObject;

        private Text _textField;
        private GameObject[] _billboards;

        void Awake()
        {
            _textField = GetComponentInChildren<Text>();
            _billboards = GameObject.FindGameObjectsWithTag("billboard");
        }

        public void Toggle ()
        {
            if(ARObject != null)
            {
                if(ARObject.activeSelf) 
                {
                    ARObject.SetActive(false);
                    HideAllBillboards();
                    _textField.text = "Show";
                }
                else
                {
                    ARObject.SetActive(true);
                    ShowAllBillboards();
                    _textField.text = "Hide";
                }
            }
        }

        private void HideAllBillboards()
        {
            foreach (var billboard in _billboards)
                billboard.gameObject.SetActive(false);
        }

        private void ShowAllBillboards()
        {
            foreach (var billboard in _billboards)
                billboard.gameObject.SetActive(true);
        }
    }
}
