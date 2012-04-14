/*
 * SlimNet - Networking Middleware For Games
 * Copyright (C) 2011-2012 Fredrik Holmström
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

using UnityEngine;

namespace SlimNet.Unity
{
    public class ActorProxy : MonoBehaviour
    {
        public bool DisplayCollider;
        public SlimNet.Actor Actor { get; internal set; }

        void OnDestroy()
        {
            Actor = null;
        }

        void LateUpdate()
        {
            if (Actor != null)
            {
                if (Actor.CopyTransformToEngine)
                {
                    transform.position = Converter.Convert(Actor.Transform.Position);
                    transform.rotation = Converter.Convert(Actor.Transform.Rotation);
                }
            }
        }

        void OnDrawGizmos()
        {
            Vector3 p = transform.position;

            p.y += 1f;

            if (Actor == null)
            {
                Gizmos.DrawIcon(p, "SlimNet-User-Gray.png");
            }
            else
            {
                if (Actor.IsActive)
                {
                    if (Actor.IsMine)
                    {
                        Gizmos.DrawIcon(p, "SlimNet-User-Green.png");
                    }
                    else
                    {
                        Gizmos.DrawIcon(p, "SlimNet-User-Blue.png");
                    }
                }
                else
                {
                    Gizmos.DrawIcon(p, "SlimNet-User-Red.png");
                }

                if (DisplayCollider)
                {
                    if (Actor.Collider is SlimNet.BoxCollider)
                    {
                        SlimNet.BoxCollider c = (SlimNet.BoxCollider)Actor.Collider;

                        Gizmos.color = Color.red;
                        Gizmos.DrawWireCube(Converter.Convert(c.Center), Converter.Convert(c.Extents * 2f));

                    }
                    else if (Actor.Collider is SlimNet.SphereCollider)
                    {
                        SlimNet.SphereCollider c = (SlimNet.SphereCollider)Actor.Collider;

                        Gizmos.color = Color.red;
                        Gizmos.DrawWireSphere(Converter.Convert(c.Center), c.Radius);
                    }
                }
            }
        }
    }
}