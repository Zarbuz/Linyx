using System;
using strange.extensions.mediation.impl;
using TMPro;
using UnityEngine;

namespace Linyx.Views.Record
{
    public sealed class RecordView : View
    {
        #region Serialize Fields

        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private TextMeshProUGUI _title;

        #endregion


        private void Update()
        {
            if (_audioSource.isPlaying)
            {
                _title.SetText($"Recording in progress ({Linyx.Utils.Utils.FromSecondsToMinutesAndSeconds(_audioSource.time)}/{Linyx.Utils.Utils.FromSecondsToMinutesAndSeconds(_audioSource.clip.length)})");
            }
        }
    }
}
