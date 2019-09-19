/* 
*   NatCorder
*   Copyright (c) 2019 Yusuf Olokoba
*/

namespace NatCorder.Internal {

    using AOT;
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public interface IDispatcher : IDisposable {
        void Dispatch (Action workload);
    }

    public class MainDispatcher : IDispatcher { // Must be constructed on the Unity main thread

        private readonly DispatcherAttachment attachment;

        public MainDispatcher () {
            this.attachment = new GameObject("NatCorder Dispatch Utility").AddComponent<DispatcherAttachment>();
        }

        public void Dispose () {
            attachment.Dispose();
        }

        public void Dispatch (Action workload) {
            attachment.Dispatch(workload);
        }
        
        private class DispatcherAttachment : MonoBehaviour, IDispatcher {
            
            private readonly Queue<Action> queue = new Queue<Action>();

            public void Dispose () {
                Destroy(this);
                Destroy(this.gameObject);
                queue.Clear();
            }

            public void Dispatch (Action workload) {
                lock ((queue as ICollection).SyncRoot)
                    queue.Enqueue(workload);
            }

            void Awake () {
                DontDestroyOnLoad(this.gameObject);
                DontDestroyOnLoad(this);
            }

            void Update () {
                for (;;)
                    lock ((queue as ICollection).SyncRoot)
                        if (queue.Count > 0)
                            queue.Dequeue()();
                        else
                            break;
            }
        }
    }

    public class RenderDispatcher : IDispatcher {

        public void Dispose () { } // Nop

        public void Dispatch (Action workload) {
            switch (Application.platform) {
                case RuntimePlatform.Android:
                case RuntimePlatform.IPhonePlayer:
                    var renderDelegateHandle = Marshal.GetFunctionPointerForDelegate(new UnityRenderingEvent(DequeueRender));
                    var contextHandle = (IntPtr)GCHandle.Alloc(workload, GCHandleType.Normal);
                    GL.IssuePluginEvent(renderDelegateHandle, contextHandle.ToInt32());
                    break;
                default: // This dispatcher shouldn't be used on other platforms
                    break;
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void UnityRenderingEvent (int context);

        [MonoPInvokeCallback(typeof(UnityRenderingEvent))]
        private static void DequeueRender (int context) {
            GCHandle handle = (GCHandle)(IntPtr)context;
            Action target = handle.Target as Action;
            handle.Free();
            target();
        }
    }
}