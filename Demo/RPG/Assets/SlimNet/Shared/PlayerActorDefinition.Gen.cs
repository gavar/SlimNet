/*
 * SlimNet - Networking Middleware For Games
 * Copyright (C) 2011-2012 Fredrik Holmstr�m
 * 
 * This notice may not be removed or altered.
 * 
 * This software is provided 'as-is', without any expressed or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software. 
 * 
 * Attribution
 * The origin of this software must not be misrepresented; you must not
 * claim that you wrote the original software. For any works using this 
 * software, reasonable acknowledgment is required.
 * 
 * Noncommercial
 * You may not use this software for commercial purposes.
 * 
 * Distribution
 * You are not allowed to distribute or make publicly available the software 
 * itself or its source code in original or modified form.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using SlimNet;
using SlimMath;

using Network = SlimNet.Network;
using Events = SlimNet.Events;

public partial class PlayerActorDefinition : ActorDefinition
{
    //{AutoGen}
    public override Collider Collider { get { return new BoxCollider(Vector3.Zero, new Vector3(0.3f, 0.75f, 0.3f), new Vector3(0f, 0.75f, 0f)); } }
    //{AutoGen}
}