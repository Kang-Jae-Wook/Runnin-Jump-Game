using UnityEngine;

public class SavePoint : MonoBehaviour
{
    private Collider2D m_Collider;      //  BoxCollider2D Component
    private Animation m_TextAnimation;  //  Text Animation Component
    private ParticleSystem m_Particle;  //  ParticleSystem Component
    public int Level;                   //  저장 인덱스 포인트

    private void Awake()
    {
        m_Collider = GetComponent<BoxCollider2D>();
        m_TextAnimation = transform.GetComponentInChildren<Animation>();
        m_Particle = transform.GetComponentInChildren<ParticleSystem>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        var player = FindObjectOfType<PlayerController>();
        if (player.SaveLevel > Level) return;
        else
        {
            FileManager.Get.Save(transform.position, Level);

            m_TextAnimation.Play();
            m_Particle.Play();
        }

        Destroy(m_Collider);
    }

}
