using CustomEventBus;
using Photon.Pun;
using Photon.Realtime;
using TankBycicleOnline.Constants;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace TankBycicleOnline.Online
{
    public class RoomLobbyManager : MonoBehaviourPunCallbacks
    {
        [Header("Panels")]
        [SerializeField] private GameObject roomPanel;

        [Header("Buttons")]
        [SerializeField] private Button readyButton;
        [SerializeField] private Button startButton;
        [SerializeField] private Button leaveButton;

        [Header("Texts")]
        [SerializeField] private TMP_Text roomNameText;
        [SerializeField] private TMP_Text statusText;

        [Header("Player Slots")]
        [SerializeField] private RoomPlayerItem[] playerSlots;

        private bool localReady;
        private void OnEnable()
        {
            SimpleEventBus.OpenLobby += OpenLobby;
            SimpleEventBus.CloseLobby += CloseLobby;
        }

        private void OnDisable()
        {
            SimpleEventBus.OpenLobby -= OpenLobby;
            SimpleEventBus.CloseLobby -= CloseLobby;
        }
        private void Start()
        {
            if (roomPanel != null)
                roomPanel.SetActive(false);

            readyButton.onClick.AddListener(OnReadyClicked);
            startButton.onClick.AddListener(OnStartClicked);
            leaveButton.onClick.AddListener(OnLeaveClicked);

            startButton.gameObject.SetActive(false);

            ClearAllSlots();
        }

        public void OpenLobby()
        {
            roomPanel.SetActive(true);
            localReady = false;
            RefreshAll();
        }

        public void CloseLobby()
        {
            roomPanel.SetActive(false);
            ClearAllSlots();
        }

        private void OnReadyClicked()
        {
            localReady = !localReady;
            MainMenuOnlineManager.Instance.SetLocalReady(localReady);
            RefreshAll();
        }

        private void OnStartClicked()
        {
            Debug.Log("Clicked");
            if (!PhotonNetwork.IsMasterClient)
                return;
            Debug.Log("nEXT sTEP");
            if (AreAllPlayersReady())
                return;

            Debug.Log("Satart Loading");
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            PhotonNetwork.LoadLevel(Const.OnlineScene);
        }

        private void OnLeaveClicked()
        {
            MainMenuOnlineManager.Instance.LeaveRoom();
        }

        private bool AreAllPlayersReady()
        {
            if (PhotonNetwork.PlayerList.Length < 2)
                return false;

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (!TryGetReady(player, out bool isReady) || !isReady)
                    return false;
            }

            return true;
        }

        private bool TryGetReady(Player player, out bool isReady)
        {
            isReady = false;

            if (player.CustomProperties.TryGetValue(Const.ReadyKey, out object value))
            {
                if (value is bool readyValue)
                {
                    isReady = readyValue;
                    return true;
                }
            }

            return false;
        }

        private void RefreshAll()
        {
            if (!PhotonNetwork.InRoom)
                return;

            if (roomNameText != null)
                roomNameText.text = $"Room: {PhotonNetwork.CurrentRoom.Name}";

            RefreshPlayerSlots();
            RefreshButtons();
            RefreshStatus();
        }

        private void RefreshPlayerSlots()
        {
            ClearAllSlots();

            Player[] players = PhotonNetwork.PlayerList;

            for (int i = 0; i < players.Length; i++)
            {
                if (i >= playerSlots.Length)
                    break;

                Player player = players[i];
                bool isReady = TryGetReady(player, out bool ready) && ready;

                playerSlots[i].Setup(player, isReady, player.IsMasterClient);
            }
        }

        private void ClearAllSlots()
        {
            for (int i = 0; i < playerSlots.Length; i++)
            {
                if (playerSlots[i] != null)
                    playerSlots[i].Clear();
            }
        }

        private void RefreshButtons()
        {
            bool isMaster = PhotonNetwork.IsMasterClient;

            startButton.gameObject.SetActive(isMaster);
            startButton.interactable = isMaster && AreAllPlayersReady();

            TMP_Text btnText = readyButton.GetComponentInChildren<TMP_Text>();
            if (btnText != null)
                btnText.text = localReady ? "Ready" : "Unready";
        }

        private void RefreshStatus()
        {
            int total = PhotonNetwork.PlayerList.Length;
            int readyCount = 0;

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (TryGetReady(player, out bool ready) && ready)
                    readyCount++;
            }

            if (PhotonNetwork.IsMasterClient)
                statusText.text = $"Players ready: {readyCount}/{total}";
            else
                statusText.text = AreAllPlayersReady()
                    ? "All players are ready. Waiting for host..."
                    : $"Waiting players: {readyCount}/{total} ready";
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            RefreshAll();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            RefreshAll();
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            RefreshAll();
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            RefreshAll();
        }
    }
}
