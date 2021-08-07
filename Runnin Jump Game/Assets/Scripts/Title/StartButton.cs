using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


class StartButton : MonoBehaviour
{
    public void SceneChange()
    {
        SceneManager.LoadScene("InGameScene");

    }

}
