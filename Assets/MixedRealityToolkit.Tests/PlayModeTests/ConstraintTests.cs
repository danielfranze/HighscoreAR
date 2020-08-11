﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// The assembly that this file is part of is still built for the player,
// even though the assembly itself is marked as a test assembly (this is not
// expected because test assemblies should not be included in player builds).
// Because the .NET backend is deprecated in 2018 and removed in 2019 and this
// issue will likely persist for 2018, this issue is worked around by wrapping all
// play mode tests in this check.

using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class ConstraintTests
    {
        [SetUp]
        public void Setup()
        {
            PlayModeTestUtilities.Setup();
        }

        [TearDown]
        public void TearDown()
        {
            PlayModeTestUtilities.TearDown();
        }

        /// <summary>
        /// Tests that the MoveAxisConstraint works for various axes.
        /// This test uses world space axes.
        /// </summary>
        [UnityTest]
        public IEnumerator ConstrainMovementAxisWorldSpace()
        {
            TestUtilities.PlayspaceToOriginLookingForward();

            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;
            Vector3 initialObjectPosition = new Vector3(0f, 0f, 1f);
            testObject.transform.position = initialObjectPosition;
            var manipHandler = testObject.AddComponent<ObjectManipulator>();
            manipHandler.HostTransform = testObject.transform;
            manipHandler.SmoothingActive = false;
            manipHandler.ManipulationType = ObjectManipulator.HandMovementType.OneHanded;

            var constraint = manipHandler.EnsureComponent<MoveAxisConstraint>();
            constraint.UseLocalSpaceForConstraint = false;

            yield return new WaitForFixedUpdate();
            yield return null;

            float moveAmount = 1.5f;
            const int numHandSteps = 1;

            // Hand pointing at middle of cube
            Vector3 initialHandPosition = new Vector3(0.044f, -0.1f, 0.45f);
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(initialHandPosition);

            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            // Constrain x axis
            constraint.ConstraintOnMovement = AxisFlags.XAxis;

            yield return hand.Move(Vector3.one * moveAmount, numHandSteps);
            yield return null;

            Assert.AreEqual(initialObjectPosition.x, testObject.transform.position.x);
            Assert.AreNotEqual(initialObjectPosition.y, testObject.transform.position.y);
            Assert.AreNotEqual(initialObjectPosition.z, testObject.transform.position.z);

            yield return hand.MoveTo(initialHandPosition, numHandSteps);
            yield return null;

            // Constrain y axis
            constraint.ConstraintOnMovement = AxisFlags.YAxis;

            yield return hand.Move(Vector3.one * moveAmount, numHandSteps);
            yield return null;

            Assert.AreNotEqual(initialObjectPosition.x, testObject.transform.position.x);
            Assert.AreEqual(initialObjectPosition.y, testObject.transform.position.y);
            Assert.AreNotEqual(initialObjectPosition.z, testObject.transform.position.z);

            yield return hand.MoveTo(initialHandPosition, numHandSteps);
            yield return null;

            // Constrain z axis
            constraint.ConstraintOnMovement = AxisFlags.ZAxis;

            yield return hand.Move(Vector3.one * moveAmount, numHandSteps);
            yield return null;

            Assert.AreNotEqual(initialObjectPosition.x, testObject.transform.position.x);
            Assert.AreNotEqual(initialObjectPosition.y, testObject.transform.position.y);
            Assert.AreEqual(initialObjectPosition.z, testObject.transform.position.z);

            yield return hand.MoveTo(initialHandPosition, numHandSteps);
            yield return null;

            // Constrain two axes
            constraint.ConstraintOnMovement = AxisFlags.XAxis | AxisFlags.ZAxis;

            yield return hand.Move(Vector3.one * moveAmount, numHandSteps);
            yield return null;

            Assert.AreEqual(initialObjectPosition.x, testObject.transform.position.x);
            Assert.AreNotEqual(initialObjectPosition.y, testObject.transform.position.y);
            Assert.AreEqual(initialObjectPosition.z, testObject.transform.position.z);
        }

        /// <summary>
        /// Tests that the MoveAxisConstraint works for various axes.
        /// This test uses local space axes.
        /// </summary>
        [UnityTest]
        public IEnumerator ConstrainMovementAxisLocalSpace()
        {
            TestUtilities.PlayspaceToOriginLookingForward();

            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;
            Vector3 initialObjectPosition = new Vector3(0f, 0f, 1f);
            testObject.transform.position = initialObjectPosition;
            Quaternion initialObjectRotation = Quaternion.Euler(30, 30, 30);
            testObject.transform.rotation = initialObjectRotation;
            var manipHandler = testObject.AddComponent<ObjectManipulator>();
            manipHandler.HostTransform = testObject.transform;
            manipHandler.SmoothingActive = false;
            manipHandler.ManipulationType = ObjectManipulator.HandMovementType.OneHanded;

            var constraint = manipHandler.EnsureComponent<MoveAxisConstraint>();
            constraint.UseLocalSpaceForConstraint = true;

            yield return new WaitForFixedUpdate();
            yield return null;

            float moveAmount = 1.5f;
            const int numHandSteps = 1;
            Quaternion inverse = Quaternion.Inverse(initialObjectRotation);

            // Hand pointing at middle of cube
            Vector3 initialHandPosition = new Vector3(0.044f, -0.1f, 0.45f);
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(initialHandPosition);

            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            // Constrain x axis
            constraint.ConstraintOnMovement = AxisFlags.XAxis;

            yield return hand.Move((initialObjectRotation * Vector3.one) * moveAmount, numHandSteps);
            yield return null;

            Assert.AreEqual((inverse * initialObjectPosition).x, (inverse * testObject.transform.position).x, 0.01f);
            Assert.AreNotEqual((inverse * initialObjectPosition).y, (inverse * testObject.transform.position).y);
            Assert.AreNotEqual((inverse * initialObjectPosition).z, (inverse * testObject.transform.position).z);

            yield return hand.MoveTo(initialHandPosition, numHandSteps);
            yield return null;

            // Constrain y axis
            constraint.ConstraintOnMovement = AxisFlags.YAxis;

            yield return hand.Move(Vector3.one * moveAmount, numHandSteps);
            yield return null;

            Assert.AreNotEqual((inverse * initialObjectPosition).x, (inverse * testObject.transform.position).x);
            Assert.AreEqual((inverse * initialObjectPosition).y, (inverse * testObject.transform.position).y, 0.01f);
            Assert.AreNotEqual((inverse * initialObjectPosition).z, (inverse * testObject.transform.position).z);

            yield return hand.MoveTo(initialHandPosition, numHandSteps);
            yield return null;

            // Constrain z axis
            constraint.ConstraintOnMovement = AxisFlags.ZAxis;

            yield return hand.Move(Vector3.one * moveAmount, numHandSteps);
            yield return null;

            Assert.AreNotEqual((inverse * initialObjectPosition).x, (inverse * testObject.transform.position).x);
            Assert.AreNotEqual((inverse * initialObjectPosition).y, (inverse * testObject.transform.position).y);
            Assert.AreEqual((inverse * initialObjectPosition).z, (inverse * testObject.transform.position).z, 0.01f);

            yield return hand.MoveTo(initialHandPosition, numHandSteps);
            yield return null;

            // Constrain two axes
            constraint.ConstraintOnMovement = AxisFlags.XAxis | AxisFlags.ZAxis;

            yield return hand.Move(Vector3.one * moveAmount, numHandSteps);
            yield return null;

            Assert.AreEqual((inverse * initialObjectPosition).x, (inverse * testObject.transform.position).x, 0.01f);
            Assert.AreNotEqual((inverse * initialObjectPosition).y, (inverse * testObject.transform.position).y);
            Assert.AreEqual((inverse * initialObjectPosition).z, (inverse * testObject.transform.position).z, 0.01f);
        }

        /// <summary>
        /// Tests that the RotationAxisConstraint works for various axes.
        /// This test uses world space axes.
        /// </summary>
        [UnityTest]
        public IEnumerator ConstrainRotationAxisWorldSpace()
        {
            TestUtilities.PlayspaceToOriginLookingForward();

            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;
            testObject.transform.position = new Vector3(0f, 0f, 1f);
            var manipHandler = testObject.AddComponent<ObjectManipulator>();
            manipHandler.HostTransform = testObject.transform;
            manipHandler.SmoothingActive = false;
            manipHandler.ManipulationType = ObjectManipulator.HandMovementType.OneHanded;
            manipHandler.OneHandRotationModeFar = ObjectManipulator.RotateInOneHandType.RotateAboutObjectCenter;

            var constraint = manipHandler.EnsureComponent<RotationAxisConstraint>();
            constraint.UseLocalSpaceForConstraint = false;

            yield return new WaitForFixedUpdate();
            yield return null;
            
            const int numHandSteps = 1;

            // Hand pointing at middle of cube
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(new Vector3(0.044f, -0.1f, 0.45f));
            var rotateTo = Quaternion.Euler(45, 45, 45);

            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            // Constrain x axis
            constraint.ConstraintOnRotation = AxisFlags.XAxis;

            yield return hand.SetRotation(rotateTo, numHandSteps);
            yield return null;

            Assert.AreEqual(0, testObject.transform.rotation.eulerAngles.x, 0.01f);
            Assert.AreNotEqual(0, testObject.transform.rotation.eulerAngles.y);
            Assert.AreNotEqual(0, testObject.transform.rotation.eulerAngles.z);

            yield return hand.SetRotation(Quaternion.identity, numHandSteps);
            yield return null;

            // Constrain y axis
            constraint.ConstraintOnRotation = AxisFlags.YAxis;

            yield return hand.SetRotation(rotateTo, numHandSteps);
            yield return null;

            Assert.AreNotEqual(0, testObject.transform.rotation.eulerAngles.x);
            Assert.AreEqual(0, testObject.transform.rotation.eulerAngles.y, 0.01f);
            Assert.AreNotEqual(0, testObject.transform.rotation.eulerAngles.z);

            yield return hand.SetRotation(Quaternion.identity, numHandSteps);
            yield return null;

            // Constrain z axis
            constraint.ConstraintOnRotation = AxisFlags.ZAxis;

            yield return hand.SetRotation(rotateTo, numHandSteps);
            yield return null;

            Assert.AreNotEqual(0, testObject.transform.rotation.eulerAngles.x);
            Assert.AreNotEqual(0, testObject.transform.rotation.eulerAngles.y);
            Assert.AreEqual(0, testObject.transform.rotation.eulerAngles.z, 0.01f);

            yield return hand.SetRotation(Quaternion.identity, numHandSteps);
            yield return null;

            // Constrain two axes
            constraint.ConstraintOnRotation = AxisFlags.XAxis | AxisFlags.ZAxis;

            yield return hand.SetRotation(rotateTo, numHandSteps);
            yield return null;

            Assert.AreEqual(0, testObject.transform.rotation.eulerAngles.x, 0.01f);
            Assert.AreNotEqual(0, testObject.transform.rotation.eulerAngles.y);
            Assert.AreEqual(0, testObject.transform.rotation.eulerAngles.z, 0.01f);
        }

        /// <summary>
        /// Tests that the RotationAxisConstraint works for various axes.
        /// This test uses local space axes.
        /// </summary>
        [UnityTest]
        public IEnumerator ConstrainRotationAxisLocalSpace()
        {
            TestUtilities.PlayspaceToOriginLookingForward();

            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;
            testObject.transform.position = new Vector3(0f, 0f, 1f);
            Quaternion initialObjectRotation = Quaternion.Euler(-30, -30, -30);
            testObject.transform.rotation = initialObjectRotation;
            var manipHandler = testObject.AddComponent<ObjectManipulator>();
            manipHandler.HostTransform = testObject.transform;
            manipHandler.SmoothingActive = false;
            manipHandler.ManipulationType = ObjectManipulator.HandMovementType.OneHanded;
            manipHandler.OneHandRotationModeFar = ObjectManipulator.RotateInOneHandType.RotateAboutObjectCenter;

            var constraint = manipHandler.EnsureComponent<RotationAxisConstraint>();
            constraint.UseLocalSpaceForConstraint = true;

            yield return new WaitForFixedUpdate();
            yield return null;

            const int numHandSteps = 1;
            Quaternion inverse = Quaternion.Inverse(initialObjectRotation);

            // Hand pointing at middle of cube
            Vector3 initialHandPosition = new Vector3(0.044f, -0.1f, 0.45f);
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(initialHandPosition);
            var rotateTo = Quaternion.Euler(45, 45, 45);

            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            // Constrain x axis
            constraint.ConstraintOnRotation = AxisFlags.XAxis;

            yield return hand.SetRotation(rotateTo, numHandSteps);
            yield return null;

            Assert.AreEqual(0, (inverse * testObject.transform.rotation).eulerAngles.x, 0.01f);
            Assert.AreNotEqual(0, (inverse * testObject.transform.rotation).eulerAngles.y);
            Assert.AreNotEqual(0, (inverse * testObject.transform.rotation).eulerAngles.z);

            yield return hand.SetRotation(initialObjectRotation, numHandSteps);
            yield return null;

            // Constrain y axis
            constraint.ConstraintOnRotation = AxisFlags.YAxis;

            yield return hand.SetRotation(rotateTo, numHandSteps);
            yield return null;

            Assert.AreNotEqual(0, (inverse * testObject.transform.rotation).eulerAngles.x);
            Assert.AreEqual(0, (inverse * testObject.transform.rotation).eulerAngles.y, 0.01f);
            Assert.AreNotEqual(0, (inverse * testObject.transform.rotation).eulerAngles.z);

            yield return hand.SetRotation(initialObjectRotation, numHandSteps);
            yield return null;

            // Constrain z axis
            constraint.ConstraintOnRotation = AxisFlags.ZAxis;

            yield return hand.SetRotation(rotateTo, numHandSteps);
            yield return null;

            Assert.AreNotEqual(0, (inverse * testObject.transform.rotation).eulerAngles.x);
            Assert.AreNotEqual(0, (inverse * testObject.transform.rotation).eulerAngles.y);
            Assert.AreEqual(0, (inverse * testObject.transform.rotation).eulerAngles.z, 0.01f);

            yield return hand.SetRotation(initialObjectRotation, numHandSteps);
            yield return null;

            // Constrain two axes
            constraint.ConstraintOnRotation = AxisFlags.XAxis | AxisFlags.ZAxis;

            yield return hand.SetRotation(rotateTo, numHandSteps);
            yield return null;

            Assert.AreEqual(0, (inverse * testObject.transform.rotation).eulerAngles.x, 0.01f);
            Assert.AreNotEqual(0, (inverse * testObject.transform.rotation).eulerAngles.y);
            Assert.AreEqual(0, (inverse * testObject.transform.rotation).eulerAngles.z, 0.01f);
        }

        /// <summary>
        /// This tests the minimum and maximum scaling for manipulation.
        /// This test will scale a cube with two hand manipulation and ensure that
        /// maximum and minimum scales are not exceeded.
        /// </summary>
        [UnityTest]
        public IEnumerator ConstrainScaleMinMax()
        {
            float initialScale = 0.2f;
            float minScale = 0.5f;
            float maxScale = 2f;

            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * initialScale;
            Vector3 initialObjectPosition = new Vector3(0f, 0f, 1f);
            testObject.transform.position = initialObjectPosition;
            var manipHandler = testObject.AddComponent<ObjectManipulator>();
            manipHandler.HostTransform = testObject.transform;
            manipHandler.SmoothingActive = false;
            manipHandler.ManipulationType = ObjectManipulator.HandMovementType.TwoHanded;
            var scaleHandler = testObject.EnsureComponent<MinMaxScaleConstraint>();
            scaleHandler.ScaleMinimum = minScale;
            scaleHandler.ScaleMaximum = maxScale;

            // add near interaction grabbable to be able to grab the cube with the simulated articulated hand
            testObject.AddComponent<NearInteractionGrabbable>();
            yield return new WaitForFixedUpdate();
            yield return null;

            const int numHandSteps = 1;

            Vector3 initialHandPosition = new Vector3(0, 0, 0.5f);
            Vector3 leftGrabPosition = new Vector3(-0.1f, -0.1f, 1f); // grab the bottom left corner of the cube 
            Vector3 rightGrabPosition = new Vector3(0.1f, -0.1f, 1f); // grab the bottom right corner of the cube 
            TestHand leftHand = new TestHand(Handedness.Left);
            TestHand rightHand = new TestHand(Handedness.Right);

            // Hands grab object at initial positions
            yield return leftHand.Show(initialHandPosition);
            yield return leftHand.MoveTo(leftGrabPosition, numHandSteps);
            yield return leftHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            yield return rightHand.Show(initialHandPosition);
            yield return rightHand.MoveTo(rightGrabPosition, numHandSteps);
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            // No change to scale yet
            Assert.AreEqual(Vector3.one * initialScale, testObject.transform.localScale);

            // Move hands beyond max scale limit
            yield return leftHand.MoveTo(new Vector3(-scaleHandler.ScaleMaximum, 0, 0) + leftGrabPosition, numHandSteps);
            yield return rightHand.MoveTo(new Vector3(scaleHandler.ScaleMaximum, 0, 0) + rightGrabPosition, numHandSteps);

            // Assert scale at max
            Assert.AreEqual(Vector3.one * scaleHandler.ScaleMaximum, testObject.transform.localScale);

            // Move hands beyond min scale limit
            yield return leftHand.MoveTo(new Vector3(scaleHandler.ScaleMinimum, 0, 0) + leftGrabPosition, numHandSteps);
            yield return rightHand.MoveTo(new Vector3(-scaleHandler.ScaleMinimum, 0, 0) + rightGrabPosition, numHandSteps);

            // Assert scale at min
            Assert.AreEqual(Vector3.one * scaleHandler.ScaleMinimum, testObject.transform.localScale);
        }

        /// <summary>
        /// Tests that the FixedDistanceConstraint keeps the manipulated object
        /// at a fixed distance from the constraint object (camera)
        /// </summary>
        [UnityTest]
        public IEnumerator ConstrainMovementFixedDistance()
        {
            TestUtilities.PlayspaceToOriginLookingForward();

            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;
            testObject.transform.position = new Vector3(0f, 0f, 1f);
            var manipHandler = testObject.AddComponent<ObjectManipulator>();
            manipHandler.HostTransform = testObject.transform;
            manipHandler.SmoothingActive = false;
            manipHandler.ManipulationType = ObjectManipulator.HandMovementType.OneHanded;
            manipHandler.OneHandRotationModeFar = ObjectManipulator.RotateInOneHandType.RotateAboutObjectCenter;

            var constraint = manipHandler.EnsureComponent<FixedDistanceConstraint>();
            constraint.ConstraintTransform = CameraCache.Main.transform;

            float originalDist = (CameraCache.Main.transform.position - testObject.transform.position).magnitude;

            yield return new WaitForFixedUpdate();
            yield return null;

            const int numHandSteps = 1;

            // Hand pointing at middle of cube
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(new Vector3(0.044f, -0.1f, 0.45f));

            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            // Move and test that still same distance from head
            yield return hand.Move(Vector3.one * 0.5f, numHandSteps);
            yield return null;

            Assert.AreEqual(originalDist, (CameraCache.Main.transform.position - testObject.transform.position).magnitude, 0.001f);

            yield return hand.Move(Vector3.one * -1f, numHandSteps);
            yield return null;

            Assert.AreEqual(originalDist, (CameraCache.Main.transform.position - testObject.transform.position).magnitude, 0.001f);
        }

        /// <summary>
        /// Tests that the MaintainApparentSizeConstraint maintains the angle between opposite
        /// corners on the cube
        /// </summary>
        [UnityTest]
        public IEnumerator ConstrainScaleApparentSize()
        {
            TestUtilities.PlayspaceToOriginLookingForward();

            // set up cube with manipulation handler
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;
            testObject.transform.position = new Vector3(0f, 0f, 1f);
            var manipHandler = testObject.AddComponent<ObjectManipulator>();
            manipHandler.HostTransform = testObject.transform;
            manipHandler.SmoothingActive = false;
            manipHandler.ManipulationType = ObjectManipulator.HandMovementType.OneHanded;
            manipHandler.OneHandRotationModeFar = ObjectManipulator.RotateInOneHandType.RotateAboutObjectCenter;

            // add an xy move constraint so that the object's position does not change on screen
            var moveConstraint = manipHandler.EnsureComponent<MoveAxisConstraint>();
            moveConstraint.UseLocalSpaceForConstraint = false;
            moveConstraint.ConstraintOnMovement = AxisFlags.XAxis | AxisFlags.YAxis;

            var constraint = manipHandler.EnsureComponent<MaintainApparentSizeConstraint>();

            Vector3 topLeft = testObject.transform.TransformPoint(new Vector3(-0.5f, 0.5f, -0.5f));
            Vector3 bottomRight = testObject.transform.TransformPoint(new Vector3(0.5f, -0.5f, -0.5f));
            float originalAngle = Vector3.Angle(topLeft - CameraCache.Main.transform.position, bottomRight - CameraCache.Main.transform.position);

            yield return new WaitForFixedUpdate();
            yield return null;

            const int numHandSteps = 100;

            // Hand pointing at middle of cube
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(new Vector3(0.044f, -0.1f, 0.45f));

            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            // Move and test that still same distance from head
            yield return hand.Move(Vector3.forward * 0.5f, numHandSteps);
            yield return null;

            Vector3 newtopLeft = testObject.transform.TransformPoint(new Vector3(-0.5f, 0.5f, -0.5f));
            Vector3 newBottomRight = testObject.transform.TransformPoint(new Vector3(0.5f, -0.5f, -0.5f));
            float newAngle = Vector3.Angle(newtopLeft - CameraCache.Main.transform.position, newBottomRight - CameraCache.Main.transform.position);

            Assert.AreEqual(originalAngle, newAngle, 0.05f);
        }
    }
}
#endif