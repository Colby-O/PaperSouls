using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PaperSouls.Core;
using PaperSouls.Runtime.MonoSystems.UI;

namespace PaperSouls.Runtime.UI.View
{
    internal sealed class SkillTreeView : View
    {
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _previousButton;

        private void OpenNext()
        {
            GameManager.GetMonoSystem<IUIMonoSystem>().Show<MenuInventoryView>(false);
        }

        private void OpenPrevious()
        {
            GameManager.GetMonoSystem<IUIMonoSystem>().Show<MenuInventoryView>(false);
        }

        public override void Init()
        {
            _nextButton.onClick.AddListener(OpenNext);
            _previousButton.onClick.AddListener(OpenPrevious);
        }
    }
}
