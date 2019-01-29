using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBaseUserHelper : MonoBehaviour {
    public System.Action callbackWhenInitDone;
    public UserInfo userInfo;
    // Use this for initialization
  
    void Start () {
        FirebaseHelper.getInstance().initFirebase();
        if (FirebaseHelper.getInstance().CheckCurrentAuth() != null)
        {
            FirebaseHelper.getInstance().GetCurrentUserInfo(OnCurrentUser);
        }
        else
        {

            FirebaseHelper.getInstance().LoginAsAnonymousUser(OnLoggedInAsAnonymousUserDone);
        }
    }
    void OnCurrentUser(UserInfo user)
    {
        this.userInfo = user;
        OnLoggedInAsAnonymousUserDone(user);
    }
    void OnLoggedInAsAnonymousUserDone(UserInfo userInfo)
    {
        Debug.Log("OnLoggedInAsAnonymousUserDone");
        this.userInfo = userInfo;
        callbackWhenInitDone();
    }
    public void UpDateUserInfo()
    {
        FirebaseHelper.getInstance().UpdateUser(userInfo);
    }
    // Update is called once per frame
    void Update () {
		
	}
}
