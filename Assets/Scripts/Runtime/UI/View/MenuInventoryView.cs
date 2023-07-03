using UnityEngine;
using UnityEngine.UI;
using PaperSouls.Core;

namespace PaperSouls.Runtime.UI.View
{
    public class MenuInventoryView : InventoryView
    {
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _previousButton;

        private void OpenNext()
        {
            ViewManger.Show<SkillTreeView>(false);
        }

        private void OpenPrevious()
        {
            ViewManger.Show<SkillTreeView>(false);
        }

        public override void Init()
        {
            base.Init();
            _nextButton.onClick.AddListener(OpenNext);
            _previousButton.onClick.AddListener(OpenPrevious);
        }

        public override void Show()
        {
            base.Show();
            _invenotryView.SetActive(true);
            _dyanmicView.SetActive(false);
            _equipmentView.SetActive(true);
        }
    }
}
