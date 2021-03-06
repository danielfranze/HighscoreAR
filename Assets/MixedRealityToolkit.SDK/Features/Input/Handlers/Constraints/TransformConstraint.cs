// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Base class for all constraints
    /// </summary>
    public abstract class TransformConstraint : MonoBehaviour
    {
        #region Properties

        [SerializeField]
        [Tooltip("Transform being constrained. Defaults to the object of the component.")]
        private Transform targetTransform = null;

        /// <summary>
        /// Transform that we intend to apply constraints to
        /// </summary>
        public Transform TargetTransform
        {
            get => targetTransform;
            set => targetTransform = value;
        }

        protected MixedRealityPose worldPoseOnManipulationStart;

        public abstract TransformFlags ConstraintType { get; }

        #endregion Properties

        #region MonoBehaviour Methods

        public virtual void Start()
        {
            if (TargetTransform == null)
            {
                TargetTransform = transform;
            }
        }

        #endregion MonoBehaviour Methods

        #region Public Methods

        /// <summary>
        /// Intended to be called on manipulation started
        /// </summary>
        public virtual void Initialize(MixedRealityPose worldPose)
        {
            worldPoseOnManipulationStart = worldPose;
        }

        /// <summary>
        /// Abstract method for applying constraints to transforms during manipulation
        /// </summary>
        public abstract void ApplyConstraint(ref MixedRealityTransform transform);

        #endregion Public Methods
    }
}