using UnityEngine;
using Photon;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun;
using TankBycicleOnline.Constants;
using CustomEventBus;

public class MainMenuOnlineManager : MonoBehaviourPunCallbacks
{
    public static MainMenuOnlineManager Instance;
    [SerializeField] private string gameVersion = "0.1";
    private bool isConnecting;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = gameVersion;
    }

    public void ConnectAndPlay()
    {
        isConnecting = true;

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            return;
        }

        if (PhotonNetwork.InRoom)
        {
            Debug.LogWarning("Already in room");
            return;
        }

        if (PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.ConnectedToMasterServer)
        {
            JoinRandomOrCreate();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master");

        PhotonNetwork.NickName = "Player_" + Random.Range(0, 100);

        if (isConnecting)
        {
            JoinRandomOrCreate();
        }
    }

    private void JoinRandomOrCreate()
    {
        Debug.Log("Trying JoinRandomOrCreateRoom...");

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = Const.MaxPlayers,
            IsVisible = true,
            IsOpen = true,
            CleanupCacheOnLeave = true
        };

        PhotonNetwork.JoinRandomOrCreateRoom(
            expectedCustomRoomProperties: null,
            expectedMaxPlayers: Const.MaxPlayers,
            matchingType: MatchmakingMode.FillRoom,
            typedLobby: null,
            sqlLobbyFilter: null,
            roomName: null,
            roomOptions: roomOptions
        );
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined room: {PhotonNetwork.CurrentRoom.Name}");

        SetLocalReady(false);

        SimpleEventBus.OpenLobby?.Invoke();

    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"Join random failed: {message}");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning($"Disconnected from Photon: {cause}");
        isConnecting = false;
    }

    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left room");

        SimpleEventBus.CloseLobby?.Invoke();
    }


    public void SetLocalReady(bool isReady)
    {
        Hashtable props = new Hashtable
        {
            {Const.ReadyKey, isReady}
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
}
