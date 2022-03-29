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
        
        WWWForm form = new WWWForm();    //all three post forms
        form.AddField("username", user);
        form.AddField("email", email);
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post("https://xenocidex.herokuapp.com/Register.php", form)) {  //location of php files 
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.ConnectionError) {

                Debug.Log(www.error);  // incase of connection error 
            }
            else {

                Result = www.downloadHandler.text;       //php echos are saved in "www.downloadHandler.text"
                if (Result.Equals("1")) {
                    UI.Instance.NotificationText.text = "New account created, you can now log in";  // if php echos 1
                    UI.Instance.Notification.SetActive(true);
                    Debug.Log("New Account created, you can now log in");
                }

                else if (Result.Equals("2")) {
                    UI.Instance.NotificationText.text = "Account already exists for this email";  //if php echos 2
                    UI.Instance.Notification.SetActive(true);
                    Debug.Log("Account already exists for this email");

                }

                else if (Result.Equals("3")) {
                    UI.Instance.NotificationText.text = "Username is already taken";  //if php echos 3
                    UI.Instance.Notification.SetActive(true);
                    Debug.Log("Username is already taken");

                }
                
                else if (Result.Equals("4")) {
                    UI.Instance.NotificationText.text = "Password must be at least 8 characters long";  //if php echos 4
                    UI.Instance.Notification.SetActive(true);
                    Debug.Log("Password must be at least 8 characters long");

                }

               else if (Result.Equals("5")) {
                    UI.Instance.NotificationText.text = "Provide a valid email address";  //if php echos 5
                    UI.Instance.Notification.SetActive(true);
                    Debug.Log("Provide a valid email address");

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
        using (UnityWebRequest www = UnityWebRequest.Post("https://xenocidex.herokuapp.com/Login.php", form)) {
            yield return www.SendWebRequest();
 
            if (www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.ConnectionError) {

               StartCoroutine( LogIn(user,  password));
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
                    UI.Instance.NotificationText.text = "Incorrect password";
                    UI.Instance.Notification.SetActive(true);
                    Debug.Log("Incorrect password");
                   
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

        using (UnityWebRequest www = UnityWebRequest.Post("https://xenocidex.herokuapp.com/RetrieveRank.php", form)) {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.ConnectionError) {

                StartCoroutine(RetrieveRank(user));
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

        using (UnityWebRequest www = UnityWebRequest.Post("https://xenocidex.herokuapp.com/RetrieveExperience.php", form)) {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.ConnectionError) {

                StartCoroutine(RetrieveExperience(user));
            }
            else {
                Debug.Log(www.downloadHandler.text);
                Result = www.downloadHandler.text;
                UI.Instance.DisplayExperience.text = Result;

            }
        }
    }

}