using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FireBaseTest : MonoBehaviour {
    protected Firebase.Auth.FirebaseAuth auth;
    protected Firebase.Auth.FirebaseAuth otherAuth;
    protected Dictionary<string, Firebase.Auth.FirebaseUser> userByAuth =
      new Dictionary<string, Firebase.Auth.FirebaseUser>();
    UserInfo userInfo;
    Text txtMsg;
    Text txtUuid;
    // Use this for initialization
    void Start () {
        //InitializeFirebase();
        FirebaseHelper.getInstance().initFirebase();
        txtMsg=GameObject.Find("/Canvas/txtMsg").GetComponent<Text>();
        txtUuid = GameObject.Find("/Canvas/txtUuid").GetComponent<Text>();
        txtUuid.text =FirebaseHelper.getInstance().CheckCurrentAuth();
        if (FirebaseHelper.getInstance().CheckCurrentAuth() != null)
        {
            FirebaseHelper.getInstance().GetCurrentUserInfo(gotCurrentUser);
        }

    }
	void gotCurrentUser(UserInfo user)
    {
        txtUuid.text = user.uid;
        txtMsg.text = user.share_code;
        this.userInfo = user;
    }
	// Update is called once per frame
	void Update () {
		
	}

    protected void InitializeFirebase()
    {
        DebugLog("Setting up Firebase Auth");
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        
        
    }
    public void LoginAnonymous()
    {
        FirebaseHelper.getInstance().LoginAsAnonymousUser(onLogedIn);
    }
    public void onLogedIn(UserInfo userInfo)
    {
        Debug.Log("Logged in "+ userInfo.uid);
        this.userInfo = userInfo;
        txtMsg.text = userInfo.uid;
    
    }
    public void LoginAnonymous_1()
    {
        auth.SignInAnonymouslyAsync().ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });
    }
    public void CreateUser()
    {
     

        userInfo.share_code = userInfo.share_code + "-"+Random.Range(0, 10);
        FirebaseHelper.getInstance().CreateNewUser(userInfo);
    }
    // Track state changes of the auth object.
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        Firebase.Auth.FirebaseAuth senderAuth = sender as Firebase.Auth.FirebaseAuth;
        Firebase.Auth.FirebaseUser user = null;
        if (senderAuth != null) userByAuth.TryGetValue(senderAuth.App.Name, out user);
        if (senderAuth == auth && senderAuth.CurrentUser != user)
        {
            bool signedIn = user != senderAuth.CurrentUser && senderAuth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                DebugLog("Signed out " + user.UserId);
            }
            user = senderAuth.CurrentUser;
            userByAuth[senderAuth.App.Name] = user;
            if (signedIn)
            {
                DebugLog("Signed in " + user.UserId); 
            }
        }
    }
 

    // Output text to the debug log text field, as well as the console.
    public void DebugLog(string s)
    {
        Debug.Log(s);

    }
}
