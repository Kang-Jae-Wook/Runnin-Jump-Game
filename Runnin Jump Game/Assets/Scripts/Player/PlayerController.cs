using UnityEngine;
using UnityEngine.SceneManagement;

//  RequireComponent ? Attribute
//  typeof(클래스 컴포넌트 이름) 가 없으면 자동으로 추가해준다
[RequireComponent(typeof(PawnAnimation))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float m_MoveSpeed; //  캐릭터 이동속도
    [SerializeField] private float m_JumpSpeed; //  캐릭터 점프 가중치
    [SerializeField] private Collider2D m_GroundCollider;   //  캐릭터 바닥 검출 콜라이더
    private PawnAnimation m_PawnAnimation;      //  캐릭터 애니메이션 컴포넌트
    private float m_JumpPower = 0;              //  캐릭터 점프 물리 힘 (Progress bar)
    private Rigidbody2D m_Rigidbody;            //  캐릭터 물리 담당 컴포넌트

    public bool IsLeft { get; private set; } = false;       //  캐릭터 방향이 왼쪽인가 ?
    public bool IsMoveLock { get; private set; } = false;   //  캐릭터가 움직일 수 있는가 ?
    public int SaveLevel { get; set; } = 0;                 //  캐릭터 세이브 레벨

    private void Awake()
    {
        //  GetComponent ?
        //  이 객체가 가지고 있는 컴포넌트 중에 해당 <타입> 컴포넌트를 가져온다
        //  만약 없다면 null 을 반환한다
        m_PawnAnimation = GetComponent<PawnAnimation>();
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            //  Path -> IO stream 도와주는 유틸 클래스
            //  Path.GetFileName -> 파일 이름 + 확장자를 가져온다
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
            //  Rigidbody 안에 있는 힘 (가중치) 값 (x, y, z)
            //  캐릭터가 가지고 있는 힘의 가중치 값을 모두 초기화해준다
            m_Rigidbody.velocity = Vector3.zero;

            //  LeftArrow 가 눌려있거나 RightArrow 가 눌려있다면 true, 아니면 false;
            bool isDirection = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow);
            Vector3 axis = new Vector3(isDirection ? transform.right.x * 0.25f : 0, 1, 0);
            //  위 코드 풀어보면 ..
            //  만약 isDirection 이 참 이면
            //  캐릭터의 right (앞 방향 의 축).x 값에다가 보정 값인 0.5f 만큼 곱한다
            //  (나누는 이유는 앞의 축으로만 점프하면 1 보다 큰 값이기 때문에 앞으로 너무 많이 간다)
            //  아니면 0. 위로만 점프하게 한다
            //Vector3 axis = new Vector3(0, 1, 0);
            //if (isDirection) axis.x = transform.right.x * 0.5f;

            //  점프 힘에 대한 가중치 추가 (1f + m_JumpSpeed * m_JumpPower);
            m_JumpPower += 1f;
            //  캐릭터에게 힘을 가해준다
            m_Rigidbody.AddForce(axis * m_JumpSpeed * m_JumpPower, ForceMode2D.Impulse);

            //  점프 값들 초기화
            m_JumpPower = 0;
            IsMoveLock = false;

            //  점프 애니메이션 실행
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

        //  캐릭터가 바닥에 닿았는지 검사한다
        if (!m_GroundCollider.enabled)
        {
            //  만약 캐릭터가 점프 중이면 콜라이더 켜준다
            if (m_PawnAnimation.Jump) m_GroundCollider.enabled = true;
            //  캐릭터가 점프하지 않았는데 떨어지고 있다면 강제로 점프 및 점프 콜라이더 켜준다
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

    //  Collider2D collision 파라메터는 해당 함수가 호출될 때 충돌한 대상이 들어온다
    //  트리거 콜라이더가 들어왔을 때 한 번만 호출된다
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Grounds")) return;

        //  점프가 진행될 때 (또는 점프 안하고 떨어질 때) 점프 값이 마이너스가 되면 (떨어지고 있을 때)
        if (m_Rigidbody.velocity.y < 0f)
        {
            m_PawnAnimation.Jump = false;
            m_Rigidbody.velocity = Vector3.zero;
            m_GroundCollider.enabled = false;       //  점프 체크용 콜라이더를 끈다
        }

    }

    //  트리거 콜라이더가 들어왔을 때 계속 호출된다
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
