using UnityEngine;
using UnityEngine.SceneManagement;
using Core.Game;
using System.Collections;
using Game.Settings;
using Game.UI;

namespace Game.Manager
{
    public sealed class GameInstance : Singleton<GameInstance>
    {

        public enum EGameInstanceState
        {
            Entrance = 0,
            Setup = 1,
            Playing = 2
        }

        public enum EGameInstanceHudState
        {
            Default = 0,
            Paused = 1
        }

        public EGameInstanceState State
        {
            get
            {
                return m_state;
            }
        }

        private EGameInstanceState m_state = EGameInstanceState.Entrance;
        private EGameInstanceState m_oldState = EGameInstanceState.Entrance;


        public delegate void OnChangeInstanceStateHandler(EGameInstanceState oldState, EGameInstanceState newState);
        public event OnChangeInstanceStateHandler OnChangeInstanceState;

        public EGameInstanceHudState HudState
        {
            get
            {
                return m_hudState;
            }
        }

        private EGameInstanceHudState m_hudState = EGameInstanceHudState.Default;

        public delegate void OnChangeInstanceHudStateHandler(EGameInstanceHudState newState);
        public event OnChangeInstanceHudStateHandler OnChangeInstanceHudState;

        public delegate void OnSettingsChangedHandler();
        public event OnSettingsChangedHandler OnSettingsChanged;

        public string currentGameInstance;

        public SettingsManager Settings;

        private void OnEnable()
        {
            this.currentGameInstance = System.Guid.NewGuid().ToString();

            if (!this.SetInstance(this))
            {
                Destroy(this.gameObject);
                return;
            }

            GameObject.DontDestroyOnLoad(this.gameObject);

            Settings = new SettingsManager();

            setInstanceState(EGameInstanceState.Entrance);
            SetHudInstanceState(EGameInstanceHudState.Default);

            SceneManager.sceneLoaded += OnSceneLoaded;
            Application.targetFrameRate = 30;

            TimerManager.setTime(9, 1, 2, 5, 2023);
        }

        private void Start()
        {
            StartCoroutine(ProcessFPS());
        }

        public void SettingsChanged()
        {
            if (OnSettingsChanged != null)
                OnSettingsChanged.Invoke();
        }

        public void setInstanceState(EGameInstanceState newState)
        {
            if (newState == m_state)
                return;

            m_oldState = m_state;
            m_state = newState;

            if (OnChangeInstanceState != null)
                OnChangeInstanceState(m_oldState, m_state);
        }

        public void SetHudInstanceState(EGameInstanceHudState newState)
        {
            if (newState == m_hudState)
                return;

            m_hudState = newState;

            if (OnChangeInstanceHudState != null)
                OnChangeInstanceHudState(m_hudState);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            if (sceneMode != LoadSceneMode.Additive)
                SetHudInstanceState(EGameInstanceHudState.Default);
        }

        float generalTimerCounter = 0;

        private void Update()
        {
            accum += Time.timeScale / Time.deltaTime;
            ++frames;

            var stateFromMachine = UpdateStateMachine();
            setInstanceState(stateFromMachine);
        }


        #region StateMachine

        private EGameInstanceState UpdateStateMachine()
        {
            if (State == EGameInstanceState.Setup) return Update_Setup();
            if (State == EGameInstanceState.Playing) return Update_Playing();

            return Update_Entrance();
        }

        private EGameInstanceState Update_Entrance()
        {
            return EGameInstanceState.Entrance;
        }

        private EGameInstanceState Update_Setup()
        {
            return EGameInstanceState.Setup;
        }

        int totalSpawned = 0;

        private EGameInstanceState Update_Playing()
        {
            generalTimerCounter += Time.deltaTime;

            float hourSpeed = TimerManager.secondEquivalenceHour;
            if (!OfficeInstance.Instance.IsWorkingHour)
                hourSpeed /= 4;

            if (generalTimerCounter >= hourSpeed)
            {
                generalTimerCounter = 0;
                TimerManager.hour++;
            }

            return EGameInstanceState.Playing;
        }

        #endregion

        #region Alert

        [SerializeField] private UIAlert _alertModel;
        [SerializeField] private UIAlert _alertConfirmModel;

        public UIAlert CurrentAlertOnScreen
        {
            get
            {
                return _alertOnScreen;
            }
        }
        private UIAlert _alertOnScreen;

        public void Alert(string title, string content, EUIAlertType _type)
        {
            Alert(title, content, _type, () => { }, () => { }, new string[0]);
        }

        public void Alert(string title, string content, EUIAlertType _type, string[] subs)
        {
            Alert(title, content, _type, () => { }, () => { }, subs);
        }

        public void Alert(string title, string content, EUIAlertType _type, System.Action callback)
        {
            Alert(title, content, _type, callback, () => { }, new string[0]);
        }

        public void Alert(string title, string content, EUIAlertType _type, System.Action callback, string[] subs)
        {
            Alert(title, content, _type, callback, () => { }, subs);
        }

        public void Alert(string title, string content, EUIAlertType _type, System.Action callback, System.Action callbackCancel, string[] subs)
        {
            if (_alertOnScreen != null)
                _alertOnScreen.ForceDestroy();

            GameObject go = GameObject.FindGameObjectWithTag("mainCanvas");
            Canvas c = go != null ? go.GetComponent<Canvas>() : null;
            if (c == null)
            {
                c = GameObject.FindObjectOfType<Canvas>();
                if (c == null)
                {
                    Debug.LogError("Alert needs a Canvas before spawn.");
                    return;
                }
            }

            UIAlert alert;

            if (_type == EUIAlertType.Alert || _type == EUIAlertType.Success)
            {
                alert = Instantiate(_alertModel, Vector3.zero, Quaternion.identity, c.transform);
            }
            else
            {
                alert = Instantiate(_alertConfirmModel, Vector3.zero, Quaternion.identity, c.transform);
            }

            alert.transform.SetAsLastSibling();

            if (_type == EUIAlertType.Success)
                alert.SetAlert(title, content, true, subs);
            else
                alert.SetAlert(title, content, subs);

            alert.Confirmed = () =>
            {
                callback.Invoke();
            };

            alert.Canceled = () =>
            {
                callbackCancel.Invoke();
            };

            _alertOnScreen = alert;
        }

        #endregion

        #region FPS

        [SerializeField]
        private float frequency = 0.5F; // The update frequency of the fps

        [SerializeField]
        private int nbDecimal = 1; // How many decimal do you want to display

        private float accum = 0f; // FPS accumulated over the interval
        private int frames = 0; // Frames drawn over the interval

        public string FPSFormated
        {
            get { return this.sFPS; }
        }
        private string sFPS = "";

        public float FPS
        {
            get { return this.m_internalFps; }
        }
        private float m_internalFps = 0f;

        public Color FPSColor
        {
            get { return colorFps; }
        }
        private Color colorFps = Color.white;

        IEnumerator ProcessFPS()
        {
            // Infinite loop executed every "frenquency" secondes.
            while (true)
            {
                // Update the FPS
                this.m_internalFps = accum / frames;
                sFPS = this.m_internalFps.ToString("f" + Mathf.Clamp(nbDecimal, 0, 10));

                //Update the color
                colorFps = (this.m_internalFps >= 30) ? Color.green : ((this.m_internalFps > 10) ? Color.red : Color.yellow);


                accum = 0.0F;
                frames = 0;

                yield return new WaitForSeconds(frequency);
            }
        }

        #endregion
    }
}
