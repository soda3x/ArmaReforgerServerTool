/******************************************************************************
 * File Name:    BoundListBox.cs
 * Project:      Longbow
 * Description:  The BoundListBox component is an extension of the ListBox
 *               component with the addition of automatically refreshing its
 *               contents upon updates to the List Box's underlying data
 *               source.
 *               
 *               Performs the same as a List Box when the DataSource property
 *               is not set.
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

namespace ReforgerServerApp.Components
{
  internal class BoundListBox : ListBox
  {
    public new void RefreshItems()
    {
      SelectedIndex = -1;
      base.RefreshItems();
    }
  }
}
