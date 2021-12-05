using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SignUp : MonoBehaviour
{
    public static SignUp Instance { set; get; }
    
    private void Awake() {
         
        Instance = this;
    }

    public string Result;
    public IEnumerator CreateAccount(string user, string email, string password) {
        
        WWWForm form = new WWWForm();
        form.AddField("username", user);
        form.AddField("email", email);
        form.AddField("password", password);
        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/TeamProject/Register.php", form)) {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.ConnectionError) {

                Debug.Log(www.error);
            }
            else {
                //Debug.Log(www.downloadHandler.text);
                Result = www.downloadHandler.text;
                if (Result.Equals("1")) {
                    UI.Instance.NotificationText.text = "New Account created";
                    UI.Instance.Notification.SetActive(true);
                    Debug.Log("New Account created");
                }
                else if (Result.Equals("2")) {
                    UI.Instance.NotificationText.text = "Account already exists";
                    UI.Instance.Notification.SetActive(true);
                    Debug.Log("Account already exists");
                }
                else {
                    Debug.Log(www.downloadHandler.text);                
                }
            }
        }
    }

    public IEnumerator LogIn(string user, string password) {
        WWWForm form = new WWWForm();
        form.AddField("username", user);    
        form.AddField("password", password);
        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/TeamProject/Login.php", form)) {
            yield return www.SendWebRequest();
 
            if (www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.ConnectionError) {

                Debug.Log(www.error);
            }
            else {
 
               // Debug.Log(www.downloadHandler.text);
                Result = www.downloadHandler.text;
                if (Result.Equals("1")) {
                    UI.Instance.startMenu.SetActive(false);
                    UI.Instance.OnlineMenu.SetActive(true);
                    StartCoroutine(RetrieveRank(user));
                    StartCoroutine(RetrieveExperience(user));
                    

                }
                else if (Result.Equals("2")) {
                    UI.Instance.NotificationText.text = "incorrect password";
                    UI.Instance.Notification.SetActive(true);
                    Debug.Log("incorrect password");
                   
                }
                else if (Result.Equals("3")) {
                    UI.Instance.NotificationText.text = "Account not found";
                    UI.Instance.Notification.SetActive(true);
                    Debug.Log("Account not found");
             
                }
            }
        }
    }

    public IEnumerator RetrieveRank(string user) {

        WWWForm form = new WWWForm();
        form.AddField("username", user);

        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/TeamProject/RetrieveRank.php", form)) {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.ConnectionError) {

                Debug.Log(www.error);
            }
            else {
                Debug.Log(www.downloadHandler.text);
                Result = www.downloadHandler.text;
                UI.Instance.DisplayUsername.text = user;
                UI.Instance.DisplayRank.text = Result;
                
            }
        }
    }

    public IEnumerator RetrieveExperience(string user) {

        WWWForm form = new WWWForm();
        form.AddField("username", user);

        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/TeamProject/RetrieveExperience.php", form)) {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.ConnectionError) {

                Debug.Log(www.error);
            }
            else {
                Debug.Log(www.downloadHandler.text);
                Result = www.downloadHandler.text;
                UI.Instance.DisplayExperience.text = Result;

            }
        }
    }

}
