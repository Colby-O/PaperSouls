using UnityEngine;
using UnityEngine.UI;
using PaperSouls.Core;
using PaperSouls.Runtime.MonoSystems.UI;

namespace PaperSouls.Runtime.UI.View
{
    public class ControlSettingsView : View
    {
        [SerializeField] private Button _backButton;

        public override void Init()
        {
            _backButton.onClick.AddListener(GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast);
        }
    }
}
