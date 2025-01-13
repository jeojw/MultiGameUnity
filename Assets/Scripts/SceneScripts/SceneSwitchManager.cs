using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitchManager : MonoBehaviour
{
    public void LoadToSignin()
    {
        SceneManager.LoadScene("SigninScene");
    }

    public void LoadToSignup()
    {
        SceneManager.LoadScene("SignupScene");
    }

    public void LoadToFirst()
    {
        SceneManager.LoadScene("FirstScene");
    }
}
