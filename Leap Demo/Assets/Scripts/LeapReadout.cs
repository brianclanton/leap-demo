using UnityEngine;
using System.Collections;
using Leap;

public class LeapReadout : MonoBehaviour {

	public GUIText readoutPrefab;

	private GUIText controllerStatus;
	private GUIText hands;
	private GUIText fingers;
	private GUIText pointables;

	private float baseOffsetX = 0.95f;
	private float baseOffsetY = 0.95f;
	private float spacing = 0.1f;

	private Controller controller;

	private bool isConnected = false;

	// Use this for initialization
	void Start () {
		controller = new Controller();
		controllerStatus = transform.Find("ControllerStatus").GetComponent<GUIText>();
		hands = transform.Find("Hands").GetComponent<GUIText>();
		fingers = transform.Find("Fingers").GetComponent<GUIText>();
		pointables = transform.Find("Pointables").GetComponent<GUIText>();
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
		}
	}
}
