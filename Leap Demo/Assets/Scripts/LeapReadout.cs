using UnityEngine;
using System.Collections;
using Leap;

public class LeapReadout : MonoBehaviour {

	public GUIText readoutPrefab;

	private GUIText controllerStatus;
	private GUIText hands;
	private GUIText fingers;
	private GUIText pointables;
	private GUIText gestures;

	private float baseOffsetX = 0.95f;
	private float baseOffsetY = 0.95f;
	private float spacing = 0.1f;

	private Controller controller;

	private bool isConnected = false;

	// Use this for initialization
	void Start () {
		// Initialize controller
		controller = new Controller();
		controller.EnableGesture(Gesture.GestureType.TYPECIRCLE);
		controller.EnableGesture(Gesture.GestureType.TYPESWIPE);

		// Initialize GUIText objects
		controllerStatus = GetGUIText("ControllerStatus");
		hands = GetGUIText("Hands");
		fingers = GetGUIText("Fingers");
		pointables = GetGUIText("Pointables");
		gestures = GetGUIText("Gestures");
	}
	
	// Update is called once per frame
	void Update () {
		// Update connection status of LeapMotion device
		if (!isConnected && controller.IsConnected) {
			isConnected = true;
			controllerStatus.text = "Leap Motion connected";
		} else if (isConnected && !controller.IsConnected) {
			isConnected = false;
			controllerStatus.text = "Leap Motion disconnected";
		}

		if (isConnected) {
			Frame frame = controller.Frame();

			// Update GUIText
			hands.text = (frame.Hands.Count == 0 ? "No" : "" + frame.Hands.Count) + " hands detected";
			fingers.text = (frame.Fingers.Count == 0 ? "No" : "" + frame.Fingers.Count) + " fingers detected";
			pointables.text = (frame.Pointables.Count == 0 ? "No" : "" + frame.Pointables.Count) + " pointables detected";

			if (frame.Gestures().Count == 0)
				gestures.text = "No gestures detected";
			else {
				gestures.text = "Detected: ";

				for (int i = 0; i < frame.Gestures().Count; i++)
					gestures.text += frame.Gestures()[i].Type + ", ";
			}
				
		}
	}

	private GUIText GetGUIText(string name) {
		return transform.Find(name).GetComponent<GUIText>();
	}
}
