// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;
using System.Threading;
namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("FireBase")]
	public class CheckUpdateCoin : FsmStateAction
	{
        private FireBaseUserHelper fireBaseUserHelper;

        public FsmInt bonusCoin;

        public FsmEvent onShowBonusCoin;
        public FsmEvent onNoBonusCoin;
     
        public override void Reset()
		{
      
		}

        public void initFirebaseDone()
        {
            
            fireBaseUserHelper.ReloadCurrentUser(OnReloadUserDone);
            // Finish();
            // Fsm.Event(isDone);
        }
        private void OnReloadUserDone(UserInfo user)
        {
            Finish();
            if (user.success_shared_queue != null && user.success_shared_queue.Count >0 )
            {
                bonusCoin.Value = user.success_shared_queue.Count * 200;
                Fsm.Event(onShowBonusCoin);
            }
            else
            {
                Fsm.Event(onNoBonusCoin);
            }
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