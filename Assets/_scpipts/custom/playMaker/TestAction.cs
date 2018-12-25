// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;
using System.Threading;
namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Finds the Child of a GameObject by Name and/or Tag. Use this to find attach points etc. NOTE: This action will search recursively through all children and return the first match; To find a specific child use Find Child.")]
	public class TestAction : FsmStateAction
	{
 
        [Tooltip("Get Puzzle from DB")]
        [UIHint(UIHint.Variable)]
        public FsmString puzzleId;



        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The source sprite of the UI Image component.")]
        [ObjectType(typeof(Sprite))]
        public FsmInt testOutput;

        public override void Reset()
		{
            puzzleId = null;
            testOutput = null;
		}

		public override void OnEnter()
		{
            Debug.Log("start");
            Thread.Sleep(10000);
            Debug.Log("set value");
            testOutput.Value = 19;
			Finish();
		}

		public override string ErrorCheck()
		{
	
			return null;
		}

	}
}