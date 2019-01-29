// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;
using System.Threading;
namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("FireBase")]
	public class PlaymakerFirebaseManager : FsmStateAction
	{
 

 
        [UIHint(UIHint.Variable)] 
        [ObjectType(typeof(FireBaseUserHelper))]
        public FsmObject storeFireBaseUserHelper;

 
        public FsmEvent isDone;

        public override void Reset()
		{
      
		}

        public void callbackWhenInitDone()
        {
            Debug.Log("callbackWhenInitDone");
            Finish();
            Fsm.Event(isDone);
        }

        public override void OnEnter()
		{
            GameObject firebaseManager = GameObject.Find("firebaseManager");
            if(firebaseManager== null)
            {
                firebaseManager = new GameObject();
                firebaseManager.name = "firebaseManager";
                FireBaseUserHelper fireBaseUserHelper = firebaseManager.AddComponent<FireBaseUserHelper>();
                fireBaseUserHelper.callbackWhenInitDone = callbackWhenInitDone;
                storeFireBaseUserHelper.Value = fireBaseUserHelper;
                GameObject.DontDestroyOnLoad(firebaseManager);

            }
            else
            {
                FireBaseUserHelper fireBaseUserHelper = firebaseManager.GetComponent<FireBaseUserHelper>();
                storeFireBaseUserHelper.Value = firebaseManager;
                callbackWhenInitDone();
            }
       
            
		}

		public override string ErrorCheck()
		{
	
			return null;
		}

	}
}