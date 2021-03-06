﻿using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;


public class DeployArrow : MonoBehaviour {

	private static List<UIImageButton> disabled = new List<UIImageButton>();

	private GameObject selectText;

	public int PathNumber = 0;

	private UISprite wait;

	// Use this for initialization
	void Start () {
		// Hide the Arrow, until a User Clicks Deploy Squad...
		GameVars.PathArrows.Add (gameObject);
		NGUITools.SetActive (gameObject, false);
	}

	void Awake () {
		selectText = GameObject.Find ("SelectPath");
		wait = GameObject.Find ("SelectPath").GetComponent<UISprite>();
	}
	
	// Update is called once per frame
	void Update () {

		// Cancel on right click...
		if (Input.GetMouseButtonDown (1)) {

			NGUITools.SetActive (gameObject, false);
			NGUITools.SetActive (selectText, false);

			// Enable all + buttons, since we cancelled...
			foreach(GameObject go in GameObject.FindGameObjectsWithTag ("plusButton")) go.GetComponent<UIImageButton>().isEnabled = true;
			Debug.Log (disabled.Count);
			foreach(UIImageButton dis in disabled) dis.isEnabled = false;

		}
		else {

			// Disable all + buttons while selecting a path
			foreach(GameObject go in GameObject.FindGameObjectsWithTag ("plusButton")) {
				go.GetComponent<UIImageButton>().isEnabled = false;
			}
		}

		// Change back to select if squad deployed...
		if(wait.spriteName == "Wait" && GameVars.DeployingPath.ContainsKey(PathNumber) && GameVars.DeployingPath[PathNumber] == false) {
			wait.spriteName = "Deploy";
			wait.MakePixelPerfect();
			wait.MarkAsChanged();
		}

	
	} // End Update()

	public string FirstLetterToUpper(string str)
	{
		if (str == null)
			return null;
		
		if (str.Length > 1)
			return char.ToUpper(str[0]) + str.Substring(1);
		
		return str.ToUpper();

	} // End FireLetterToUpper()


	void OnHover (bool hovering) {

		// Change back to select if squad deployed...
		UISprite wait = GameObject.Find ("SelectPath").GetComponent<UISprite>();

		if(hovering) {

			if(GameVars.DeployingPath.ContainsKey(PathNumber) && GameVars.DeployingPath[PathNumber] == true) {
				
				// Change Select to Wait
				
				wait.spriteName = "Wait";
				wait.MakePixelPerfect();
				wait.MarkAsChanged();
				
			}
			else {
				
				wait.spriteName = "Deploy";
				wait.MakePixelPerfect();
				wait.MarkAsChanged();
			}
		}
		else {

			wait.spriteName = "SelectAPath";
			wait.MakePixelPerfect();
			wait.MarkAsChanged();	wait.MarkAsChanged();
		}

	} // End OnHover()
	

	void OnClick () {

		// If waiting, return...
		if(GameVars.DeployingPath.ContainsKey(PathNumber) && GameVars.DeployingPath[PathNumber] == true) return;

		if (UICamera.currentTouchID == -1) { // Only on a left-click

			// Determine the "Deploy" Button Clicked
			string whichSquad = GameVars.SquadDeployClicked.squadName;

			// If Null, return.
			if(GameVars.SquadDeployClicked == null) return;

			// If the squad name isn't "alpha", "beta", or "omega" throw an exception...
			if(GameVars.Squads[whichSquad] == null) throw new UnityException("Unknown Squad");

			// Instantiate the squad's units...
			if(GameVars.Squads[whichSquad].Length > 0) {

				DeployButton src = GameObject.Find("BuildSquad" + FirstLetterToUpper(whichSquad) + "/DeploySquad").GetComponent<DeployButton>();

				src.UnitFactory(PathNumber, GameVars.Squads[whichSquad], new Vector3(gameObject.transform.position.x - .1f, gameObject.transform.position.y, gameObject.transform.position.z), new Quaternion(0,0,0,0));

				// Disable the deploy button now,
				GameVars.SquadDeployClicked.GetComponent<UIImageButton>().isEnabled = false;


				// Re-enable all + buttons
				foreach(GameObject go in GameObject.FindGameObjectsWithTag ("plusButton")) {
					if(disabled.IndexOf(go.GetComponent<UIImageButton>()) < 0) {
						go.GetComponent<UIImageButton>().isEnabled = true;
					}
				}


				// Now hide the "Select A Path" Sprite...
				NGUITools.SetActive(selectText, false);
			}

			int squadStartI = 0;
			
			switch(whichSquad) {
			case "alpha":
				squadStartI = 1;
				break;
			case "beta":
				squadStartI = 7;
				break;
			case "omega":
				squadStartI = 13;
				break;
			}
			
			// Now disable the add buttons for this squad
			for(int i = squadStartI; i <= GameVars.SquadMaxUnits + squadStartI - 1; i++) {

				UIImageButton plusButton = GameObject.Find ("AddUnit" + i).GetComponent<UIImageButton>();
				
				if (plusButton.disabledSprite == "AddUnit") plusButton.disabledSprite = "NoAdd";

				plusButton.isEnabled = false;
				disabled.Add(plusButton);
			}

			// Now hide the Arrows...
			foreach(GameObject g in GameVars.PathArrows) NGUITools.SetActive (g, false);

			// Indicate that this squad was deployed
			GameVars.SquadsDeployed.Add(whichSquad);
			Console.Push (GameVars.UCFirst(whichSquad) + " squad has been deployed");


			wait.spriteName = "SelectAPath";
			wait.MakePixelPerfect();
			wait.MarkAsChanged();

		} // End if left click

	} // End OnClick
}
