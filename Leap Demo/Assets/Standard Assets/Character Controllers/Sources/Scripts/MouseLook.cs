using UnityEngine;
using System.Collections;
using Leap;

/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSInputController script to the capsule
///   -> A CharacterMotor and a CharacterController component will be automatically added.

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour {

	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;

	public float minimumX = -120F;
	public float maximumX = 120F;

	public float minimumY = -60F;
	public float maximumY = 60F;

	public bool leftHanded = false;

	private Controller controller;

	private bool handJustEntered = false;
	private int currentHandCount = 0;
	private bool usingCorrectHand = true;

	float rotationY = 0F;

	void Update ()
	{
		// Get frame from Leap Motion Controller
		Frame frame = controller.Frame();

		float xInput = 0;
		float yInput = 0;

		if (frame.Hands.Count > currentHandCount) {
			handJustEntered = true;
			currentHandCount = frame.Hands.Count;
		} else if (frame.Hands.Count < currentHandCount) {
			Debug.Log("Hand removed from the interaction box");
			usingCorrectHand = false;
			currentHandCount = frame.Hands.Count;
		}

		if (handJustEntered) {
			Debug.Log("New hand just entered the interaction box");
			handJustEntered = false;

			// First check if the correct hand is being used
			Handedness handedness = Util.GetHandedness(frame.Hands[currentHandCount - 1]);
			
			usingCorrectHand = leftHanded && handedness == Handedness.RIGHT || !leftHanded && handedness == Handedness.LEFT;
			Debug.Log("Correct hand: " + usingCorrectHand);
		}

		if (usingCorrectHand) {
			// Calculate average finger velocity
			Vector3 averageFingerVelocity = Vector3.zero;
			
			for (int i = 0; i < frame.Fingers.Count; i++)
				averageFingerVelocity += frame.Fingers[i].TipVelocity.ToUnityScaled();
			
			averageFingerVelocity /= frame.Fingers.Count;
			
			// Define movement factors
			//		float xFactor = averageFingerVelocity.x * averageFingerVelocity.x + 1.0f;
			//		float yFactor = averageFingerVelocity.y * averageFingerVelocity.y + 1.0f;
			//		float zFactor = Mathf.Abs(averageFingerVelocity.z);
			
			if (frame.Hands.Count > 0) {
				Vector3 palmPosition = frame.Hands[0].PalmPosition.ToUnityScaled();
				Vector3 palmVelocity = frame.Hands[0].PalmVelocity.ToUnityScaled();
				
				if (palmPosition.z > 0.0 && palmVelocity.magnitude < 8.0f && frame.Fingers.Count > 2) {
					xInput = averageFingerVelocity.x * Mathf.Abs(averageFingerVelocity.x);
					xInput *= 0.1f;
					yInput = averageFingerVelocity.y * Mathf.Abs(averageFingerVelocity.y);
					yInput *= 0.1f;
				}
			}
			
			if (axes == RotationAxes.MouseXAndY) {
				
				float rotationX = transform.localEulerAngles.y + xInput * sensitivityX;
				
				rotationY += yInput * sensitivityY;	
				rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
                
                transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
            } else if (axes == RotationAxes.MouseX) {
                transform.Rotate(0, xInput * sensitivityX, 0);
            } else {
                rotationY += yInput * sensitivityY;
                rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
                
                transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
			}
		}
	}
	
	void Start ()
	{
		// Make the rigid body not change rotation
		if (rigidbody)
			rigidbody.freezeRotation = true;

		controller = new Controller();
	}
}