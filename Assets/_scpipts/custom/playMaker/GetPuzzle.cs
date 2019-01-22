// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Finds the Child of a GameObject by Name and/or Tag. Use this to find attach points etc. NOTE: This action will search recursively through all children and return the first match; To find a specific child use Find Child.")]
	public class GetPuzzle : FsmStateAction
	{
 
        [Tooltip("Get Puzzle from DB")]
		public string poolId;
        [Tooltip("Get Puzzle from DB")]
        public FsmInt oldId;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the id of puzzle")]
        public FsmInt puzzleId; 



        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the solution of puzzle")]
        public FsmString solution;


        [UIHint(UIHint.Variable)]
        [Tooltip("Store the id of puzzle")]
        public FsmString randomSolution1;



        [UIHint(UIHint.Variable)]
        [Tooltip("Store the id of puzzle")]
        public FsmString randomSolution2;


        [UIHint(UIHint.Variable)]
        [Tooltip("Store the id of puzzle")]
        public FsmString randomSolution3;


        [UIHint(UIHint.Variable)]
        [Tooltip("Store the id of puzzle")]
        public FsmString randomSolution4;



        [UIHint(UIHint.Variable)]
        [Tooltip("Store the id of puzzle")]
        public FsmArray solutionArray;

 
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the id of puzzle")]
        public FsmArray RandomSolutionArray;

        public override void Reset()
		{
            poolId = null;
            solution = null;
            puzzleId = null;
		}

		public override void OnEnter()
		{
             DoGetpuzzle() ;

			Finish();
		}

		void DoGetpuzzle()
		{
            Puzzle puzzle = DbHelper.getInstance().getNextPuzzle(oldId.Value);
            puzzleId.Value = puzzle.id;
            solution.Value = puzzle.solution;

            randomSolution1.Value = puzzle.randomSolution[0];
            randomSolution2.Value = puzzle.randomSolution[1];
            randomSolution3.Value = puzzle.randomSolution[2];
            randomSolution4.Value = puzzle.randomSolution[3];
            string randomSolution = puzzle.solution;
          
            int randomCharsLength = 12 - puzzle.solution.Length;
            //string randomSolutionChar = puzzle.solution + GenerateRandomString(randomCharsLength);
            string randomSolutionChar = puzzle.questions;
            string[] randomaray = StringToCharArray(randomSolutionChar);
            solutionArray.Values= StringToCharArray(puzzle.solution);
            RandomSolutionArray.Values= randomaray;
            //reshuffle(randomaray);
          //  Debug.Log("RandomSolutionArray.Values="+ RandomSolutionArray.Values.Length);
        }
        void reshuffle(string[] texts)
        {
            // Knuth shuffle algorithm :: courtesy of Wikipedia :)
            for (int t = 0; t < texts.Length; t++)
            {
                string tmp = texts[t];
                int r = Random.Range(t, texts.Length);
                texts[t] = texts[r];
                texts[r] = tmp;
            }
        }
        string[] StringToCharArray(string str)
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
        public static string GenerateRandomString(int length)
        {
            System.Random random=new System.Random();
            string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            //string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            System.Text.StringBuilder result = new System.Text.StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                result.Append(characters[random.Next(characters.Length)]);
            }
            return result.ToString();
        }
        public override string ErrorCheck()
		{
	
			return null;
		}

	}
}