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

using System;
using System.Collections.Generic;
using SlimMath;

namespace SlimNet.Collections
{
    public class QuadTree : ISpatialPartitioner
    {
        struct Point2
        {
            public float X;
            public float Z;

            public Point2(float x, float z)
            {
                X = x;
                Z = z;
            }
        }

        class Node : ISpatialPartition
        {
            static readonly Log log = Log.GetLogger(typeof(Node));

            #region Pool

            static Queue<Node> pool = new Queue<Node>();

            public static Node Acquire(QuadTree tree, Node parent, Point2 center, float extent)
            {
                Node node;

                if (pool.Count > 0)
                {
                    node = pool.Dequeue();
                }
                else
                {
                    node = new Node();
                }

                // Set input values
                node.Tree = tree;
                node.Parent = parent;
                node.Center = center;
                node.Extent = extent;

                // Calculate extents
                node.Min.X = center.X - extent;
                node.Min.Z = center.Z - extent;
                node.Max.X = center.X + extent;
                node.Max.Z = center.Z + extent;

                // Yay!
                return node;
            }

            public static void Release(Node node)
            {
                //pool.Enqueue(node);
            }

            #endregion

            public const int LEFT_TOP = 0;
            public const int LEFT_BOTTOM = 1;
            public const int RIGHT_TOP = 2;
            public const int RIGHT_BOTTOM = 3;

            public float Extent;
            public Point2 Center;
            public Point2 Min;
            public Point2 Max;
            public Node[] Children;
            public QuadTree Tree;
            public Node Parent;
            public HashSet<Collider> Colliders;

            Node()
            {
                Colliders = new HashSet<Collider>();
            }

            public void Split()
            {
                var extent = Extent / 2;
                var top = Center.Z + extent;
                var left = Center.X - extent;
                var bottom = Center.Z - extent;
                var right = Center.X + extent;

                Children = new Node[4];
                Children[LEFT_TOP] = Acquire(Tree, this, new Point2(left, top), extent);
                Children[LEFT_BOTTOM] = Acquire(Tree, this, new Point2(left, bottom), extent);
                Children[RIGHT_TOP] = Acquire(Tree, this, new Point2(right, top), extent);
                Children[RIGHT_BOTTOM] = Acquire(Tree, this, new Point2(right, bottom), extent);
            }

            public bool Contains(Collider c)
            {
                Vector3 cMin = c.Min;
                Vector3 cMax = c.Max;
                return Min.X < cMin.X && Min.Z < cMin.Z && Max.X >= cMax.X && Max.Z >= cMax.Z;
            }

            public void Raycast(ref Ray ray, List<Collider> result)
            {
                if (rayIntersects(ref ray))
                {
                    float d;

                    foreach (Collider c in Colliders)
                    {
                        if (c.Raycast(ref ray, out d))
                        {
                            result.Add(c);
                        }
                    }

                    if (Children != null)
                    {
                        for (int i = 0; i < 4; ++i)
                        {
                            Children[i].Raycast(ref ray, result);
                        }
                    }
                }
            }

            public void Overlap(ref BoundingSphere sphere, List<Collider> result)
            {
                Vector3 center = sphere.Center;
                center.Y = 0;

                Vector3 min = new Vector3(Min.X, 0, Min.Z);
                Vector3 max = new Vector3(Max.X, 0, Max.Z);

                Vector3 vector;
                Vector3.Clamp(ref center, ref min, ref max, out vector);

                if (Vector3.DistanceSquared(sphere.Center, vector) <= sphere.Radius * sphere.Radius)
                {
                    foreach (Collider c in Colliders)
                    {
                        if (c.Overlap(ref sphere))
                        {
                            result.Add(c);
                        }
                    }

                    if (Children != null)
                    {
                        for (int i = 0; i < 4; ++i)
                        {
                            Children[i].Overlap(ref sphere, result);
                        }
                    }
                }
            }

            public void Overlap(ref BoundingBox box, List<Collider> result)
            {
                Vector3 bMin = box.Minimum;
                Vector3 bMax = box.Maximum;

                if (Min.X > bMax.X || bMin.X > Max.X)
                {
                    return;
                }

                if (Min.Z > bMax.Z || bMin.Z > Max.Z)
                {
                    return;
                }

                // Intersection

                foreach (Collider c in Colliders)
                {
                    if (c.Overlap(ref box))
                    {
                        result.Add(c);
                    }
                }

                if (Children != null)
                {
                    for (int i = 0; i < 4; ++i)
                    {
                        Children[i].Overlap(ref box, result);
                    }
                }
            }

            public void Insert(Collider c)
            {
                if (Children == null)
                {
                    if (Colliders.Count >= Tree.maxColliders)
                    {
                        float xSize = c.Max.X - c.Min.X;
                        float zSize = c.Max.Z - c.Max.Z;

                        if (Extent > Tree.minExtent && xSize < Extent / 2 && zSize < Extent / 2)
                        {
                            Split();

                            HashSet<Collider> cs = Colliders;
                            Colliders = new HashSet<Collider>();

                            cs.Add(c);

                            foreach (Collider col in cs)
                            {
                                Insert(col);
                            }
                        }
                        else
                        {
                            Colliders.Add(c);
                            c.Partition = this;
                            return;
                        }
                    }
                    else
                    {
                        Colliders.Add(c);
                        c.Partition = this;
                        return;
                    }
                }

                for (int i = 0; i < 4; ++i)
                {
                    if (Children[i].Contains(c))
                    {
                        Children[i].Insert(c);
                        return;
                    }
                }

                Colliders.Add(c);
                c.Partition = this;
                return;
            }

