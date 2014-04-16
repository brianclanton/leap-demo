using UnityEngine;
using System.Collections;
using Leap;

public class LeapReadout : MonoBehaviour {

	public GUIText readoutPrefab;

	private GUIText controllerStatus;
	private GUIText test1;
	private GUIText test2;

	private float baseOffsetX = 0.95f;
	private float baseOffsetY = 0.95f;
	private float spacing = 0.1f;

	private Controller controller;

	private bool isConnected = false;

	// Use this for initialization
	void Start () {
		controller = new Controller();
		controllerStatus = transform.Find("ControllerStatus").GetComponent<GUIText>();
		test1 = transform.Find("Test1").GetComponent<GUIText>();
		test2 = transform.Find("Test2").GetComponent<GUIText>();
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
		}

		fps.text = "" + Time.deltaTime;
		test1.text = "" + Time.frameCount;
		test2.text = "" + Time.realtimeSinceStartup;
	}
}
