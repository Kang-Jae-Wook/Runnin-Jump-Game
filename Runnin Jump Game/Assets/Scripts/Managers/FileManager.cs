using System.IO;
using UnityEngine;

public class FileManager : MonoBehaviour
{
    //  Singleton ? -> 디자인 패턴
    //  개체를 하나만 존재하게 하는 방법
    private static FileManager m_Instance = null;
    public static FileManager Get
    {
        get
        {
            //  만약 생성된 객체가 없다면
            if (m_Instance == null)
            {
                //  오브젝트를 만들어주고 그 안에 FileManager 컴포넌트를 넣어준다
                m_Instance = new GameObject("File Manager").AddComponent<FileManager>();

                //  씬을 넘어가도 부셔지지 않도록 설정
                DontDestroyOnLoad(m_Instance);
            }

            return m_Instance;
        }
    }

    public string GetDirectory => $"{Application.persistentDataPath}/PlayerData";
    public string GetSaveFilePath => $"{GetDirectory}/data.fc";

    public void Save(Vector3 position, int level)
    {
        //  폴더 경로 3가지
        //  Company name 과 Product name 은 File -> Build Settings -> Player 탭 에서 볼 수 있다!
        //  Application.persistentDataPath -> %Appdata%\..\LocalLow\(Company name)\(Product Name)
        //  ex) 현재 프로젝트 경로라면 -> %Appdata%\..\LocalLow\DefaultCompany\Sample-RPG

        //  Application.dataPath -> Project directory\Assets
        //  ex) D:\KBKIM\Sample-RPG-Game\Assets

        //  Application.streamingAssetsPath;
        //  Project directory\Assets\StreamingAssets
        //  폴더는 내가 직접 만들어야함 ..
        //  특수한 폴더 (영상 등등 저장할 때 ..)

        //  폴더가 있으면 넘어가고 없으면 폴더를 생성한다
        string directory = GetDirectory;
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        //  Extension. 확장자
        //  ex) Sample.txt -> Sample text 파일
        //  말 그대로 뒤에 이름을 붙이는 확장자 이름
        //  큰 의미가 없다! 내가 텍스트형식을 저장한다 해서 txt 를 붙일 필요는 없다
        string path = GetSaveFilePath;
        using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.UTF8))
        {
            sw.WriteLine(level);
            sw.WriteLine($"{position.x},{position.y}");
        }
    }

    public bool Load(string pathName, PlayerController player)
    {
        string fullPath = $"{GetDirectory}/{pathName}";

        //  파일 검사. 있으면 true, 없으면 false
        if (!File.Exists(fullPath)) return false;

        using (StreamReader sr = new StreamReader(fullPath))
        {
            string str = sr.ReadLine();
            player.SaveLevel = int.Parse(str);

            str = sr.ReadLine();
            if (!string.IsNullOrEmpty(str))
            {
                //  해당 스트링을 나눈다. 기준은 괄호 안에 있는 문자 기준
                //  ex) str = "1.0,-2.0";
                //  ex-return) splits = new string[] { "1.0", "-2.0" };
                string[] splits = str.Split(',');

                //  문자를 숫자로 변경 (float.Parse())
                Vector3 position = new Vector3(float.Parse(splits[0]), float.Parse(splits[1]));
                player.transform.position = position;
            }
        }
        return true;
    }

}
