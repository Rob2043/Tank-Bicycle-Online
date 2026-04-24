using UnityEngine;
using UnityEngine.UI;
using CustomEventBus;
using TankBycicleOnline.CallBacks;
using TankBycicleOnline.Constants;
using Photon.Pun;

namespace Tanks.Complete
{
    public class TankHealth : MonoBehaviourPun
    {
        public float m_StartingHealth = 100f;
        public Slider m_Slider;
        public Image m_FillImage;
        public Color m_FullHealthColor = Color.green;
        public Color m_ZeroHealthColor = Color.red;
        public GameObject m_ExplosionPrefab;
        [HideInInspector] public bool m_HasShield;

        private AudioSource m_ExplosionAudio;
        private ParticleSystem m_ExplosionParticles;

        private float m_CurrentHealth;
        private bool m_Dead;
        private float m_ShieldValue;
        private bool m_IsInvincible;

        private EventBus eventBus;
        private ITankId myTankId;

        private void Start()
        {
            eventBus = ServiceLocator.Current.Get<EventBus>();
            myTankId = GetComponent<ITankId>();
            if (PhotonNetwork.InRoom)
                Init();
            ResetHealth();
        }

        public void Init()
        {
            m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
            m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();
            m_ExplosionParticles.gameObject.SetActive(false);

            m_Slider.maxValue = m_StartingHealth;
        }

        private void OnDestroy()
        {
            if (m_ExplosionParticles != null)
                Destroy(m_ExplosionParticles.gameObject);
        }

        private void ResetHealth()
        {
            m_CurrentHealth = m_StartingHealth;
            m_Dead = false;
            m_HasShield = false;
            m_ShieldValue = 0f;
            m_IsInvincible = false;

            SetHealthUI();

            if (PhotonNetwork.InRoom)
                photonView.RPC(nameof(RPC_SetHealth), RpcTarget.Others, m_CurrentHealth);
        }

        public void TakeDamage(float amount, int attackerId, string attackerName)
        {
            // if (!PhotonNetwork.InRoom)
            //     return;
            if (m_IsInvincible || m_Dead)
                return;

            float finalDamage = amount * (1f - m_ShieldValue);
            m_CurrentHealth -= finalDamage;
            m_CurrentHealth = Mathf.Clamp(m_CurrentHealth, 0f, m_StartingHealth);

            if (eventBus != null)
            {
                eventBus.Invoke(new GiveScoreSignal(Const.DamageScore, attackerId, attackerName));
            }

            SetHealthUI();

            if (PhotonNetwork.InRoom)
                photonView.RPC(nameof(RPC_SetHealth), RpcTarget.Others, m_CurrentHealth);

            if (m_CurrentHealth <= 0f)
            {
                if (eventBus != null)
                {
                    if (!PhotonNetwork.InRoom)
                        eventBus.Invoke(new GiveScoreSignal(Const.DeathScore, attackerId, attackerName));
                    else
                        eventBus.Invoke(new GiveScoreSignalOnline(Const.DeathScore, attackerId, attackerName));
                }

                OnDeath();
            }
        }

        public void IncreaseHealth(float amount)
        {
            if (PhotonNetwork.InRoom && !photonView.IsMine)
                return;

            m_CurrentHealth += amount;
            m_CurrentHealth = Mathf.Clamp(m_CurrentHealth, 0f, m_StartingHealth);

            SetHealthUI();

            if (PhotonNetwork.InRoom)
                photonView.RPC(nameof(RPC_SetHealth), RpcTarget.Others, m_CurrentHealth);
        }

        public void ToggleShield(float shieldAmount)
        {
            if (PhotonNetwork.InRoom && !photonView.IsMine)
                return;

            m_HasShield = !m_HasShield;
            m_ShieldValue = m_HasShield ? shieldAmount : 0f;
        }

        public void ToggleInvincibility()
        {
            if (PhotonNetwork.InRoom && !photonView.IsMine)
                return;

            m_IsInvincible = !m_IsInvincible;
        }

        private void SetHealthUI()
        {
            if (m_Slider != null)
                m_Slider.value = m_CurrentHealth;

            if (m_FillImage != null)
                m_FillImage.color = Color.Lerp(
                    m_ZeroHealthColor,
                    m_FullHealthColor,
                    m_CurrentHealth / m_StartingHealth
                );
        }

        private void OnDeath()
        {
            if (m_Dead)
                return;

            m_Dead = true;

            if (PhotonNetwork.InRoom)
                photonView.RPC(nameof(RPC_PlayExplosion), RpcTarget.All, transform.position);
            else
                PlayExplosion(transform.position);

            RespawnSignal respawnSignal = new RespawnSignal(transform, myTankId);

            if (eventBus != null)
                eventBus.Invoke(respawnSignal);

            transform.position = respawnSignal.ObjectTransform.position;
            if (PhotonNetwork.InRoom)
                photonView.RPC(nameof(RPC_Respawn), RpcTarget.All, respawnSignal.ObjectTransform.position, respawnSignal.ObjectTransform.rotation);


            m_CurrentHealth = m_StartingHealth;
            m_Dead = false;

            SetHealthUI();

            if (PhotonNetwork.InRoom)
                photonView.RPC(nameof(RPC_SetHealth), RpcTarget.Others, m_CurrentHealth);
        }

        [PunRPC]
        private void RPC_SetHealth(float health)
        {
            Debug.Log("Set HEalth");
            m_CurrentHealth = health;
            Debug.Log("Health: " + m_CurrentHealth);
            SetHealthUI();
        }

        [PunRPC]
        private void RPC_PlayExplosion(Vector3 position)
        {
            PlayExplosion(position);
        }

        [PunRPC]
        public void RPC_Respawn(Vector3 pos, Quaternion rot, PhotonMessageInfo info)
        {
            Debug.Log("Respawn THIS object");
            Debug.Log("Positoion "  + pos);
            Rigidbody rb = GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.position = pos;
                rb.rotation = rot;
            }
            else
            {
                transform.position = pos;
                transform.rotation = rot;
            }
        }

        private void PlayExplosion(Vector3 position)
        {
            if (m_ExplosionParticles == null)
                return;

            m_ExplosionParticles.transform.position = position;
            m_ExplosionParticles.gameObject.SetActive(true);
            m_ExplosionParticles.Play();

            if (m_ExplosionAudio != null)
                m_ExplosionAudio.Play();
        }
    }
}