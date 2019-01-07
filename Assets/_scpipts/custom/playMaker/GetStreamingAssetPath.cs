// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Finds the Child of a GameObject by Name and/or Tag. Use this to find attach points etc. NOTE: This action will search recursively through all children and return the first match; To find a specific child use Find Child.")]
	public class GetStreamingAssetPath : FsmStateAction
	{

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The source sprite of the UI Image component.")]
  
        public FsmString streamingAssetPath;

        public override void Reset()
		{

            streamingAssetPath = null;
		}

		public override void OnEnter()
		{
            streamingAssetPath.Value = Application.streamingAssetsPath;

			Finish();
		}



		public override string ErrorCheck()
		{
	
			return null;
		}

	}
}