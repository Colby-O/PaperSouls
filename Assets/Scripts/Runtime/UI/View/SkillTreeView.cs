using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PaperSouls.Core;

namespace PaperSouls.Runtime.UI.View
{
    public class SkillTreeView : View
    {
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _previousButton;

        private void OpenNext()
        {
            ViewManger.Show<MenuInventoryView>(false);
        }

        private void OpenPrevious()
        {
            ViewManger.Show<MenuInventoryView>(false);
        }

        public override void Init()
        {
            _nextButton.onClick.AddListener(OpenNext);
            _previousButton.onClick.AddListener(OpenPrevious);
        }
    }
}