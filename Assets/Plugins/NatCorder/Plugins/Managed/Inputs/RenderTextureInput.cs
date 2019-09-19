/* 
*   NatCorder
*   Copyright (c) 2019 Yusuf Olokoba
*/

namespace NatCorder.Inputs {

    using AOT;
    using UnityEngine;
    using UnityEngine.Rendering;
    using System;
    using System.Runtime.InteropServices;
    using Clocks;
    using Internal;

    /// <summary>
    /// Recorder input for recording video frames from RenderTextures
    /// </summary>
    [Doc(@"RenderTextureInput")]
    public sealed class RenderTextureInput : IDisposable {

        #region --Client API--

        /// <summary>
        /// Create a recorder input for RenderTextures
        /// </summary>
        /// <param name="mediaRecorder">Media recorder to receive committed frames</param>
        /// <param name="clock">Clock for generating timestamps</param>
        [Doc(@"RenderTextureInputCtor")]
        public RenderTextureInput (IMediaRecorder mediaRecorder) {
            // Construct state
            this.mediaRecorder = mediaRecorder;
            this.pixelBuffer = new byte[mediaRecorder.pixelWidth * mediaRecorder.pixelHeight * 4];
            this.readbackBuffer = new Texture2D(mediaRecorder.pixelWidth, mediaRecorder.pixelHeight, TextureFormat.RGBA32, false, false);
            this.readbackiOS = MTLReadbackCreate(mediaRecorder.pixelWidth, mediaRecorder.pixelHeight);
        }

        /// <summary>
        /// Stop recorder input and teardown resources
        /// </summary>
        [Doc(@"AudioInputDispose")]
        public void Dispose () {
            Texture2D.Destroy(readbackBuffer);
            lock (this)
                disposed = true;
            using (var dispatcher = new RenderDispatcher())
                dispatcher.Dispatch(() => MTLReadbackDispose(readbackiOS));
        }

        /// <summary>
        /// Commit a video frame from a RenderTexture
        /// </summary>
        /// <param name="framebuffer">RenderTexture containing video frame to commit</param>
        /// <param name="timestamp">Frame timestamp in nanoseconds</param>
        [Doc(@"RenderTextureInputCommitFrame")]
        public void CommitFrame (RenderTexture framebuffer, long timestamp) {
            if (Application.platform == RuntimePlatform.Android)
                CommitAndroid(framebuffer, timestamp);
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
                CommitiOS(framebuffer, timestamp);
            else if (SystemInfo.supportsAsyncGPUReadback)
                CommitAsync(framebuffer, timestamp);
            else
                CommitSync(framebuffer, timestamp);
        }
        #endregion


        #region --Operations--

        private readonly IMediaRecorder mediaRecorder;
        private readonly byte[] pixelBuffer;
        private readonly Texture2D readbackBuffer;
        private readonly IntPtr readbackiOS;
        private bool disposed;

        private void CommitAndroid (RenderTexture framebuffer, long timestamp) {
            /*
             * Because Android is terrible at everything, we have to *break* the IMediaRecorder contract and directly
             * send the framebuffer texture to the native recorder for encoding. We wouldn't have to do this if 
             * Android OEM's properly implemented PBO's, WHICH SHOULD NOT BLOCK with `glReadPixels`. So infuriating.
             */
            var nativeRecorder = ((mediaRecorder as IAbstractRecorder).recorder as MediaRecorderAndroid).recorder;
            var textureID = framebuffer.GetNativeTexturePtr().ToInt32();
            if (mediaRecorder is MP4Recorder || mediaRecorder is HEVCRecorder) // GIFRecorder can't use this trick
                using (var dispatcher = new RenderDispatcher())
                    dispatcher.Dispatch(() => {
                        AndroidJNI.AttachCurrentThread();
                        lock (this)
                            if (!disposed)
                                nativeRecorder.Call(@"encodeFrame", textureID, timestamp);
                    });
            else
                CommitSync(framebuffer, timestamp); // For GIFRecorder, do standard readback
        }

        private void CommitiOS (RenderTexture framebuffer, long timestamp) {
            var _ = framebuffer.colorBuffer;
            var texturePtr = framebuffer.GetNativeTexturePtr();
            Action<IntPtr> commitFrame = pixelBuffer => {
                lock (this)
                    if (!disposed)
                        mediaRecorder.CommitFrame(pixelBuffer, timestamp);
            };
            using (var dispatcher = new RenderDispatcher())
                dispatcher.Dispatch(() => MTLReadbackReadback(readbackiOS, texturePtr, OnReadback, (IntPtr)GCHandle.Alloc(commitFrame, GCHandleType.Normal)));
        }

        private void CommitSync (RenderTexture framebuffer, long timestamp) {
            RenderTexture.active = framebuffer;
            readbackBuffer.ReadPixels(new Rect(0, 0, readbackBuffer.width, readbackBuffer.height), 0, 0, false);
            readbackBuffer.GetRawTextureData<byte>().CopyTo(pixelBuffer);
            mediaRecorder.CommitFrame(pixelBuffer, timestamp);
        }

        private void CommitAsync (RenderTexture framebuffer, long timestamp) {
            AsyncGPUReadback.Request(framebuffer, 0, request => {
                request.GetData<byte>().CopyTo(pixelBuffer);
                if (!disposed) // No need to synchronize, invoked on main thread
                    mediaRecorder.CommitFrame(pixelBuffer, timestamp);
            });
        }
        #endregion


        #region --Bridge--

        #if UNITY_IOS && !UNITY_EDITOR
        [DllImport(@"__Internal", EntryPoint = @"NCMTLReadbackCreate")]
        private static extern IntPtr MTLReadbackCreate (int width, int height);
        [DllImport(@"__Internal", EntryPoint = @"NCMTLReadbackDispose")]
        private static extern void MTLReadbackDispose (IntPtr readback);
        [DllImport(@"__Internal", EntryPoint = @"NCMTLReadbackReadback")]
        private static extern void MTLReadbackReadback (IntPtr readback, IntPtr texture, Action<IntPtr, IntPtr> handler, IntPtr context);
        #else
        private static IntPtr MTLReadbackCreate (int width, int height) { return IntPtr.Zero; }
        private static void MTLReadbackDispose (IntPtr readback) {}
        private static void MTLReadbackReadback (IntPtr readback, IntPtr texture, Action<IntPtr, IntPtr> handler, IntPtr context) {}
        #endif

        [MonoPInvokeCallback(typeof(Action<IntPtr, IntPtr>))]
        private static void OnReadback (IntPtr context, IntPtr pixelBuffer) {
            var handle = (GCHandle)context;
            Action<IntPtr> commitFrame = handle.Target as Action<IntPtr>;
            handle.Free();
            commitFrame(pixelBuffer);
        }
        #endregion
    }
}