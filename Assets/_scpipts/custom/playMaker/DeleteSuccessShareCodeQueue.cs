// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;
using System.Threading;
namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("FireBase")]
	public class DeleteSuccessShareCodeQueue : FsmStateAction
	{
        private FireBaseUserHelper fireBaseUserHelper;

     
        public override void Reset()
		{
      
		}

        public void initFirebaseDone()
        {
            fireBaseUserHelper.currentUserInfo.success_shared_queue.Clear();
            fireBaseUserHelper.UpDateUserInfo();
            Finish();
            // Fsm.Event(isDone);
        }


        public override void OnEnter()
        {
            GameObject firebaseManager = GameObject.Find("firebaseManager");
            if (firebaseManager == null)
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