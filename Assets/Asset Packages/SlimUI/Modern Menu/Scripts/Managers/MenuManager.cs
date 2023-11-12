using UnityEngine;
using System.Collections;
using Managers;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using WebSocketSharp;

namespace SlimUI.ModernMenu{
	public class MenuManager : MonoBehaviour
	{
		public static MenuManager Instance { get; private set; }
		
		#region Animator Components
		
		private Animator CameraObject;
        private static readonly int EnterRightProperty = Animator.StringToHash("Enter Right");
        private static readonly int ExitRightProperty = Animator.StringToHash("Exit Right");
        private static readonly int EnterLeftProperty = Animator.StringToHash("Enter Left");
        private static readonly int ExitLeftProperty = Animator.StringToHash("Exit Left");

        #endregion
        
        #region Menus
        
		// campaign button sub menu
        [Header("Menus")]
        [Tooltip("The Menu for when the MAIN menu buttons")]
        public GameObject mainMenu;
        [Tooltip("THe first list of buttons")]
        public GameObject firstMenu;
        [Tooltip("The Menu for when the PLAY button is clicked")]
        public GameObject playMenu;
        [Tooltip("The Menu for when the EXIT button is clicked")]
        public GameObject exitMenu;
        [Tooltip("Optional 4th Menu")]
        public GameObject extrasMenu;

        #endregion
        
        #region Theme

        public enum Theme {custom1, custom2, custom3};
        [Header("Theme Settings")]
        public Theme theme;
        private int themeIndex;
        public ThemedUIData themeController;

        #endregion

        #region Panels
        
        [Header("Panels")]
        [Tooltip("The UI Panel parenting all sub menus")]
        public GameObject mainCanvas;
        [Tooltip("The UI Panel for settings")]
        public GameObject settingsCanvas;
        [Tooltip("The UI Panel for login")]
        public GameObject loginCanvas;
        [Tooltip("The UI Panel for loading")]
        public GameObject loadingCanvas;
        [Tooltip("The UI Panel that holds the CONTROLS window tab")]
        public GameObject PanelControls;
        [Tooltip("The UI Panel that holds the VIDEO window tab")]
        public GameObject PanelVideo;
        [Tooltip("The UI Panel that holds the GAME window tab")]
        public GameObject PanelGame;
        [Tooltip("The UI Panel that holds the KEY BINDINGS window tab")]
        public GameObject PanelKeyBindings;
        [Tooltip("The UI Sub-Panel under KEY BINDINGS for MOVEMENT")]
        public GameObject PanelMovement;
        [Tooltip("The UI Sub-Panel under KEY BINDINGS for COMBAT")]
        public GameObject PanelCombat;
        [Tooltip("The UI Sub-Panel under KEY BINDINGS for GENERAL")]
        public GameObject PanelGeneral;
        
        #endregion
        
        #region Main Screen Components
        
        [Header("Main Screen")]
        [SerializeField] private GameObject loginButton;
        [SerializeField] private GameObject profileButton;
        
        #endregion

        #region Settings Screen Components
        
        // highlights in settings screen
        [Header("Settings Screen")]
        [Tooltip("Highlight Image for when GAME Tab is selected in Settings")]
        public GameObject lineGame;
        [Tooltip("Highlight Image for when VIDEO Tab is selected in Settings")]
        public GameObject lineVideo;
        [Tooltip("Highlight Image for when CONTROLS Tab is selected in Settings")]
        public GameObject lineControls;
        [Tooltip("Highlight Image for when KEY BINDINGS Tab is selected in Settings")]
        public GameObject lineKeyBindings;
        [Tooltip("Highlight Image for when MOVEMENT Sub-Tab is selected in KEY BINDINGS")]
        public GameObject lineMovement;
        [Tooltip("Highlight Image for when COMBAT Sub-Tab is selected in KEY BINDINGS")]
        public GameObject lineCombat;
        [Tooltip("Highlight Image for when GENERAL Sub-Tab is selected in KEY BINDINGS")]
        public GameObject lineGeneral;

        #endregion
        
        #region Login Screen Components
        
        [Header("Login Screen")]
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private string menuName;
        private string _email, _password;
        private bool _isRememberMe = true;

        #endregion
        
        #region Profile Screen Components
        
