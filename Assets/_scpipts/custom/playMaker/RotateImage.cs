// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;
using UnityEngine.UI;
namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Finds the Child of a GameObject by Name and/or Tag. Use this to find attach points etc. NOTE: This action will search recursively through all children and return the first match; To find a specific child use Find Child.")]
	public class RotateImage : FsmStateAction
	{

        public int speed;
        [Tooltip("Get Puzzle from DB")]
        public FsmGameObject image;

        Vector3 rotationEuler;

        public override void Reset()
		{
     
		}

		public override void OnEnter()
		{
     

		}

        public override void OnUpdate()
        {
          
            rotationEuler += Vector3.forward * speed * Time.deltaTime; //increment 30 degrees every second
            image.Value.transform.rotation = Quaternion.Euler(rotationEuler);
           // Debug.Log("OnUpdate");
        }
        public override string ErrorCheck()
		{
	
			return null;
		}

	}
}