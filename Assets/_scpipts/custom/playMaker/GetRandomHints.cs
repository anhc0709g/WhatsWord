// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Finds the Child of a GameObject by Name and/or Tag. Use this to find attach points etc. NOTE: This action will search recursively through all children and return the first match; To find a specific child use Find Child.")]
	public class GetRandomHints : FsmStateAction
	{
 
        [Tooltip("Get Puzzle from DB")]
        [UIHint(UIHint.Variable)]
        public FsmString solution;


        [Tooltip("Get Puzzle from DB")]
        [UIHint(UIHint.Variable)]
        public FsmString currentHints;


        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The source sprite of the UI Image component.")]
        public FsmString storeHints;

        public override void Reset()
		{
            solution = null;
            currentHints = null;
            storeHints = null;
		}

		public override void OnEnter()
		{

            System.Random ran = new System.Random();
            string _currentHints = currentHints.Value;
            // Debug.Log("solution=" + solution.Value);
            // Debug.Log("_currentHints=" + _currentHints);
            ArrayList alEmptyIndex = new ArrayList();
            for ( int i=0; i< solution.Value.Length; i++)
            {
                if (!currentHints.Value.Contains(i.ToString()))
                {
                    
                    alEmptyIndex.Add(i);
                }
            }
     
            int ranIdx = Random.Range(0, alEmptyIndex.Count);
   
            int newHintCharIndex = (int)alEmptyIndex[ranIdx];
           
            _currentHints = _currentHints + newHintCharIndex;
    
            storeHints.Value = _currentHints;

            Finish();
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