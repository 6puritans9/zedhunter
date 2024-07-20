using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
    {
        private const string PASSWORD_REGEX = "(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.{8,24})";

        private string loginEndpoint = "http://52.78.204.71:13756/account/login";
        private string createEndpoint = "http://52.78.204.71:13756/account/create";

        [SerializeField] private TextMeshProUGUI loginAlertText;
        [SerializeField] private TextMeshProUGUI createAlertText;
        [SerializeField] private Button loginButton;
        [SerializeField] private Button createButton;
        [SerializeField] private TMP_InputField loginIDInputField;
        [SerializeField] private TMP_InputField loginPWInputField;
        [SerializeField] private TMP_InputField createIDInputField;
        [SerializeField] private TMP_InputField createPWInputField;
        [SerializeField] private TMP_InputField createConfirmInputField;
        [SerializeField] private Color successColor;
        public ConnectToServer serverConnection;
        

        public void OnLoginClick()
            {
                ActivateButtons(false);
                StartCoroutine(TryLogin());
            }

        public void OnCreateClick()
            {
                ActivateButtons(false);

                StartCoroutine(TryCreate());
            }

        private IEnumerator TryLogin()
            {
                string username = loginIDInputField.text;
                string password = loginPWInputField.text;

                if (username.Length < 3 || username.Length > 14)
                    {
                        loginAlertText.text = "Invalid username";
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
                                // loginAlertText.text = "Welcome " + ((response.data.adminFlag == 1) ? " Admin" : "");
                                loginAlertText.text = "Welcome";
                                loginAlertText.color = successColor;
                                
                                #region PhotonLogin
                                
                                serverConnection.OnClickConnect(loginIDInputField.text);
                                
                                #endregion

                            }
                        else
                            {
                                switch (response.code)
                                    {
                                        case 1:
                                            loginAlertText.text = "Invalid credentials";
                                            ActivateButtons(true);
                                            break;
                                        default:
                                            loginAlertText.text = "Corruption detected";
                                            ActivateButtons(false);
                                            break;
                                    }
                            }
                    }
                else
                    {
                        loginAlertText.text = "Error connecting to the server...";
                        ActivateButtons(true);
                    }


                yield return null;
            }

        private IEnumerator TryCreate()
            {
                string username = createIDInputField.text;
                string password = createPWInputField.text;

                if (username.Length < 3 || username.Length > 24)
                    {
                        createAlertText.text = "Invalid ID";
                        ActivateButtons(true);
                        yield break;
                    }
                else if (createPWInputField.text != createConfirmInputField.text)
                    {
                        createAlertText.text = "Password Does not Match";
                        ActivateButtons(true);
                        yield break;
                    }

                // if (!Regex.IsMatch(password, PASSWORD_REGEX))
                //     {
                //         createAlertText.text = "Invalid credentials";
                //         ActivateButtons(true);
                //         yield break;
                //     }

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
                                
                                
                                
                                createAlertText.text = "Account has been created!";
                                createAlertText.color = successColor;
                            }
                        else
                            {
                                switch (response.code)
                                    {
                                        case 1:
                                            createAlertText.text = "Invalid credentials";
                                            break;
                                        case 2:
                                            createAlertText.text = "Username is already taken";
                                            break;
                                        case 3:
                                            createAlertText.text = "Password is unsafe";
                                            break;
                                        default:
                                            createAlertText.text = "Corruption detected";
                                            break;
                                    }
                            }
                    }
                else
                    {
                        Debug.LogError("Request failed. Error: " + request.error);
                        Debug.LogError("Response code: " + request.responseCode);
                        createAlertText.text = "Error connecting to the server...";
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