using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Game.UI
{
    public enum EUIAlertType
    {
        Alert,
        Confirm,
        Success
    }

    public class UIAlert : MonoBehaviour
    {
        public Action Confirmed;
        public Action Canceled;

        [SerializeField] private Text m_txtTitleAlert;
        [SerializeField] private Text m_txtContentAlert;

        [SerializeField] private Button btnConfirm;
        [SerializeField] private Button btnCancel;

        bool eventSet = false;

        public void SetAlert(string title, string content, string[] subs)
        {
            SetAlert(title, content, false, subs);
        }

        public void SetAlert(string title, string content, bool green, string[] subs)
        {
            m_txtTitleAlert.text = title;

            if (subs.Length > 0)
            {
                content = string.Format(content, subs);
            }
            m_txtContentAlert.text = content;
        }

        void Start()
        {
            if (btnConfirm != null)
            {
                if (EventSystem.current != null)
                    EventSystem.current.SetSelectedGameObject(btnConfirm.gameObject);
                btnConfirm.Select();
            }
        }

        void Update()
        {
            if (EventSystem.current != null)
            {
                if (btnCancel == null)
                {
                    if (EventSystem.current.currentSelectedGameObject != btnConfirm.gameObject)
                    {
                        EventSystem.current.SetSelectedGameObject(btnConfirm.gameObject);
                        btnConfirm.Select();
                    }
                }
                else
                {
                    if (EventSystem.current.currentSelectedGameObject != btnConfirm.gameObject && EventSystem.current.currentSelectedGameObject != btnCancel.gameObject)
                    {
                        EventSystem.current.SetSelectedGameObject(btnConfirm.gameObject);
                        btnConfirm.Select();
                    }
                }
            }

        }

        void OnEnable()
        {
            if (!eventSet)
            {
                eventSet = true;



                if (btnConfirm != null)
                {
                    btnConfirm.onClick.AddListener(() =>
                    {
                        Confirmed.Invoke();
                        Destroy(this.gameObject);
                    });
                }

                if (btnCancel != null)
                {
                    btnCancel.onClick.AddListener(() =>
                    {
                        Canceled.Invoke();
                        Destroy(this.gameObject);
                    });
                }

            }
        }

        public void ForceDestroy()
        {
            if (btnCancel != null)
            {
                Canceled.Invoke();
                Destroy(this.gameObject);
            }
            else
            {
                Confirmed.Invoke();
                Destroy(this.gameObject);
            }
        }
    }
}
