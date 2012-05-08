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
    public class BoxCollider : Collider
    {
        Vector3 extents;
        Vector3 offset;
        BoundingBox box;

        public override Vector3 Min { get { return box.Minimum; } }
        public override Vector3 Max { get { return box.Maximum; } }
        public override Vector3 Extents { get { return extents; } }
        public override Vector3 Center { get { return (Max + Min) * 0.5f; } }

        public BoxCollider(Vector3 center, Vector3 extents)
            : this(center, extents, Vector3.Zero)
        {

        }

        public BoxCollider(Vector3 center, Vector3 extents, Vector3 offset)
        {
            this.offset = offset;
            Update(center, extents);
        }

        public override void Update(Vector3 center)
        {
            center += offset;

            box.Minimum = center - extents;
            box.Maximum = center + extents;

            if (Partition != null)
            {
                Partition.Update(this);
            }
        }

        public void Update(Vector3 center, Vector3 extents)
        {
            center += offset;

            this.extents = extents;

            box.Minimum = center - extents;
            box.Maximum = center + extents;

            if (Partition != null)
            {
                Partition.Update(this);
            }
        }

        public override bool Overlap(ref BoundingBox bb)
        {
            return SlimMath.Collision.BoxIntersectsBox(ref box, ref bb);
        }

        public override bool Overlap(ref BoundingSphere bs)
        {
            return SlimMath.Collision.BoxIntersectsSphere(ref box, ref bs);
        }

        public override bool Raycast(ref Ray ray, out float distance)
        {
            return SlimMath.Collision.RayIntersectsBox(ref ray, ref box, out distance);
        }
    }
}
