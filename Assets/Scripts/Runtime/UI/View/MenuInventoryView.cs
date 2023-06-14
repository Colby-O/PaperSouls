namespace PaperSouls.Runtime.UI.View
{
    public class MenuInventoryView : InventoryView
    {
        public override void Show()
        {
            base.Show();
            _invenotryView.SetActive(true);
            _dyanmicView.SetActive(false);
            _equipmentView.SetActive(true);
        }
    }
}
