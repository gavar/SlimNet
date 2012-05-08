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

using SlimMath;

namespace SlimNet
{
    public class SphereCollider : Collider
    {
        Vector3 offset;
        BoundingSphere sphere;

        public float Radius { get { return sphere.Radius; } }
        public override Vector3 Min { get { return sphere.Center - Extents; } }
        public override Vector3 Max { get { return sphere.Center + Extents; } }
        public override Vector3 Extents { get { return new Vector3(sphere.Radius, sphere.Radius, sphere.Radius); } }
        public override Vector3 Center { get { return sphere.Center; } }

        public SphereCollider(Vector3 center, Vector3 offset, float radius)
        {
            this.offset = offset;
            Update(center, radius);
        }

        public SphereCollider(Vector3 center, float radius)
        {
            this.offset = Vector3.Zero;
            Update(center, radius);
        }

        public override bool Overlap(ref BoundingBox bb)
        {
            return SlimMath.Collision.BoxIntersectsSphere(ref bb, ref sphere);
        }

        public override bool Overlap(ref BoundingSphere bs)
        {
            return SlimMath.Collision.SphereIntersectsSphere(ref bs, ref sphere);
        }

        public override bool Raycast(ref Ray ray, out float distance)
        {
            return SlimMath.Collision.RayIntersectsSphere(ref ray, ref sphere, out distance);
        }

        public override void Update(Vector3 center)
        {
            sphere.Center = center + offset;

            if (Partition != null)
            {
                Partition.Update(this);
            }
        }

        public void Update(Vector3 center, float radius)
        {
            sphere.Center = center + offset;
            sphere.Radius = radius;

            if (Partition != null)
            {
                Partition.Update(this);
            }
        }
    }
}
