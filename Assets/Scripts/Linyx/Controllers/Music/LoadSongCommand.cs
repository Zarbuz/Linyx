using Linyx.Services.Audio;
using NAudio.Wave;
using SFB;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;
using System;
using System.Threading.Tasks;
using MaterialUI;
using UnityEngine;

namespace Linyx.Controllers.Music
{
    public sealed class LoadSongCommand : Command
    {
        [Inject] public IAudioPeerService AudioPeerService { get; set; }
        [Inject] public SongLoadedSignal SongLoadedSignal { get; set; }
        public override async void Execute()
        {
            ExtensionFilter[] extensions = {
                new ExtensionFilter("Sound Files", "mp3", "wav" ),
            };

            string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
            if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
            {
                string filepath = paths[0];
                DialogProgress dialog = DialogManager.ShowProgressLinear("Loading " + filepath, "Loading music", MaterialIconHelper.GetIcon(MaterialIconEnum.INFO_OUTLINE));
                Tuple<AudioFileReader, float[]> data = await Task.Run(() => ReadAudioFile(filepath));
                AudioClip clip = AudioClip.Create(filepath, (int)(data.Item1.Length / sizeof(float) / data.Item1.WaveFormat.Channels), data.Item1.WaveFormat.Channels, data.Item1.WaveFormat.SampleRate, false);
                clip.SetData(data.Item2, 0);

                dialog.Hide();
                SongLoadedSignal.Dispatch(clip);
                AudioPeerService.SetupAudioSource(clip);
            }
        }

        private Tuple<AudioFileReader, float[]> ReadAudioFile(string path)
        {
            AudioFileReader audioFile = new AudioFileReader(path);
            float[] audioData = new float[audioFile.Length];
            audioFile.Read(audioData, 0, audioData.Length);
            return new Tuple<AudioFileReader, float[]>(audioFile, audioData);
        }

    }

    public sealed class SongLoadedSignal : Signal<AudioClip> { }
    public sealed class LoadSongSignal : Signal { }
}
