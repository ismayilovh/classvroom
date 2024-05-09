using PlayFab;
using PlayFab.ClientModels;
using SlimUI.ModernMenu;
using UnityEngine;

namespace Managers
{
    public class PlayfabManager : MonoBehaviour
    {
        public static PlayfabManager Instance { get; private set; }
        private string _email, _password;
        private bool _isRememberMe;

        private void Awake()
        {
            if (Instance) { Destroy(gameObject); return; } Instance = this;
            
            if (PlayerPrefs.HasKey("Email") && PlayerPrefs.HasKey("Password"))
            {
                string email = PlayerPrefs.GetString("Email");
                string password = PlayerPrefs.GetString("Password");
                
                MenuManager.Instance.EnableProfileMode();
                
                Login(email, password, true);
                return;
            }

            MenuManager.Instance.EnableLoginMode();
        }

        private bool IsValid(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)) return false;
            return true;
        }
    
        public void Login(string email, string password, bool isRememberMe)
        {
            if(!IsValid(email, password)){ MenuManager.Instance.SendErrorMessage("Email and Password are not valid."); return;}
            
            _email = email;
            _password = password;
            _isRememberMe = isRememberMe;
            
            var request = new LoginWithEmailAddressRequest()
            {
                Email = email,
                Password = password,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                {
                    GetPlayerProfile = true,
                    GetUserData = true
                }
            };

            MenuManager.Instance.EnableLoading();
            PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
        }

        public void Logout()
        {
            PlayerPrefs.DeleteAll();
            // PhotonManager.Instance.Disconnect();
            // Destroy(ClassroomManager.Instance.gameObject);
            // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            MenuManager.Instance.EnableLoginMode();
            MenuManager.Instance.EnableMainCanvas();
        }

        private void OnLoginSuccess(LoginResult result)
        {
            string accountName = "";
            
            MenuManager.Instance.DisableLoading();
            
            if(_isRememberMe){ PlayerPrefs.SetString("Email", _email); PlayerPrefs.SetString("Password", _password);}

            if (string.IsNullOrEmpty(result.InfoResultPayload.PlayerProfile.DisplayName))
            {
                MenuManager.Instance.SendErrorMessage("This account doesn't have a name. Please contact with your school");
                return;
            }
            accountName = result.InfoResultPayload.PlayerProfile.DisplayName;
            PlayerPrefs.SetString("Name", accountName); 
            MenuManager.Instance.SendDebugMessage("Logged in successfully as: " + accountName);

            if (!result.InfoResultPayload.UserData.TryGetValue("Role", out var role))
            {
                MenuManager.Instance.SendErrorMessage("This account doesn't have a role. Please contact with your school"); 
                return;
            }

            if (role.Value != PlayerPrefs.GetInt("Role").ToString())
            {
                MenuManager.Instance.SendErrorMessage("You didn't choose the correct login type. Please return and change it"); 
                return;
            }
            
            if (!result.InfoResultPayload.UserData.TryGetValue("Classroom", out var classroom) && role.Value == "0")
            {
                MenuManager.Instance.SendErrorMessage("This account doesn't have a classroom. Please contact with your school"); 
                return;
            }
            
            if (!result.InfoResultPayload.UserData.TryGetValue("Desk", out var desk) && role.Value == "0")
            {
                MenuManager.Instance.SendErrorMessage("This account doesn't have a desk. Please contact with your school"); 
                return;
            }
            
            MenuManager.Instance.EnableProfileMode();

            MenuManager.Instance.AddName(accountName);
            
            PhotonManager.Instance.ChangeNickName(accountName);
            
            if (role.Value == "0")
            {
                MenuManager.Instance.DisableTeacherFields();

                MenuManager.Instance.AddClassroom(classroom.Value);
                MenuManager.Instance.AddDesk(desk.Value);
            }
            else
            {
                MenuManager.Instance.DisableStudentFields();
            }

            MenuManager.Instance.DisableLoginCampaign();
            MenuManager.Instance.EnableMainCanvas();
        }

        private void OnLoginFailure(PlayFabError error)
        {
            MenuManager.Instance.SendErrorMessage("Login failed: " + error.GenerateErrorReport());
            MenuManager.Instance.DisableLoading();
        }
    }
}