        [Header("Profile Screen")]
        [SerializeField] private GameObject profilePanel;
        [SerializeField] private GameObject classroomPanel;
        [SerializeField] private GameObject examPanel;
        [SerializeField] private GameObject aiLessonPanel;
        [SerializeField] private TMP_Text helloText;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text classroomText;
        [SerializeField] private TMP_Text deskText;
        [SerializeField] private TMP_Text[] studentCountTexts;
        [SerializeField] private GameObject[] studentFields;
        [SerializeField] private GameObject[] teacherFields;
        [SerializeField] private GameObject[] studentRoomFields;
        [SerializeField] private GameObject[] teacherRoomFields;
        private int _role, _studentCount;
        private string _classroom, _desk;
        private string _examSubject, _examQuestionCount, _examDifficulty;
        private string _lessonSubject, _lesson;
		
        #endregion
        
        #region Loading Screen Components
        
        [Header("Loading Screen")]
		[Tooltip("If this is true, the loaded scene won't load until receiving user input")]
		public bool waitForInput = true;
        public GameObject loadingMenu;
		[Tooltip("The loading bar Slider UI element in the Loading Screen")]
        public Slider loadingBar;
        public TMP_Text loadPromptText;
		public KeyCode userPromptKey;

		[Header("SFX")]
        [Tooltip("The GameObject holding the Audio Source component for the HOVER SOUND")]
        public AudioSource hoverSound;
        [Tooltip("The GameObject holding the Audio Source component for the AUDIO SLIDER")]
        public AudioSource sliderSound;
        [Tooltip("The GameObject holding the Audio Source component for the SWOOSH SOUND when switching to the Settings Screen")]
        public AudioSource swooshSound;
        
		#endregion

        private void Awake()
        {
	        if (Instance == null) Instance = this; else Destroy(gameObject);
        }

        void Start(){
			if (Camera.main != null) CameraObject = Camera.main.GetComponent<Animator>();
			if (PlayerPrefs.HasKey("Role")) _role = PlayerPrefs.GetInt("Role");

			playMenu.SetActive(false);
			exitMenu.SetActive(false);
			if(extrasMenu) extrasMenu.SetActive(false);
			firstMenu.SetActive(true);
			mainMenu.SetActive(true);

			SetThemeColors();
		}

		void SetThemeColors()
		{
			switch (theme)
			{
				case Theme.custom1:
					themeController.currentColor = themeController.custom1.graphic1;
					themeController.textColor = themeController.custom1.text1;
					themeIndex = 0;
					break;
				case Theme.custom2:
					themeController.currentColor = themeController.custom2.graphic2;
					themeController.textColor = themeController.custom2.text2;
					themeIndex = 1;
					break;
				case Theme.custom3:
					themeController.currentColor = themeController.custom3.graphic3;
					themeController.textColor = themeController.custom3.text3;
					themeIndex = 2;
					break;
				default:
					Debug.Log("Invalid theme selected.");
					break;
			}
		}

		#region Handlers
		
        public void HandleEmail(string email)
        {
	        _email = email;
        }

        public void HandlePassword(string password)
        {
	        _password = password;
        }

        public void HandleRememberMe(bool isRememberMe)
        {
	        _isRememberMe = isRememberMe;
        }

        public void HandleClassroomName(string classroom)
        {
	        _classroom = classroom;
        }

        public void HandleExamSubject(string subject)
        {
	        _examSubject = subject;
        }
        
        public void HandleExamQuestionCount(string questionCount)
        {
	        _examQuestionCount = questionCount;
        }
        
        public void HandleExamDifficulty(string difficulty)
        {
	        _examDifficulty = difficulty;
        }

        public void HandleLessonSubject(string subject)
        {
	        _lessonSubject = subject;
        }
        
        public void HandleLesson(string lesson)
        {
	        _lesson = lesson;
        }

        #endregion

        public void EnableLoading()
        {
	        loadingCanvas.SetActive(true);
        }

        public void DisableLoading()
        {
	        loadingCanvas.SetActive(false);
        }

		public void EnableLoginMode()
		{
			profileButton.SetActive(false);
			loginButton.SetActive(true);
		}
		
		public void EnableProfileMode()
		{
			profileButton.SetActive(true);
			loginButton.SetActive(false);
		}
		
		#region Main
		
		public void PlayCampaign(){
			exitMenu.SetActive(false);
			if(extrasMenu) extrasMenu.SetActive(false);
			playMenu.SetActive(true);
		}
		
		public void PlayCampaignMobile(){
			exitMenu.SetActive(false);
			if(extrasMenu) extrasMenu.SetActive(false);
			playMenu.SetActive(true);
			mainMenu.SetActive(false);
		}

		public void LoginCampaign(int loginType)
		{
			loginCanvas.SetActive(true);
			settingsCanvas.SetActive(false);
			_role = loginType;
			PlayerPrefs.SetInt("Role", _role);
		}

		public void SettingsCampaign()
		{
			settingsCanvas.SetActive(true);
			loginCanvas.SetActive(false);
		}

