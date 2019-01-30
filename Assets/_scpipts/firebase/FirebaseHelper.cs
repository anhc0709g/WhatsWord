using System;
using UnityEngine;
using System.Collections.Generic;

#if !UNITY_WEBGL
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Threading.Tasks;


public class FirebaseHelper
{

    private const string USERS = "users";
    private static FirebaseHelper _instance = null;
    private static Firebase.Auth.FirebaseAuth auth = null;
    private static Firebase.Auth.FirebaseUser firebaseUser = null;

    private bool initiated = false;
    bool signedIn = false;

    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }

    public static FirebaseHelper getInstance()
    {
        if (_instance == null)
        {
            _instance = new FirebaseHelper();

            _instance.initFirebase();
        }
        return _instance;
    }

    private void _initFirebase()
    {
        FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(true);

        FirebaseApp app = FirebaseApp.DefaultInstance;
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

        app.SetEditorDatabaseUrl("https://what-s-word.firebaseio.com");
        //app.SetEditorP12FileName ("filebaseTest-2e653eef7319.p12");
        app.SetEditorServiceAccountEmail("nam@itpro.vn");

        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);

        initiated = true;

        /*
        string testjson = "{\"active_code\" : 123456,\"active_status\" : 1," +
  "\"expired_datetime\" : 1554681676315," +
  "\"join_date\" : 1544681676315," +
  "\"sub_profiles\" : {" +
    "\"-LTap2EaH7NnQ_IsAbqS\" : {" +
      "\"created_datetime\" : 1544692185908," +
      "\"icon\" : \"icon1\"," +
            "\"name\" : \"profile 1\"},"+
    "\"-LTap2EaH7NnQ_IsAbqS2\" : {" +
      "\"created_datetime\" : 1544692185908," +
      "\"icon\" : \"icon2\"," +
            "\"name\" : \"profile 2\"}}}";*/

        /* string langtest = "{'language_pack_id_1':{'difficulty':{'easy':{'course_id_1':'course 1','course_id_2':'course 2','course_id_3':'course 3','course_id_4':'course 4','course_id_5':'course 5'},'hard':{'course_id_31':'course 1','course_id_32':'course 2','course_id_33':'course 3','course_id_34':'course 4','course_id_35':'course 5'},'normal':{'course_id_21':'course 1','course_id_22':'course 2','course_id_23':'course 3','course_id_24':'course 4','course_id_25':'course 5'}},'introduce':'introduce something','language':'en-us'},'language_pack_id_2':{'difficulty':{'easy':{'course_id_vv':'Course 4','course_id_ww':'Course 5','course_id_xx':'Course 1','course_id_yy':'Course 2','course_id_zz':'Course 3'},'normal':{'course_id_aa':'Course 1','course_id_bb':'Course 2','course_id_cc':'Course 3','course_id_dd':'Course 4','course_id_ee':'Course 5'}},'introduce':'introduce something else','language':'vn-northern'}}";

         langtest = langtest.Replace('\'', '\"');
         List<LanguagePackage> packs = parseAllLanguagePackageFromString(langtest);

         Debug.Log("LanguagePackage :: " + packs.ToString());
         */
        /*
        string langtest = "{ 'course_id_1':{ 'download_link':'https://xxxxx all','introduce':'somthing something','lessons':{ 'lesson_id_1':{ 'download_link':'https://yyyyyyyyyy','locked':0,'name':'lesson 1'},'lesson_id_2':{ 'download_link':'https://kkkkk','locked':0,'name':'lesson 2'},'lesson_id_3':{ 'download_link':'https://jjjjjjj','locked':1,'name':'lesson 3'} },'locked':1,'name':'course 1','total_lesson':250} }";

        langtest = langtest.Replace('\'', '\"');
        CourseInfo course = parseCourseInfoFromString(langtest);

        Debug.Log("CourseInfo :: " + course.ToString());
        */
    }

    // Track state changes of the auth object.
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        Debug.Log("AuthStateChanged");

        if (auth.CurrentUser != firebaseUser)
        {
            signedIn = firebaseUser != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && firebaseUser != null)
            {
                Debug.Log("Signed out :: " + firebaseUser.UserId);
            }

            firebaseUser = auth.CurrentUser;

            if (signedIn)
            {
                Debug.LogFormat("AuthStateChanged :: User Changed: {0} ({1})",
                    firebaseUser.DisplayName, firebaseUser.UserId);

            }
            else
            {
                Debug.Log("AuthStateChanged :: No user is signed in :: " + firebaseUser.UserId);
            }
        }
    }

    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
    public void initFirebase()
    {
        if (!initiated)
        {
            dependencyStatus = FirebaseApp.CheckDependencies();
            if (dependencyStatus != DependencyStatus.Available)
            {	//if jump into this case => bug => will fix later
                FirebaseApp.FixDependenciesAsync().ContinueWith(task =>
                {

                    dependencyStatus = FirebaseApp.CheckDependencies();
                    if (dependencyStatus == DependencyStatus.Available)
                    {
                        initiated = true;
                        _initFirebase();

                    }
                    else
                    {
                        // This should never happen if we're only using Firebase Analytics.
                        // It does not rely on any external dependencies.
                        Debug.LogError("Could --  not resolve all Firebase dependencies: " + dependencyStatus);
                    }
                });
            }
            else
            {
                Debug.Log("_initFirebase");
                _initFirebase();
            }
        }
    }

    /**************** functions for USERS *****************/
    public void LoginAsAnonymousUser(System.Action<UserInfo> callbackWhenDone)
    {
#if UNITY_EDITOR
        Debug.Log("UNITY_EDITOR");
        if (signedIn == false)
        {   //only allow to login if it is not logged in
            auth.SignInWithEmailAndPasswordAsync("editor@itpro.vn","123456").ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInAnonymouslyAsync was canceled.");
                    callbackWhenDone(null);
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                    callbackWhenDone(null);
                    return;
                }

                firebaseUser = task.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})",
                    firebaseUser.DisplayName, firebaseUser.UserId);

                Debug.Log("DisplayName :: " + firebaseUser.DisplayName);
                Debug.Log("UserId :: " + firebaseUser.UserId);

                _instance.GetCurrentUserInfo(userInfo =>
                {
                    if (userInfo != null && userInfo.uid != "")
                    {
                        callbackWhenDone(userInfo);

                    }
                    else
                    {
                        UserInfo user = new UserInfo();
                        user.uid = firebaseUser.UserId;
                        user.share_code = "123";
                        _instance.CreateNewUser(user);

                        callbackWhenDone(user); //in callback function, inititate the first profile
                    }
                });
            });
        }
        else
        {
            Debug.Log("there is a user logged in already");

            _instance.GetCurrentUserInfo(userInfo =>
            {
                if (userInfo != null && userInfo.uid != "")
                {
                    callbackWhenDone(userInfo);

                }
                else
                {
                    UserInfo user = new UserInfo();
                    user.uid = firebaseUser.UserId;
                    user.share_code = "123";
                    _instance.CreateNewUser(user);

                    callbackWhenDone(user);
                }
            });

        }

