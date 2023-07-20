namespace PaperSouls.Runtime.UI.View
{
    internal sealed class ExternalInventoryView : InventoryView
    {
        public override void Show()
        {
            base.Show();
            _invenotryView.SetActive(true);
            _dyanmicView.SetActive(true);
            _equipmentView.SetActive(false);
        }
    }
}
