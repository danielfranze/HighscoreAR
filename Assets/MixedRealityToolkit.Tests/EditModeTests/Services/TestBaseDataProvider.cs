﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tests.Services
{
    /// <summary>
    /// Base class for test data providers
    /// </summary>
    public class TestBaseDataProvider : BaseDataProvider
    {
        public TestBaseDataProvider(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealityService service,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null)
        : base(registrar, service, name, priority, profile) { }

        public bool IsEnabled { get; private set; }
        public bool IsInitialized { get; private set; }

        public override void Initialize()
        {
            base.Initialize();

            IsInitialized = true;
        }

        public override void Reset()
        {
            base.Reset();
            Debug.Log("TestDataProvider Reset");
            IsInitialized = false;
        }

        public override void Enable()
        {
            base.Enable();

            IsEnabled = true;
        }

        public override void Disable()
        {
            base.Disable();

            IsEnabled = false;
        }

        public override void Destroy()
        {
            base.Destroy();
        }
    }
}
