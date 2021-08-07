using UnityEngine;
using UnityEngine.SceneManagement;

//  RequireComponent ? Attribute
//  typeof(Ŭ���� ������Ʈ �̸�) �� ������ �ڵ����� �߰����ش�
[RequireComponent(typeof(PawnAnimation))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float m_MoveSpeed; //  ĳ���� �̵��ӵ�
    [SerializeField] private float m_JumpSpeed; //  ĳ���� ���� ����ġ
    [SerializeField] private Collider2D m_GroundCollider;   //  ĳ���� �ٴ� ���� �ݶ��̴�
    private PawnAnimation m_PawnAnimation;      //  ĳ���� �ִϸ��̼� ������Ʈ
    private float m_JumpPower = 0;              //  ĳ���� ���� ���� �� (Progress bar)
    private Rigidbody2D m_Rigidbody;            //  ĳ���� ���� ��� ������Ʈ

    public bool IsLeft { get; private set; } = false;       //  ĳ���� ������ �����ΰ� ?
    public bool IsMoveLock { get; private set; } = false;   //  ĳ���Ͱ� ������ �� �ִ°� ?
    public int SaveLevel { get; set; } = 0;                 //  ĳ���� ���̺� ����

    private void Awake()
    {
        //  GetComponent ?
        //  �� ��ü�� ������ �ִ� ������Ʈ �߿� �ش� <Ÿ��> ������Ʈ�� �����´�
        //  ���� ���ٸ� null �� ��ȯ�Ѵ�
        m_PawnAnimation = GetComponent<PawnAnimation>();
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            //  Path -> IO stream �����ִ� ��ƿ Ŭ����
            //  Path.GetFileName -> ���� �̸� + Ȯ���ڸ� �����´�
            //  ex) Assets/Data/Material.mat -> return : Material.mat
            FileManager.Get.Load(
                System.IO.Path.GetFileName(FileManager.Get.GetSaveFilePath),
                this);
        }

        if (m_PawnAnimation.Jump) return;
        if (Input.GetKey(KeyCode.Space))
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                IsMoveLock = true;
                m_PawnAnimation.Move = false;
            }

            m_JumpPower += Time.deltaTime;
            if (m_JumpPower >= 1f) m_JumpPower = 1f;
        }
        else if (Input.GetKeyUp(KeyCode.Space) && m_JumpPower != 0f)
        {
            //  Velocity ? 
            //  Rigidbody �ȿ� �ִ� �� (����ġ) �� (x, y, z)
            //  ĳ���Ͱ� ������ �ִ� ���� ����ġ ���� ��� �ʱ�ȭ���ش�
            m_Rigidbody.velocity = Vector3.zero;

            //  LeftArrow �� �����ְų� RightArrow �� �����ִٸ� true, �ƴϸ� false;
            bool isDirection = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow);
            Vector3 axis = new Vector3(isDirection ? transform.right.x * 0.25f : 0, 1, 0);
            //  �� �ڵ� Ǯ��� ..
            //  ���� isDirection �� �� �̸�
            //  ĳ������ right (�� ���� �� ��).x �����ٰ� ���� ���� 0.5f ��ŭ ���Ѵ�
            //  (������ ������ ���� �����θ� �����ϸ� 1 ���� ū ���̱� ������ ������ �ʹ� ���� ����)
            //  �ƴϸ� 0. ���θ� �����ϰ� �Ѵ�
            //Vector3 axis = new Vector3(0, 1, 0);
            //if (isDirection) axis.x = transform.right.x * 0.5f;

            //  ���� ���� ���� ����ġ �߰� (1f + m_JumpSpeed * m_JumpPower);
            m_JumpPower += 1f;
            //  ĳ���Ϳ��� ���� �����ش�
            m_Rigidbody.AddForce(axis * m_JumpSpeed * m_JumpPower, ForceMode2D.Impulse);

            //  ���� ���� �ʱ�ȭ
            m_JumpPower = 0;
            IsMoveLock = false;

            //  ���� �ִϸ��̼� ����
            m_PawnAnimation.Jump = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("TitleScene");
           // Application.Quit();
        }
    }

    private void FixedUpdate()
    {
        if (!m_PawnAnimation.Jump) OnMove();

        //  ĳ���Ͱ� �ٴڿ� ��Ҵ��� �˻��Ѵ�
        if (!m_GroundCollider.enabled)
        {
            //  ���� ĳ���Ͱ� ���� ���̸� �ݶ��̴� ���ش�
            if (m_PawnAnimation.Jump) m_GroundCollider.enabled = true;
            //  ĳ���Ͱ� �������� �ʾҴµ� �������� �ִٸ� ������ ���� �� ���� �ݶ��̴� ���ش�
            else if (m_Rigidbody.velocity.y < -0.1f)
            {
                m_PawnAnimation.Jump = true;
                m_GroundCollider.enabled = true;
            }
        }
    }

    private void OnMove()
    {
        if (IsMoveLock) return;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (!IsLeft)
            {
                transform.Rotate(Vector3.up * 180f);
                m_Rigidbody.velocity = Vector3.zero;
                IsLeft = true;
            }

            Vector3 speed = Vector3.right * m_MoveSpeed * Time.deltaTime;
            transform.Translate(speed);

            m_PawnAnimation.Move = true;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            if (IsLeft)
            {
                transform.Rotate(Vector3.up * 180f);
                m_Rigidbody.velocity = Vector3.zero;
                IsLeft = false;
            }

            Vector3 speed = Vector3.right * m_MoveSpeed * Time.deltaTime;
            transform.Translate(speed);

            m_PawnAnimation.Move = true;
        }
        else
            m_PawnAnimation.Move = false;
    }

    //  Collider2D collision �Ķ���ʹ� �ش� �Լ��� ȣ��� �� �浹�� ����� ���´�
    //  Ʈ���� �ݶ��̴��� ������ �� �� ���� ȣ��ȴ�
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Grounds")) return;

        //  ������ ����� �� (�Ǵ� ���� ���ϰ� ������ ��) ���� ���� ���̳ʽ��� �Ǹ� (�������� ���� ��)
        if (m_Rigidbody.velocity.y < 0f)
        {
            m_PawnAnimation.Jump = false;
            m_Rigidbody.velocity = Vector3.zero;
            m_GroundCollider.enabled = false;       //  ���� üũ�� �ݶ��̴��� ����
        }

    }

    //  Ʈ���� �ݶ��̴��� ������ �� ��� ȣ��ȴ�
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Grounds"))
        {
            m_PawnAnimation.Jump = false;
            m_Rigidbody.velocity = Vector3.zero;
            m_GroundCollider.enabled = false;
        }
    }

}
