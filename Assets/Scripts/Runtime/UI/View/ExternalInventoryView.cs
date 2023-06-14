namespace PaperSouls.Runtime.UI.View
{
    public class ExternalInventoryView : InventoryView
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