#else
        if (signedIn == false)
        {   //only allow to login if it is not logged in
            auth.SignInAnonymouslyAsync().ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInAnonymouslyAsync was canceled.");
                    callbackWhenDone(null);
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                    callbackWhenDone(null);
                    return;
                }

                firebaseUser = task.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})",
                    firebaseUser.DisplayName, firebaseUser.UserId);

                Debug.Log("DisplayName :: " + firebaseUser.DisplayName);
                Debug.Log("UserId :: " + firebaseUser.UserId);

                _instance.GetCurrentUserInfo(userInfo =>
                {
                    if (userInfo != null && userInfo.uid != "")
                    {
                        callbackWhenDone(userInfo);

                    }
                    else
                    {
                        UserInfo user = new UserInfo();
                        user.uid = firebaseUser.UserId;
                        user.share_code = "123";
                        _instance.CreateNewUser(user);

                        callbackWhenDone(user); //in callback function, inititate the first profile
                    }
                });
            });
        }
        else
        {
            Debug.Log("there is a user logged in already");

            _instance.GetCurrentUserInfo(userInfo =>
            {
                if (userInfo != null && userInfo.uid != "")
                {
                    callbackWhenDone(userInfo);

                }
                else
                {
                    UserInfo user = new UserInfo();
                    user.uid = firebaseUser.UserId;
                    user.share_code = "123";
                    _instance.CreateNewUser(user);

                    callbackWhenDone(user);
                }
            });

        }

