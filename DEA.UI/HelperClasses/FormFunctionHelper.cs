namespace DEA.UI.HelperClasses
{
    internal class FormFunctionHelper
    {
        public void CheckBoxListHandler(object sender, ItemCheckEventArgs e)
        {
            if (sender is not CheckedListBox checkedListBox) return;

            // Unsubscribe from the ItemCheck event to avoid recursive calls
            checkedListBox.ItemCheck -= CheckBoxListHandler;

            // If the first item is checked then check everything else.
            if (e.Index == 0 && e.NewValue == CheckState.Checked)
            {
                // Iterate through the list and check everything
                for (int i = 1; i < checkedListBox.Items.Count; i++)
                {
                    checkedListBox.SetItemChecked(i, true);
                }
            }
            else if (e.Index == 0 && e.NewValue == CheckState.Unchecked)
            {
                for (int i = 1; i < checkedListBox.Items.Count; i++)
                {
                    checkedListBox.SetItemChecked(i, false);
                }
            }

            // Resubscribe to the ItemCheck event
            checkedListBox.ItemCheck += CheckBoxListHandler;
        }
    }
}
