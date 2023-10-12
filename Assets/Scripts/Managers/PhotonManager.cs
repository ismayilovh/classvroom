using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using SlimUI.ModernMenu;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public static PhotonManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this; else Destroy(gameObject);
    }

    void Start()
    {
        Debug.Log("Connecting to master");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        //MenuManager.Instance.OpenMenu("Title");
    }

    public void ChangeNickName(string name)
    {
        PhotonNetwork.NickName = name;
    }
    
    public void CreateRoom(string roomName)
    {
        if (string.IsNullOrEmpty(roomName)) return;
        PhotonNetwork.CreateRoom(roomName);
        // MenuManager.Instance.OpenMenu("Loading");
    }

    public override void OnCreatedRoom()
    {
        PhotonNetwork.LoadLevel("Classroom");
    }

    public void JoinRoom(string classroom)
    {
        PhotonNetwork.JoinRoom(classroom);
        //MenuManager.Instance.OpenMenu("Loading");
    }
    
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        MenuManager.Instance.StudentCount = PhotonNetwork.CurrentRoom.PlayerCount;
        //MenuManager.Instance.OpenMenu("Room");

        // foreach (Transform player in playerListContent)
        // {
        //     Destroy(player.gameObject);
        // }

        // Player[] players = PhotonNetwork.PlayerList;

        // foreach (var player in players)
        // {
        //     Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(player);
        // }

        // startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        // startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        // errorText.text = "Room creation failed: " + message;
        // MenuManager.Instance.OpenMenu("Error Menu");
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel("Classroom");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        // MenuManager.Instance.OpenMenu("Loading");
    }

    public override void OnLeftRoom()
    {
        //MenuManager.Instance.OpenMenu("Title");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // foreach (Transform room in roomListContent)
        // {
        //     Destroy(room.gameObject);
        // }
        // foreach (var room in roomList)
        // {
        //     if(room.RemovedFromList)
        //         continue;
        //     Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(room);
        // }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        MenuManager.Instance.OnStudentEntered();
        Debug.Log($"Player named {newPlayer.NickName} has entered to room");
        //Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }
}