            public void Draw(System.Action<Vector3, Vector3, Color4> draw)
            {
                Vector3 center = new Vector3(Center.X, 0, Center.Z);
                Vector3 size = new Vector3(Extent * 2, 0.05f, Extent * 2);

                draw(center, size, new Color4(1f, 1f, 1f, 1f));

                if (Children != null)
                {
                    foreach (Node n in Children)
                    {
                        n.Draw(draw);
                    }
                }

                foreach (Collider c in Colliders)
                {
                    c.Draw(draw);
                }
            }

            bool Merge()
            {
                if (Children != null)
                {
                    for (int i = 0; i < 4; ++i)
                    {
                        if (!Children[i].Merge())
                        {
                            return false;
                        }
                    }
                }

                return Colliders.Count == 0;
            }

            void ParentMerge()
            {
                if (Children != null)
                {
                    for (int i = 0; i < 4; ++i)
                    {
                        if (!Children[i].Merge())
                        {
                            return;
                        }
                    }

                    for (int i = 0; i < 4; ++i)
                    {
                        Release(Children[i]);
                    }


                    log.Trace("Removing children");
                    Children = null;
                }

                if (Colliders.Count == 0 && Parent != null)
                {
                    Parent.ParentMerge();
                }
            }

            public void Update(Collider c)
            {
                // We know we contain this collider
                if (Contains(c))
                {
                    // No childern = leaf = do nothing
                    if (Children == null && (Colliders.Count <= Tree.maxColliders || Extent <= Tree.minExtent))
                    {
                        return;
                    }
                    else
                    {
                        // Remove collider
                        Colliders.Remove(c);

                        // But start insertion from this node
                        Tree.Insert(c);
                    }
                }
                else
                {
                    // Remove collider
                    Colliders.Remove(c);

                    // Insert into tree
                    Tree.Insert(c);
                }

                // If we have a parent
                if (Parent != null && Children == null && Colliders.Count == 0)
                {
                    Parent.ParentMerge();
                }
            }

            public void Remove(Collider c)
            {
                Colliders.Remove(c);
            }

            #region Utils

            bool rayIntersects(ref Ray ray)
            {
                float distance = 0f;
                float tmax = float.MaxValue;

                if (System.Math.Abs(ray.Direction.X) < 1e-6f)
                {
                    if (ray.Position.X < Min.X || ray.Position.X > Max.X)
                    {
                        distance = 0f;
                        return false;
                    }
                }
                else
                {
                    float inverse = 1.0f / ray.Direction.X;
                    float t1 = (Min.X - ray.Position.X) * inverse;
                    float t2 = (Max.X - ray.Position.X) * inverse;

                    if (t1 > t2)
                    {
                        float temp = t1;
                        t1 = t2;
                        t2 = temp;
                    }

                    distance = Math.Max(t1, distance);
                    tmax = Math.Min(t2, tmax);

                    if (distance > tmax)
                    {
                        distance = 0f;
                        return false;
                    }
                }

                if (Math.Abs(ray.Direction.Z) < 1e-6f)
                {
                    if (ray.Position.Z < Min.Z || ray.Position.Z > Max.Z)
                    {
                        distance = 0f;
                        return false;
                    }
                }
                else
                {
                    float inverse = 1.0f / ray.Direction.Z;
                    float t1 = (Min.Z - ray.Position.Z) * inverse;
                    float t2 = (Max.Z - ray.Position.Z) * inverse;

                    if (t1 > t2)
                    {
                        float temp = t1;
                        t1 = t2;
                        t2 = temp;
                    }

                    distance = Math.Max(t1, distance);
                    tmax = Math.Min(t2, tmax);

                    if (distance > tmax)
                    {
                        distance = 0f;
                        return false;
                    }
                }

                return true;
            }

            #endregion
        }

        Node root;
        float minExtent = 4;
        int maxColliders = 4;

        public QuadTree(float minExtent, int maxColliders)
        {
            root = Node.Acquire(this, null, new Point2(0, 0), 512);
            root.Split();

            this.minExtent = minExtent;
            this.maxColliders = maxColliders;
        }

        public QuadTree()
            : this(4, 4)
        {

        }

        public void Draw(System.Action<Vector3, Vector3, Color4> draw)
        {
            root.Draw(draw);
        }

        public void Insert(Collider c)
        {
            root.Insert(c);
        }

        public List<Collider> Overlap(BoundingSphere sphere)
        {
            List<Collider> result = new List<Collider>();
            root.Overlap(ref sphere, result);
            return result;
        }

        public List<Collider> Overlap(BoundingBox box)
        {
            List<Collider> result = new List<Collider>();
            root.Overlap(ref box, result);
            return result;
        }

        public List<Collider> Raycast(Ray ray)
        {
            List<Collider> result = new List<Collider>();
            root.Raycast(ref ray, result);
            return result;
        }
    }
}