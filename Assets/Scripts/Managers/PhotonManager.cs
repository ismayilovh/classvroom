using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using SlimUI.ModernMenu;
using UnityEngine.SceneManagement;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public static PhotonManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Debug.Log("Connecting to master");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.JoinLobby();
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
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
        Debug.Log("Created Room");
        MenuManager.Instance.OnCreatedRoom();
    }

    public void JoinRoom(string classroom)
    {
        Debug.Log($"Joining Room: {classroom}");
        PhotonNetwork.JoinRoom(classroom);
        //MenuManager.Instance.OpenMenu("Loading");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        MenuManager.Instance.SetStudentCount(PhotonNetwork.CurrentRoom.PlayerCount);
        MenuManager.Instance.OnJoinedRoom();
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

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Join room failed: {message}");
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log($"Master switched to {newMasterClient.NickName}");
        // startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Room creation failed: {message}");
        // MenuManager.Instance.OpenMenu("Error Menu");
    }

    public void StartRoom()
    {
        PhotonNetwork.LoadLevel("Classroom");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftLobby()
    {
        Debug.Log("Left lobby");
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left room");
        if (SceneManager.GetActiveScene().name != "Menu")
        {
            Debug.Log("Scene loading...");
            SceneManager.LoadScene("Menu");
        }
        else
        {
            MenuManager.Instance.OnLeftRoom();
        }
        
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

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        MenuManager.Instance.OnStudentLeft();
        Debug.Log($"Player named {otherPlayer.NickName} has left room");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        MenuManager.Instance.OnStudentEntered();
        Debug.Log($"Player named {newPlayer.NickName} has entered to room");
        //Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }

    private void OnApplicationQuit()
    {
        if (PhotonNetwork.InRoom) PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
    }
}
