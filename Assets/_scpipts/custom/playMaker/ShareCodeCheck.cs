// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;
using System.Threading;
namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("FireBase")]
	public class ShareCodeCheck : FsmStateAction
	{

        private FireBaseUserHelper fireBaseUserHelper;

        public FsmString shareCode;



        public FsmEvent onSuccess;

        public FsmString errorString;
        public FsmEvent onErrorAlreadyAdded;
        public FsmEvent onErrorCodeNotFound;
        public FsmEvent onError;
        public override void Reset()
		{
      
		}

        public void initFirebaseDone()
        {
            Debug.Log("ShareCodeCheck start check shareCode=" + shareCode);
            if(shareCode.Value.Equals(fireBaseUserHelper.currentUserInfo.share_code))
            {
                Finish();
                Fsm.Event(onErrorCodeNotFound);
                errorString.Value = "cannot add yourself code!";
                return;
            }
            fireBaseUserHelper.CheckIsAdded(shareCode.Value,fireBaseUserHelper.currentUserInfo.uid, OnCheckIsAddedDone);
           // Finish();
           // Fsm.Event(isDone);
        }

        private void OnCheckIsAddedDone(bool isAdded)
        {
            if (isAdded)
            {
                Finish();
                Fsm.Event(onErrorAlreadyAdded);
                errorString.Value = "code already added!";
            }
            else
            {
                fireBaseUserHelper.GetUserByShareCode(shareCode.Value, OnGetUserByShareCodeDone);
            }
        }
        private void OnGetUserByShareCodeDone(UserInfo user)
        {
            if (user == null)
            {
                Finish();
                Fsm.Event(onErrorCodeNotFound);
                errorString.Value = "code not found!";
            }
            else
            {
                fireBaseUserHelper.addUidToShareCode(fireBaseUserHelper.currentUserInfo.uid, shareCode.Value);
                fireBaseUserHelper.addSuccessSharedCodeToQueue(user);
                fireBaseUserHelper.currentUserInfo.entered_code = true;
                Finish();
                Fsm.Event(onSuccess);
            }
        }


        public override void OnEnter()
		{
            GameObject firebaseManager = GameObject.Find("firebaseManager");
            if(firebaseManager== null)
            {
                firebaseManager = new GameObject();
                firebaseManager.name = "firebaseManager";
                fireBaseUserHelper = firebaseManager.AddComponent<FireBaseUserHelper>();
                fireBaseUserHelper.callbackWhenInitDone = initFirebaseDone;
                GameObject.DontDestroyOnLoad(firebaseManager);

            }
            else
            {
                fireBaseUserHelper = firebaseManager.GetComponent<FireBaseUserHelper>();
                initFirebaseDone();
            }
       
            
		}

		public override string ErrorCheck()
		{
	
			return null;
		}

	}
}