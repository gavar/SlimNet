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
    public abstract class Collider
    {
        public Actor Actor { get; internal set; }
        public ISpatialPartition Partition { get; internal set; }

        public abstract Vector3 Min { get; }
        public abstract Vector3 Max { get; }
        public abstract Vector3 Extents { get; }
        public abstract Vector3 Center { get; }
        public abstract bool Overlap(ref SlimMath.BoundingBox bb);
        public abstract bool Overlap(ref SlimMath.BoundingSphere bs);
        public abstract bool Raycast(ref SlimMath.Ray ray, out float distance);
        public abstract void Update(Vector3 center);

        public void Draw(System.Action<Vector3, Vector3, Color4> draw)
        {
            draw(Center, Extents * 2f, new Color4(1f, 1f, 0f, 0f));
        }

        public void RemoveFromPartition()
        {
            if (Partition != null)
            {
                Partition.Remove(this);
            }
        }

        public override int GetHashCode()
        {
            return Actor.Id;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj);
        }
    }

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