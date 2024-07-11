using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class Login : MonoBehaviour
{
    private const string PASSWORD_REGEX = "(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.{8,24})";

    private string loginEndpoint = "http://127.0.0.1:13756/account/login";
    private string createEndpoint = "http://127.0.0.1:13756/account/create";

    [SerializeField] private TextMeshProUGUI alertText;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button createButton;
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField passwordInputField;

    public void OnLoginClick()
    {
        alertText.text = "Signing in...";
        ActivateButtons(false);
        Debug.Log("login clicked");
        StartCoroutine(TryLogin());
    }

    public void OnCreateClick()
    {
        alertText.text = "Creating account...";
        ActivateButtons(false);

        StartCoroutine(TryCreate());
    }

    private IEnumerator TryLogin()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;

        if (username.Length < 3 || username.Length > 24)
        {
            alertText.text = "Invalid username";
            Debug.Log("bad username ");
            ActivateButtons(true);
            yield break;
        }

        // if (!Regex.IsMatch(password, PASSWORD_REGEX))
        // {
        //     alertText.text = "Invalid credentials";
        //     Debug.Log("regex is here ");
        //     ActivateButtons(true);
        //     yield break;
        // }

        WWWForm form = new WWWForm();
        form.AddField("rUsername", username);
        form.AddField("rPassword", password);

        UnityWebRequest request = UnityWebRequest.Post(loginEndpoint, form);
        Debug.Log("Login Request sent to: " + loginEndpoint);
        Debug.Log(request);
        var handler = request.SendWebRequest();

        float startTime = 0.0f;
        while (!handler.isDone)
        {
            startTime += Time.deltaTime;

            if (startTime > 10.0f)
            {
                break;
            }

            yield return null;
        }

        if (request.result == UnityWebRequest.Result.Success)
        {
            LoginResponse response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);

            if (response.code == 0) // login success?
            {
                // 여기서 추가적인 처리하면됨
                
                ActivateButtons(false);
                alertText.text = "Welcome " + ((response.data.adminFlag == 1) ? " Admin" : "");
            }
            else
            {
                switch (response.code)
                {
                    case 1:
                        alertText.text = "Invalid credentials";
                        ActivateButtons(true);
                        break;
                    default:
                        alertText.text = "Corruption detected";
                        ActivateButtons(false);
                        break;
                }
            }
        }
        else
        {
            alertText.text = "Error connecting to the server...";
            ActivateButtons(true);
        }


        yield return null;
    }

    private IEnumerator TryCreate()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;

        if (username.Length < 3 || username.Length > 24)
        {
            alertText.text = "Invalid username";
            ActivateButtons(true);
            yield break;
        }

        // if (!Regex.IsMatch(password, PASSWORD_REGEX))
        // {
        //     alertText.text = "Invalid credentials";
        //     ActivateButtons(true);
        //     yield break;
        // }

        WWWForm form = new WWWForm();
        form.AddField("rUsername", username);
        form.AddField("rPassword", password);

        UnityWebRequest request = UnityWebRequest.Post(createEndpoint, form);
        Debug.Log("Create Request sent to: " + loginEndpoint);
        Debug.Log(request);
        var handler = request.SendWebRequest();

        float startTime = 0.0f;
        while (!handler.isDone)
        {
            startTime += Time.deltaTime;

            if (startTime > 10.0f)
            {
                break;
            }

            yield return null;
        }

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log(request.downloadHandler.text);
            CreateResponse response = JsonUtility.FromJson<CreateResponse>(request.downloadHandler.text);

            if (response.code == 0) 
            {
                alertText.text = "Account has been created!";
            }
            else
            {
                switch (response.code)
                {
                    case 1:
                        alertText.text = "Invalid credentials";
                        break;
                    case 2:
                        alertText.text = "Username is already taken";
                        break;
                    case 3:
                        alertText.text = "Password is unsafe";
                        break;
                    default:
                        alertText.text = "Corruption detected";
                        break;

                }
            }
        }
        else
        {
            Debug.LogError("Request failed. Error: " + request.error);
            Debug.LogError("Response code: " + request.responseCode);
            alertText.text = "Error connecting to the server...";
        }

        ActivateButtons(true);

        yield return null;
    }

    private void ActivateButtons(bool toggle)
    {
        loginButton.interactable = toggle;
        createButton.interactable = toggle;
    }
}
