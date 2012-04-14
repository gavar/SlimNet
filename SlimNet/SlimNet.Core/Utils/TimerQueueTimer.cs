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

#if WIN && !UNITY_WEBPLAYER
using System;
using System.Runtime.InteropServices;

namespace SlimNet.Utils
{
    internal class TimerQueueTimer
    {
        static IntPtr phNewTimer;
        static WaitOrTimerDelegate delegateRef;

        public const int RESOLUTION = 1;

        enum Flag
        {
            WT_EXECUTEDEFAULT = 0x00000000,
            WT_EXECUTEINIOTHREAD = 0x00000001,
          //WT_EXECUTEINWAITTHREAD = 0x00000004,
            WT_EXECUTEONLYONCE = 0x00000008,
            WT_EXECUTELONGFUNCTION = 0x00000010,
            WT_EXECUTEINTIMERTHREAD = 0x00000020,
            WT_EXECUTEINPERSISTENTTHREAD = 0x00000080,
          //WT_TRANSFER_IMPERSONATION = 0x00000100
        }

        [DllImport("winmm.dll")]
        static extern void timeBeginPeriod(int resolution);

        [DllImport("winmm.dll")]
        static extern void timeEndPeriod(int resolution);

        [DllImport("kernel32.dll")]
        static extern bool CreateTimerQueueTimer(
            out IntPtr phNewTimer,          // phNewTimer - Pointer to a handle; this is an out value
            IntPtr TimerQueue,              // TimerQueue - Timer queue handle. For the default timer queue, NULL
            WaitOrTimerDelegate Callback,   // Callback - Pointer to the callback function
            IntPtr Parameter,               // Parameter - Value passed to the callback function
            uint DueTime,                   // DueTime - Time (milliseconds), before the timer is set to the signaled state for the first time 
            uint Period,                    // Period - Timer period (milliseconds). If zero, timer is signaled only once
            uint Flags                      // Flags - One or more of the next values (table taken from MSDN):

            // WT_EXECUTEINTIMERTHREAD 	The callback function is invoked by the timer thread itself. This flag should be used only for short tasks or it could affect other timer operations.
            // WT_EXECUTEINIOTHREAD 	The callback function is queued to an I/O worker thread. This flag should be used if the function should be executed in a thread that waits in an alertable state.
            // The callback function is queued as an APC. Be sure to address reentrancy issues if the function performs an alertable wait operation.
            // WT_EXECUTEINPERSISTENTTHREA The callback function is queued to a thread that never terminates. This flag should be used only for short tasks or it could affect other timer operations.
            // Note that currently no worker thread is persistent, although no worker thread will terminate if there are any pending I/O requests.
            // WT_EXECUTELONGFUNCTION 	Specifies that the callback function can perform a long wait. This flag helps the system to decide if it should create a new thread.
            // WT_EXECUTEONLYONCE 	The timer will be set to the signaled state only once.
            );

        [DllImport("kernel32.dll")]
        static extern bool DeleteTimerQueueTimer(
            IntPtr timerQueue,              // TimerQueue - A handle to the (default) timer queue
            IntPtr timer,                   // Timer - A handle to the timer
            IntPtr completionEvent          // CompletionEvent - A handle to an optional event to be signaled when the function is successful and all callback functions have completed. Can be NULL.
        );

        [DllImport("kernel32.dll")]
        static extern bool DeleteTimerQueue(IntPtr TimerQueue);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);

        public delegate void WaitOrTimerDelegate(IntPtr lpParameter, bool timerOrWaitFired);

        //[CLSCompliant(false)]
        public static void Create(uint dueTime, uint period, WaitOrTimerDelegate callbackDelegate)
        {
            if (callbackDelegate == null)
                throw new ArgumentNullException("callbackDelegate");

            // Make sure to hold on to a reference of the callback delegate
            delegateRef = callbackDelegate;

            // enable high precision
            timeBeginPeriod(RESOLUTION);

            IntPtr pParameter = IntPtr.Zero;

            bool success = CreateTimerQueueTimer(
                // Timer handle
                out phNewTimer,

                // Default timer queue. IntPtr.Zero is just a constant value that represents a null pointer.
                IntPtr.Zero,

                // Timer callback function
                callbackDelegate,

                // Callback function parameter
                pParameter,

                // Time (milliseconds), before the timer is set to the signaled state for the first time.
                dueTime,

                // Period - Timer period (milliseconds). If zero, timer is signaled only once.
                period,

                // Execute the callback in the IO thread
                (uint)Flag.WT_EXECUTEINIOTHREAD);

            if (!success)
                throw new QueueTimerException("Error creating QueueTimer");
        }

        public static void Delete()
        {
            //bool success = DeleteTimerQueue(IntPtr.Zero);

            if (delegateRef == null)
                return;

            bool success = DeleteTimerQueueTimer(
                    IntPtr.Zero, // TimerQueue - A handle to the (default) timer queue
                    phNewTimer,  // Timer - A handle to the timer
                    IntPtr.Zero  // CompletionEvent - A handle to an optional event to be signaled when the function is successful and all callback functions have completed. Can be NULL.
                );

            int error = Marshal.GetLastWin32Error();

            // disable high resolution
            timeEndPeriod(RESOLUTION);

            //CloseHandle(phNewTimer);
        }
    }

    public class QueueTimerException : Exception
    {
        public QueueTimerException(string message)
            : base(message)
        {

        }

        public QueueTimerException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
#endif