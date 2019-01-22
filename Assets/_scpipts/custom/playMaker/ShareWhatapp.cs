// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;
using System.Threading;
namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Finds the Child of a GameObject by Name and/or Tag. Use this to find attach points etc. NOTE: This action will search recursively through all children and return the first match; To find a specific child use Find Child.")]
	public class ShareWhatapp : FsmStateAction
	{
 
        [Tooltip("Get Puzzle from DB")]
        [UIHint(UIHint.Variable)]
        public FsmInt puzzleId;

        public override void Reset()
		{
            puzzleId = null;
		}

		public override void OnEnter()
		{
            Application.OpenURL("whatsapp://send?text=https://whatsword.gameaz.net/index.php?word=" + puzzleId.Value);
			Finish();
		}

		public override string ErrorCheck()
		{
	
			return null;
		}

	}
}