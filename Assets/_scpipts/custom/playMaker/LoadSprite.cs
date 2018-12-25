// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Finds the Child of a GameObject by Name and/or Tag. Use this to find attach points etc. NOTE: This action will search recursively through all children and return the first match; To find a specific child use Find Child.")]
	public class LoadSprite : FsmStateAction
	{
 
        [Tooltip("Get Puzzle from DB")]
        [UIHint(UIHint.Variable)]
        public FsmString puzzleId;

        public string imageIndex;


        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The source sprite of the UI Image component.")]
        [ObjectType(typeof(Sprite))]
        public FsmObject loadedSprite;

        public override void Reset()
		{
            puzzleId = null;
            imageIndex = null;
            loadedSprite = null;
		}

		public override void OnEnter()
		{
             DoGetpuzzle() ;

			Finish();
		}

		void DoGetpuzzle()
		{
            string resourceName = "_" + puzzleId + "_" + imageIndex;
           // Debug.Log("start load sprite for "+ resourceName);
            Sprite sprite = Resources.Load<Sprite>("images/" + resourceName);
          //  Debug.Log("Done load sprite", sprite);
            loadedSprite.Value = sprite;
        }

		public override string ErrorCheck()
		{
	
			return null;
		}

	}
}