		public void ReturnMenu(){
			playMenu.SetActive(false);
			if(extrasMenu) extrasMenu.SetActive(false);
			exitMenu.SetActive(false);
			mainMenu.SetActive(true);
		}

		public void LoadScene(string scene){
			if(scene != ""){
				StartCoroutine(LoadAsynchronously(scene));
			}
		}

		public void DisablePlayCampaign(){
			playMenu.SetActive(false);
		}

		public void EnterRight(){
			DisablePlayCampaign();
			CameraObject.SetTrigger(EnterRightProperty);
		}
		
		public void ExitRight(){
			CameraObject.SetTrigger(ExitRightProperty);
		}
		
		public void EnterLeft(){
			DisablePlayCampaign();
			CameraObject.SetTrigger(EnterLeftProperty);
		}
		
		public void ExitLeft(){
			CameraObject.SetTrigger(ExitLeftProperty);
		}
		
		public void PlayHover(){
			hoverSound.Play();
		}

		public void PlaySFXHover(){
			sliderSound.Play();
		}

		public void PlaySwoosh(){
			swooshSound.Play();
		}

		// Are You Sure - Quit Panel Pop Up
		public void AreYouSure(){
			exitMenu.SetActive(true);
			if(extrasMenu) extrasMenu.SetActive(false);
			DisablePlayCampaign();
		}

		public void AreYouSureMobile(){
			exitMenu.SetActive(true);
			if(extrasMenu) extrasMenu.SetActive(false);
			mainMenu.SetActive(false);
			DisablePlayCampaign();
		}

		public void ExtrasMenu(){
			playMenu.SetActive(false);
			if(extrasMenu) extrasMenu.SetActive(true);
			exitMenu.SetActive(false);
		}

		public void QuitGame(){
			#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
			#else
				Application.Quit();
			#endif
		}

		#endregion Main
		
		#region Settings
		
		public void GamePanel(){
			DisableRightPanels();
			PanelGame.SetActive(true);
			lineGame.SetActive(true);
		}

		public void VideoPanel(){
			DisableRightPanels();
			PanelVideo.SetActive(true);
			lineVideo.SetActive(true);
		}

		public void ControlsPanel(){
			DisableRightPanels();
			PanelControls.SetActive(true);
			lineControls.SetActive(true);
		}

		public void KeyBindingsPanel(){
			DisableRightPanels();
			MovementPanel();
			PanelKeyBindings.SetActive(true);
			lineKeyBindings.SetActive(true);
		}

		public void MovementPanel(){
			DisableRightPanels();
			PanelKeyBindings.SetActive(true);
			PanelMovement.SetActive(true);
			lineMovement.SetActive(true);
		}

		public void CombatPanel(){
			DisableRightPanels();
			PanelKeyBindings.SetActive(true);
			PanelCombat.SetActive(true);
			lineCombat.SetActive(true);
		}

		public void GeneralPanel(){
			DisableRightPanels();
			PanelKeyBindings.SetActive(true);
			PanelGeneral.SetActive(true);
			lineGeneral.SetActive(true);
		}
		
		#endregion Settings
		
		#region Login
		
        public void Login()
        {
	        PlayfabManager.Instance.Login(_email, _password, _isRememberMe);
        }
		
		public void SendDebugMessage(string message)
		{
			messageText.color = Color.green;
			messageText.text = message;
		}
        
		public void SendErrorMessage(string message)
		{
			messageText.color = Color.red;
			messageText.text = message;
		}
		
		#endregion Login
		
		#region Profile
		
		public void AddName(string nickname)
		{
			nameText.text = $"Name: {nickname}";
			helloText.text = $"Hello, {nickname}";;
		}

		public void AddClassroom(string classroom)
		{
			_classroom = classroom;
			classroomText.text = $"Classroom: {_classroom}";
		}

		public void AddDesk(string desk)
		{
			_desk = desk;
			deskText.text = $"Desk: {_desk}";
		}
		
		public void JoinClassroom()
		{
			PhotonManager.Instance.JoinRoom(_classroom);
		}

		public void CreateClassroom()
		{
			PhotonManager.Instance.CreateRoom(_classroom);
		}

		public void LeaveClassroom()
		{
			PhotonManager.Instance.LeaveRoom();
		}
		
		public void StartClassroom()
		{
			PhotonManager.Instance.StartRoom();
		}
		
		public void OnCreatedRoom()
		{
			foreach (var field in teacherFields)
			{
				{
					field.SetActive(false);
				}
			}
			foreach (var field in teacherRoomFields)
			{
				field.SetActive(true);
			}
		}

