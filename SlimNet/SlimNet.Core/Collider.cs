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
}