#endif
    }

    //return current uid
    public string CheckCurrentAuth()
    {

        if (firebaseUser != null)
        {
            // User is signed in.
            //Debug.Log("RefreshToken :: " + firebaseUser.RefreshToken);
            Debug.Log("DisplayName :: " + firebaseUser.DisplayName);
            Debug.Log("UserId :: " + firebaseUser.UserId);

            string currentUID = firebaseUser.UserId;

            return currentUID;

        }
        else
        {
            // No user is signed in.
            Debug.Log("No user is signed in");
        }

        return null;
    }

    public void signOut()
    {

        auth.SignOut();
    }

    //create new user
    public void CreateNewUser(UserInfo user)
    {
        Debug.Log(string.Format("CreateNewUser :: {0}", user.uid));

        string jsonstring = JsonUtility.ToJson(user);
 

        FirebaseDatabase.DefaultInstance
            .GetReference(USERS)
            .Child(user.uid).SetRawJsonValueAsync(jsonstring);
    }
    //create new user
    public void UpdateUser(UserInfo user)
    {
        Debug.Log(string.Format("UpdateUser :: {0}", user.uid));

        string jsonstring = JsonUtility.ToJson(user);


        FirebaseDatabase.DefaultInstance
            .GetReference(USERS)
            .Child(user.uid).SetRawJsonValueAsync(jsonstring);
    }


    public void GetCurrentUserInfo(System.Action<UserInfo> callbackWhenDone)
    {
        if (signedIn == true)
        {
            Debug.Log("GetCurrentUserInfo :: signedIn == true :: " + firebaseUser.UserId);

             DatabaseReference child= FirebaseDatabase.DefaultInstance
                .GetReference(USERS)
                .Child(firebaseUser.UserId);
            child.KeepSynced(true);
            child.GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        // Handle the error...
                        Debug.Log("getCurrentUserInfo :: task :: error" + task.ToString());
                        callbackWhenDone(null);

                    }
                    else if (task.IsCompleted)
                    {
                        Debug.Log("getCurrentUserInfo :: task :: IsCompleted");
                        DataSnapshot snapshot = task.Result;
                        Debug.Log("getCurrentUserInfo :: snapshot :: OK");

                        if (snapshot != null && 
                            snapshot.Exists == true)
                        {
                            Debug.Log("getCurrentUserInfo :: snapshot Exist");

                            Debug.Log("snapshot :: result :: " + task.Result);
                            Debug.Log("snapshot :: key :: " + snapshot.Key);
                            Debug.Log("snapshot :: value :: " + snapshot.GetRawJsonValue());

                            string userJson = snapshot.GetRawJsonValue();
                            UserInfo user = JsonUtility.FromJson<UserInfo>(userJson);

                            if (user != null)
                            {
                                Debug.Log("snapshot :: user :: OK");

                                if (user.uid!=null && user.uid.Length > 0)
                                {
                                    Debug.Log("snapshot :: userid :: OK :: " + user.uid);

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
                            Debug.Log("getCurrentUserInfo :: snapshot not Exist");
                            callbackWhenDone(null);
                        }
                    }
                });
        }
        else
        {
            Debug.Log("GetCurrentUserInfo :: signedIn == false");
            callbackWhenDone(null);
        }
    }


}
#endif