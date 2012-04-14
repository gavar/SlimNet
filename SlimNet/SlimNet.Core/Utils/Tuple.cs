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

namespace SlimNet
{
    public class Pair<TFirst, TSecond>
    {
        public TFirst First { get; private set; }
        public TSecond Second { get; private set; }

        public Pair(TFirst first, TSecond second)
        {
            First = first;
            Second = second;
        }
    }

    public class MutablePair<TFirst, TSecond>
    {
        public TFirst First;
        public TSecond Second;

        public MutablePair(TFirst first, TSecond second)
        {
            First = first;
            Second = second;
        }
    }

    public class Triple<TFirst, TSecond, TThird>
    {
        public TFirst First { get; private set; }
        public TSecond Second { get; private set; }
        public TThird Third { get; private set; }

        public Triple(TFirst first, TSecond second, TThird third)
        {
            First = first;
            Second = second;
            Third = third;
        }
    }

    public class Quad<TFirst, TSecond, TThird, TFourth>
    {
        public TFirst First { get; private set; }
        public TSecond Second { get; private set; }
        public TThird Third { get; private set; }
        public TFourth Fourth { get; private set; }

        public Quad(TFirst first, TSecond second, TThird third, TFourth fourth)
        {
            First = first;
            Second = second;
            Third = third;
            Fourth = fourth;
        }
    }

    public static class Tuple
    {
        public static Pair<T0, T1> Create<T0, T1>(T0 first, T1 second)
        {
            return new Pair<T0, T1>(first, second);
        }

        public static Triple<T0, T1, T2> Create<T0, T1, T2>(T0 first, T1 second, T2 third)
        {
            return new Triple<T0, T1, T2>(first, second, third);
        }

        public static Quad<T0, T1, T2, T3> Create<T0, T1, T2, T3>(T0 first, T1 second, T2 third, T3 fourth)
        {
            return new Quad<T0, T1, T2, T3>(first, second, third, fourth);
        }

        public static void Unpack<T0, T1>(Pair<T0, T1> pair, out T0 first, out T1 second)
        {
            first = pair.First;
            second = pair.Second;
        }

        public static void Unpack<T0, T1, T2>(Triple<T0, T1, T2> triple, out T0 first, out T1 second, out T2 third)
        {
            first = triple.First;
            second = triple.Second;
            third = triple.Third;
        }

        public static void Unpack<T0, T1, T2, T3>(Quad<T0, T1, T2, T3> quad, out T0 first, out T1 second, out T2 third, out T3 fourth)
        {
            first = quad.First;
            second = quad.Second;
            third = quad.Third;
            fourth = quad.Fourth;
        }
    }
}
