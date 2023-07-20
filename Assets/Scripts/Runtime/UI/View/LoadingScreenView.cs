using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PaperSouls.Core;

namespace PaperSouls.Runtime.UI.View
{
    internal sealed class LoadingScreenView : View
    {
        [SerializeField] private Slider _progressBar;

        public void IncreaseProgressBar(float progress)
        {
            _progressBar.value = progress;
        }

        public override void Init()
        {
            _progressBar.value = 0;
        }
    }
}
