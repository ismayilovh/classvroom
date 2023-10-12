using System;
using PlayFab;
using PlayFab.ClientModels;
using SlimUI.ModernMenu;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class PlayfabManager : MonoBehaviour
    {
        public static PlayfabManager Instance { get; private set; }

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
            if (email == "" || password == "") return false;
            return true;
        }
    
        public void Login(string email, string password, bool isRememberMe)
        {
            if(!IsValid(email, password)){ MenuManager.Instance.SendErrorMessage("Email and Password are not valid."); return;}
            if(isRememberMe){ PlayerPrefs.SetString("Email", email); PlayerPrefs.SetString("Password", password);}

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

            PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
        }

        public void Logout()
        {
            PlayerPrefs.DeleteAll();
            Destroy(ClassroomManager.Instance.gameObject);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void OnLoginSuccess(LoginResult result)
        {
            string accountName = "";

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
            
            if (!result.InfoResultPayload.UserData.TryGetValue("Deck", out var deck) && role.Value == "0")
            {
                MenuManager.Instance.SendErrorMessage("This account doesn't have a deck. Please contact with your school"); 
                return;
            }
            
            MenuManager.Instance.EnableProfileMode();

            MenuManager.Instance.AddName(accountName);
            
            PhotonManager.Instance.ChangeNickName(accountName);
            
            if (role.Value == "0")
            {
                MenuManager.Instance.DisableTeacherFields();

                MenuManager.Instance.AddClassroom(classroom.Value);
                MenuManager.Instance.AddDeck(deck.Value);
            }
            else
            {
                MenuManager.Instance.DisableStudentFields();
            }
            
            MenuManager.Instance.EnterLeft();
            // PhotonManager.Instance.JoinRoom(classroom.Value);
            // UIMenuManager.Instance.LoadScene(menuName);
        }

        private void OnLoginFailure(PlayFabError error)
        {
            MenuManager.Instance.SendErrorMessage("Login failed: " + error.GenerateErrorReport());
        }
    }
}