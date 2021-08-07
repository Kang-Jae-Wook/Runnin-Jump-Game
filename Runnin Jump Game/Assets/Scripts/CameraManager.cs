using UnityEngine;

public class CameraManager : MonoBehaviour
{
    internal struct CameraRect
    {
        public float Left;
        public float Top;
        public float Right;
        public float Bottom;
    }

    public Camera MainCamera;   //  메인 카메라
    [SerializeField] private Transform m_PlayerTransform;   //  캐릭터 포지션 가져올 트랜스폼 컴포넌트

    private float m_Height = 0;     //  카메라의 길이
    private float m_Width = 0;      //  카메라의 높이
    private CameraRect m_Rect;      //  카메라 렉트

    private void Awake()
    {
        m_Height = MainCamera.orthographicSize * 2f;
        m_Width = m_Height * MainCamera.aspect;     //  m_Height * (1920 / 1080) <- 1920, 1080 해상도 기준

        float halfWidth = m_Width * 0.5f;
        float halfHeight = m_Height * 0.5f;

        m_Rect = new CameraRect()
        {
            Left = -halfWidth,
            Right = +halfWidth,
            Top = +halfHeight,
            Bottom = -halfHeight
        };
    }

    //  Update 다음으로 호출되는 LateUpdate
    //  왜 ?
    //  캐릭터 업데이트 다음에 호출하려고!
    private void LateUpdate()
    {
        var playerPosition = m_PlayerTransform.position;        //  플레이어 포지션
        var position = transform.position;                      //  카메라 포지션

        //  렉트 검사!!
        //  캐릭터가 렉트 안에서 벗어날 경우 카메라를 이동시켜준다
        //  플레이어가 카메라의 오른쪽 범위에서 벗어난 경우
        if (playerPosition.x > m_Rect.Right)
        {
            transform.position = new Vector3(position.x + m_Width, position.y, 0);
            m_Rect.Left += m_Width;
            m_Rect.Right += m_Width;
        }
        else if (playerPosition.x < m_Rect.Left)
        {
            transform.position = new Vector3(position.x - m_Width, position.y, 0);
            m_Rect.Left -= m_Width;
            m_Rect.Right -= m_Width;
        }

        //  실습겸 숙제 1
        //  카메라 위 아래 움직이게 하기
        if (playerPosition.y > m_Rect.Top)
        {
            transform.position = new Vector3(position.x, position.y + m_Height, 0);
            m_Rect.Top += m_Height;
            m_Rect.Bottom += m_Height;
        }
        else if (playerPosition.y < m_Rect.Bottom)
        {
            transform.position = new Vector3(position.x, position.y - m_Height, 0);
            m_Rect.Top -= m_Height;
            m_Rect.Bottom -= m_Height;
        }

    }
}
