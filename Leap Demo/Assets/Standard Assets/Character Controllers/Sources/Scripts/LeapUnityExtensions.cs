/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2013.                                   *
* Leap Motion proprietary and  confidential.  Not for distribution.            *
* Use subject to the terms of the Leap Motion SDK Agreement available at       *
* https://developer.leapmotion.com/sdk_agreement, or another agreement between *
* Leap Motion and you, your company or other organization.                     *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;
using System.Collections.Generic;

namespace Leap {
	
	//Extension to the unity vector class. Provides automatic scaling into unity scene space.
	//Leap coordinates are in cm, so the .02f scaling factor means 1cm of hand motion = .02m scene motion
	public static class UnityVectorExtension
	{
		public static Vector3 InputScale = new Vector3(0.02f, 0.02f, 0.02f);
		public static Vector3 InputOffset = new Vector3(0,0,0);
		
		//For Directions
		public static Vector3 ToUnity(this Vector lv)
		{
			return FlippedZ(lv);
		}
		//For Acceleration/Velocity
		public static Vector3 ToUnityScaled(this Vector lv)
		{
			return Scaled(FlippedZ( lv ));
		}
		//For Positions
		public static Vector3 ToUnityTranslated(this Vector lv)
		{
			return Offset(Scaled(FlippedZ( lv )));
		}
		
		private static Vector3 FlippedZ( Vector v ) { return new Vector3( v.x, v.y, -v.z ); }
		private static Vector3 Scaled( Vector3 v ) { return new Vector3( v.x * InputScale.x,
																		 v.y * InputScale.y,
																		 v.z * InputScale.z ); }
		private static Vector3 Offset( Vector3 v ) { return v + InputOffset; }
	}

	public enum Handedness{ LEFT, RIGHT, UNKNOWN };

	public static class Util {

		public static Vector3 ToVector3(Vector v) {
			return new Vector3(v.x, v.y, v.z);
		}

		public static Handedness GetHandedness(Hand hand) {
			// Can't tell handness if the hand has no fingers
			if (hand.Fingers.Count == 0)
				return Handedness.UNKNOWN;

			Vector handXBasis = hand.PalmNormal.Cross(hand.Direction).Normalized;
			Vector handYBasis = -hand.PalmNormal;
			Vector handZBasis = -hand.Direction;
			Vector handOrigin = hand.PalmPosition;
			Matrix handTransform = new Matrix(handXBasis, handYBasis, handZBasis, handOrigin).RigidInverse();
			float avgDist = 0;
			int fingerCount = 0;

			List<Vector> transformedFingers = new List<Vector>();

			foreach (Finger finger in hand.Fingers) {
				Vector transformedPosition = handTransform.TransformPoint(finger.TipPosition);
				Vector transformedDirection = handTransform.TransformPoint(finger.Direction);

				transformedFingers.Add(transformedPosition);
				avgDist += transformedPosition.z - handOrigin.z;
				fingerCount++;
			}

			avgDist /= fingerCount;
			fingerCount = 0;

			Vector leftmostFingerVector = null;
			Vector rightmostFingerVector = null;

			foreach (Finger finger in hand.Fingers) {
				Vector transformedPosition = transformedFingers[fingerCount];

				if (leftmostFingerVector == null || transformedPosition.x < leftmostFingerVector.x)
					leftmostFingerVector = transformedPosition;
				if (rightmostFingerVector == null || transformedPosition.x > rightmostFingerVector.x)
					rightmostFingerVector = transformedPosition;

				fingerCount++;
			}

			if (leftmostFingerVector.z - handOrigin.z < avgDist * 0.55f && rightmostFingerVector.z - handOrigin.z < avgDist * 0.55f)
				return Handedness.UNKNOWN;
			if (leftmostFingerVector.z > rightmostFingerVector.z)
				return Handedness.RIGHT;
			else
				return Handedness.LEFT;
		}	

	}
}
