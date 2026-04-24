using UnityEngine;
using Photon.Pun;

namespace Tanks.Complete
{
    public class ShellExplosionOnline : MonoBehaviourPun
    {
        public LayerMask m_TankMask;
        public ParticleSystem m_ExplosionParticles;
        public AudioSource m_ExplosionAudio;
        [HideInInspector] public float m_MaxLifeTime = 2f;

        [HideInInspector] public float m_MaxDamage = 100f;
        [HideInInspector] public float m_ExplosionForce = 50f;
        [HideInInspector] public float m_ExplosionRadius = 5f;

        private int ownerId;
        private string ownerName;

        public void Init(int id, string name)
        {
            ownerId = id;
            ownerName = name;
        }

        private void Start()
        {
            if (PhotonNetwork.InRoom)
            {
                if (photonView.IsMine)
                    Invoke(nameof(DestroyShell), m_MaxLifeTime);
            }
            else
            {
                Destroy(gameObject, m_MaxLifeTime);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (PhotonNetwork.InRoom && !photonView.IsMine)
                return;

            Explode();
        }

        private void Explode()
        {
            Collider[] colliders = Physics.OverlapSphere(
                transform.position,
                m_ExplosionRadius,
                m_TankMask
            );

            for (int i = 0; i < colliders.Length; i++)
            {
                Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

                if (!targetRigidbody)
                    continue;

                TankMovement movement = targetRigidbody.GetComponent<TankMovement>();

                if (movement != null)
                {
                    movement.AddExplosionForce(
                        m_ExplosionForce,
                        transform.position,
                        m_ExplosionRadius
                    );
                }

                TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();

                if (!targetHealth)
                    continue;

                float damage = CalculateDamage(targetRigidbody.position);
                targetHealth.TakeDamage(damage, ownerId, ownerName);
            }

            photonView.RPC(nameof(RPC_PlayExplosion), RpcTarget.All, transform.position);

            if (PhotonNetwork.InRoom)
                PhotonNetwork.Destroy(gameObject);
            else
                Destroy(gameObject);
        }

        [PunRPC]
        private void RPC_PlayExplosion(Vector3 position)
        {
            m_ExplosionParticles.transform.parent = null;
            m_ExplosionParticles.transform.position = position;

            m_ExplosionParticles.Play();

            if (m_ExplosionAudio != null)
                m_ExplosionAudio.Play();

            ParticleSystem.MainModule mainModule = m_ExplosionParticles.main;
            Destroy(m_ExplosionParticles.gameObject, mainModule.duration);
        }

        private void DestroyShell()
        {
            if (PhotonNetwork.InRoom)
                PhotonNetwork.Destroy(gameObject);
            else
                Destroy(gameObject);
        }

        private float CalculateDamage(Vector3 targetPosition)
        {
            Vector3 explosionToTarget = targetPosition - transform.position;
            float explosionDistance = explosionToTarget.magnitude;
            float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;
            float damage = relativeDistance * m_MaxDamage;

            return Mathf.Max(0f, damage);
        }
    }

}
