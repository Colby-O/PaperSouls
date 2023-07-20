using UnityEngine;

namespace PaperSouls.Runtime.UI.View
{
    internal abstract class InventoryView : View
    {
        [SerializeField] protected GameObject _invenotryView;
        [SerializeField] protected GameObject _dyanmicView;
        [SerializeField] protected GameObject _equipmentView;

        public override void Init() { }

        public override void Hide()
        {
            base.Hide();
            _invenotryView.SetActive(false);
            _dyanmicView.SetActive(false);
            _equipmentView.SetActive(false);
        }

        public override void Show()
        {
            base.Show();
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
    }
}