		public void OnJoinedRoom()
		{
			if (PlayerPrefs.GetInt("Role").Equals(0))
			{
				foreach (var field in studentFields)
				{
					field.SetActive(false);
				}

				foreach (var field in studentRoomFields)
				{
					field.SetActive(true);
				}
			}
		}

		public void OnLeftRoom()
		{
			if (_role.Equals(0))
			{
				foreach (var field in studentFields)
				{
					field.SetActive(true);
				}
				foreach (var field in studentRoomFields)
				{
					field.SetActive(false);
				}
			}
			else
			{
				foreach (var field in teacherFields)
				{
					field.SetActive(true);
				}
				foreach (var field in teacherRoomFields)
				{
					field.SetActive(false);
				}
			}
		}

		public void SetStudentCount(int studentCount)
		{
			_studentCount = studentCount - 1;
			foreach (var studentCountText in studentCountTexts)
			{
				studentCountText.text = "Student count: " + _studentCount;
			}
		}
		
		public void OnStudentEntered()
		{
			_studentCount++;
			foreach (var studentCountText in studentCountTexts)
			{
				studentCountText.text = "Student count: " + _studentCount;
			}
		}
		
		public void OnStudentLeft()
		{
			_studentCount--;
			foreach (var studentCountText in studentCountTexts)
			{
				studentCountText.text = "Student count: " + _studentCount;
			}
		}

		public void GenerateExam()
		{
			if (_examSubject.IsNullOrEmpty() || _examQuestionCount.IsNullOrEmpty() || _examDifficulty.IsNullOrEmpty())
			{
				Debug.Log("Please fill all the fields");
				return;
			}

			ExamManager.Instance.GenerateExam(_examSubject, _examQuestionCount, _examDifficulty);
		}
		
		public void GenerateLesson()
		{
			if (_lessonSubject.IsNullOrEmpty() || _lesson.IsNullOrEmpty())
			{
				Debug.Log("Please fill all the fields");
				return;
			}

			AILessonManager.Instance.GenerateLesson(_lessonSubject, _lesson);
		}

		public void DisableStudentFields()
		{
			foreach (var field in studentFields)
			{
				field.SetActive(false);
			}
                
			foreach (var field in teacherFields)
			{
				field.SetActive(true);
			}
		}

		public void DisableTeacherFields()
		{
			foreach (var field in studentFields)
			{
				field.SetActive(true);
			}
                
			foreach (var field in teacherFields)
			{
				field.SetActive(false);
			}
		}
		
		public void DisableLeftPanels()
		{
			profilePanel.SetActive(false);
			classroomPanel.SetActive(false);
			examPanel.SetActive(false);
			aiLessonPanel.SetActive(false);
		}
		
		public void ProfilePanel()
		{
			DisableLeftPanels();
			profilePanel.SetActive(true);
		}
		
		public void ClassroomPanel()
		{
			DisableLeftPanels();
			classroomPanel.SetActive(true);
		}

		public void ExamPanel()
		{
			DisableLeftPanels();
			examPanel.SetActive(true);
		}
        
        public void AILessonPanel()
        {
        	DisableLeftPanels();
        	aiLessonPanel.SetActive(true);
        }
		
		#endregion Profile
		
		void DisableRightPanels(){
			PanelControls.SetActive(false);
			PanelVideo.SetActive(false);
			PanelGame.SetActive(false);
			PanelKeyBindings.SetActive(false);

			lineGame.SetActive(false);
			lineControls.SetActive(false);
			lineVideo.SetActive(false);
			lineKeyBindings.SetActive(false);

			PanelMovement.SetActive(false);
			lineMovement.SetActive(false);
			PanelCombat.SetActive(false);
			lineCombat.SetActive(false);
			PanelGeneral.SetActive(false);
			lineGeneral.SetActive(false);
		}

		// Load Bar synching animation
		IEnumerator LoadAsynchronously(string sceneName){ // scene name is just the name of the current scene being loaded
			AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
			operation.allowSceneActivation = false;
			mainCanvas.SetActive(false);
			loadingMenu.SetActive(true);

			while (!operation.isDone){
				float progress = Mathf.Clamp01(operation.progress / .95f);
				loadingBar.value = progress;

				if (operation.progress >= 0.9f && waitForInput){
					loadPromptText.text = "Press " + userPromptKey.ToString().ToUpper() + " to continue";
					loadingBar.value = 1;

					if (Input.GetKeyDown(userPromptKey)){
						operation.allowSceneActivation = true;
					}
                }else if(operation.progress >= 0.9f && !waitForInput){
					operation.allowSceneActivation = true;
				}

				yield return null;
			}
		}
	}
}