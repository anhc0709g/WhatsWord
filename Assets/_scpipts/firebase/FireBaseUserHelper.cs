using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
public class FireBaseUserHelper : MonoBehaviour {
    public System.Action callbackWhenInitDone;
    public UserInfo currentUserInfo;
    // Use this for initialization
    public FirebaseHelper firebaseHelper;
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
        firebaseHelper=FirebaseHelper.getInstance();
    }
    void OnCurrentUser(UserInfo user)
    {
        this.currentUserInfo = user;
        OnLoggedInAsAnonymousUserDone(user);
    }
    public void ReloadCurrentUser(System.Action<UserInfo> callbackWhenDone)
    {
        FirebaseHelper.getInstance().GetCurrentUserInfo(user => {

            this.currentUserInfo = user;
            callbackWhenDone(user);

        });

    }

    void OnLoggedInAsAnonymousUserDone(UserInfo userInfo)
    {
        Debug.Log("OnLoggedInAsAnonymousUserDone");
        this.currentUserInfo = userInfo;
        callbackWhenInitDone();
    }
    public void UpDateUserInfo()
    {
        FirebaseHelper.getInstance().UpdateUser(currentUserInfo);
    }
    // Update is called once per frame
    void Update () {
		
	}
    public void CheckIsAdded(string sharecode, string uid, System.Action<bool> callbackWhenDone)
    {
    //public void CheckIsAdded()
   // {
       // string uid = "iE3osinqMgcjehknsyBMlOHBu7k2";
        //System.Action<bool> callbackWhenDone = userInfo => { };
        // string sharecode = "RHXLUW";
        DatabaseReference child = FirebaseDatabase.DefaultInstance
            .GetReference(FirebaseHelper.SHARE_CODE).Child(sharecode).Child("added_uid").Child(uid);
        child.KeepSynced(true);
        child.GetValueAsync().ContinueWith(task =>
        {
            Debug.Log("CheckIsAdded :: task :: IsCompleted");
            DataSnapshot snapshot = task.Result;

            if (snapshot == null || snapshot.Exists == false)
            {
            
                Debug.Log("CheckIsAdded :: task :: CheckIsAdded =" + false);
                callbackWhenDone(false);
            }
            else
            {
                Debug.Log("CheckIsAdded :: task :: CheckIsAdded =" + true);
                callbackWhenDone(true);
            }
        });

    }


    public void addUidToShareCode(string uid, string shareCode)//
    {
        //string uid="test"+FirebaseHelper.GenerateRandomString(10);
       // string shareCode= "RHXLUW";
        Dictionary<string, object> userDict = new Dictionary<string, object>();
        userDict.Add(uid,"0");
        FirebaseDatabase.DefaultInstance
            .GetReference(FirebaseHelper.SHARE_CODE)
            .Child(shareCode)
            .Child("added_uid").UpdateChildrenAsync(userDict);
        Debug.Log("addUidToShareCode done!");
    }

    //public void addSuccessSharedCodeToQueue(UserInfo user, System.Action<bool> callbackWhenDone)
    //{

    public void addSuccessSharedCodeToQueue( UserInfo userInfo)
    {
      

        userInfo.success_shared_queue.Add(currentUserInfo.uid);
        firebaseHelper.UpdateUser(userInfo);

      
    }

    public void GetUserByShareCode(string sharecode,System.Action<UserInfo> callbackWhenDone)
    { 
   // public void GetUserByShareCode()
   // {
        //System.Action<UserInfo> callbackWhenDone = userInfo => { };
       // string sharecode = "RHXLUW";
        DatabaseReference child = FirebaseDatabase.DefaultInstance
            .GetReference(FirebaseHelper.SHARE_CODE).Child(sharecode).Child("uid");
        child.KeepSynced(true);
        child.GetValueAsync().ContinueWith(task =>
        {
            Debug.Log("GetUserByShareCode :: task :: IsCompleted");
            DataSnapshot snapshot = task.Result;
            

            if (snapshot != null && snapshot.Exists == true)
            {
                Debug.Log("GetUserByShareCode :: snapshot ::  " + snapshot.GetRawJsonValue());
                string uid = (string)snapshot.Value;
                Debug.Log("GetUserByShareCode :: task :: userJson="+ uid);
                GetUserByUid(uid, callbackWhenDone);
            }
            else
            {
               callbackWhenDone(null);
            }
        });

    }
    public void GetUserByUid(string uid,System.Action<UserInfo> callbackWhenDone)
    {
            DatabaseReference child = FirebaseDatabase.DefaultInstance
               .GetReference(FirebaseHelper.USERS)
               .Child(uid);
            child.KeepSynced(true);
            child.GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    // Handle the error...
                    Debug.Log("GetUserByUid :: task :: error" + task.ToString());
                    callbackWhenDone(null);

                }
                else if (task.IsCompleted)
                {
                    Debug.Log("GetUserByUid :: task :: IsCompleted");
                    DataSnapshot snapshot = task.Result;
                    Debug.Log("GetUserByUid :: snapshot :: OK");

                    if (snapshot != null &&
                        snapshot.Exists == true)
                    {
                        Debug.Log("GetUserByUid :: snapshot Exist");

                        Debug.Log("snapshot :: result :: " + task.Result);
                        Debug.Log("snapshot :: key :: " + snapshot.Key);
                        Debug.Log("snapshot :: value :: " + snapshot.GetRawJsonValue());

                        string userJson = snapshot.GetRawJsonValue();
                        UserInfo user = JsonUtility.FromJson<UserInfo>(userJson);

                        if (user != null)
                        {
                            Debug.Log("snapshot :: user :: OK");

                            if (user.uid != null && user.uid.Length > 0)
                            {
                                Debug.Log("snapshot :: userid :: OK :: " + user.coin);

                            }
                            else
                            {

                                Debug.Log("snapshot :: userid :: NOK");
                            }

                        }
                        else
                        {
                            Debug.Log("snapshot :: user :: NOK");
                        }

                        callbackWhenDone(user);
                    }
                    else
                    {
                        Debug.Log("GetUserByUid :: snapshot not Exist");
                        callbackWhenDone(null);
                    }
                }
            });
        
    }

    public void GetLeaderboard(System.Action<UserInfo> callbackWhenDone)
    {
        List<UserInfo> leaderboard = new List<UserInfo>();
        DatabaseReference child = FirebaseDatabase.DefaultInstance
            .GetReference(FirebaseHelper.USERS);
        child.OrderByChild("coin").LimitToLast(10);
        child.KeepSynced(true);
        child.GetValueAsync().ContinueWith(task =>
        {
            Debug.Log("GetLeaderboard :: task :: IsCompleted");
            DataSnapshot snapshot = task.Result;
            Debug.Log("GetLeaderboard :: snapshot ::  " + snapshot.GetRawJsonValue());

            if (snapshot != null && snapshot.Exists == true)
            {
                string userJson = snapshot.GetRawJsonValue();
                //Dictionary<string, UserInfo> users = JsonUtility.FromJson<Dictionary<string,UserInfo>>(userJson);
                // Debug.Log("GetLeaderboard ::  get user: " + users.Count);

                //https://stackoverflow.com/questions/44270769/ordered-dictionary-in-c-sharp-and-unity3d-firebase
                var rawUsers = snapshot.Value as Dictionary<string, object>;
                foreach (var rawUser in rawUsers)
                {
                    UserInfo u = new UserInfo();
                    //Debug.Log("GetLeaderboard :: value :: " + value.Value);
                    var values = rawUser.Value as Dictionary<string, object>;
                    foreach (var value in values)
                    {
                        if (value.Key == "uid") { u.uid= ""+ value.Value; }
                        if (value.Key == "full_name") { u.full_name = "" + value.Value; }
                        if (value.Key == "coin") { u.coin =  int.Parse(value.Value.ToString()); }
                    }
                    leaderboard.Add(u);
                    Debug.Log("GetLeaderboard :: got user: uid" + u.uid + ", coin=" + u.coin);
                }
               
            }

            Debug.Log("GetLeaderboard :: task :: IsCompleted,leaderboard size=" + leaderboard.Count);
        });
       
    }
}
