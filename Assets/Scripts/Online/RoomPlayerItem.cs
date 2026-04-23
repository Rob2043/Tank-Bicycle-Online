using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TankBycicleOnline.Online
{
    public class RoomPlayerItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text nicknameText;
        [SerializeField] private TMP_Text readyText;
        [SerializeField] private TMP_Text hostText;
        
        [SerializeField] private GameObject root;

        public void Setup(Player player, bool isReady, bool isHost)
        {
            root.SetActive(true);

            nicknameText.text = player.NickName;
            readyText.text = isReady ? "Ready" : "Not Ready";
            hostText.text = isHost ? "Host" : "";
        }

        public void Clear()
        {
            nicknameText.text = "";
            readyText.text = "";
            hostText.text = "";

            root.SetActive(false);
        }
    }
}
