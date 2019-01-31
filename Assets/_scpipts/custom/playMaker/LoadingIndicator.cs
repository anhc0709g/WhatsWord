// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;
using System.Threading;
namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Finds the Child of a GameObject by Name and/or Tag. Use this to find attach points etc. NOTE: This action will search recursively through all children and return the first match; To find a specific child use Find Child.")]
	public class LoadingIndicator : FsmStateAction
	{
 


        public override void Reset()
		{

		}

		public override void OnEnter()
		{
            #if UNITY_IPHONE
                    Handheld.SetActivityIndicatorStyle(UnityEngine.iOS.ActivityIndicatorStyle.Gray);
            #elif UNITY_ANDROID
                        Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle.Small);
            #endif

            Handheld.StartActivityIndicator();
       
		}

		public override string ErrorCheck()
		{
	
			return null;
		}
        public override void OnExit()
        {
            Handheld.StopActivityIndicator();
        }


    }
}