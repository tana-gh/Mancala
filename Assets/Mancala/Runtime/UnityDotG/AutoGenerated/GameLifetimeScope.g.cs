﻿// <auto-generated/>
// This file is generated by UnityDotG. Do not modify it manually.

using UnityEngine;
using VContainer;
using VContainer.Unity;
using VitalRouter.VContainer;

namespace tana_gh.Mancala
{
    public partial class GameLifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterEntryPoint<GameEntryPoint>();
        }
    }
}
