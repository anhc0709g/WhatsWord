// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Finds the Child of a GameObject by Name and/or Tag. Use this to find attach points etc. NOTE: This action will search recursively through all children and return the first match; To find a specific child use Find Child.")]
	public class StringToCharArray : FsmStateAction
	{
 
        [Tooltip("Get Puzzle from DB")]
        [UIHint(UIHint.Variable)]
        public FsmString stringInput;


        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The source sprite of the UI Image component.")]
        public FsmArray charsArray;

        public override void Reset()
		{
            stringInput = null;
            charsArray = null;
		}

		public override void OnEnter()
		{
             DoGetpuzzle() ;

			Finish();
		}

		void DoGetpuzzle()
		{
            charsArray.Values = ConvertStringToCharArray(stringInput.Value);
        }

        string[] ConvertStringToCharArray(string str)
        {
            string[] strarr = new string[str.Length];
            int i = 0;
            foreach (char c in str.ToCharArray())
            {
                strarr[i] = c.ToString();
                i++;
            }
            return strarr;
        }
        public override string ErrorCheck()
		{
	
			return null;
		}

	}
}