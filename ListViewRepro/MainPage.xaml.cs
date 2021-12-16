using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ListViewRepro
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ItemsViewModel _itemsViewModel;

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _itemsViewModel = await ItemsViewModel.CreateAsync();
            ContactsCVS.Source = _itemsViewModel.Available;
            lvSelectedItems.ItemsSource = _itemsViewModel.Selected;
        }

        private void lvSelectedItems_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            if (e.Items.First() is Contact contact)
            {
                e.Data.SetText(contact.Name);
                e.Data.RequestedOperation = DataPackageOperation.Move;
            }
        }

        private void lvSelectedItems_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = (e.DataView.AvailableFormats.Contains(StandardDataFormats.Text)) ? DataPackageOperation.Move : DataPackageOperation.None;
        }

        private async void lvSelectedItems_Drop(object sender, DragEventArgs e)
        {
            var def = e.GetDeferral();
            string moved = await e.DataView.GetTextAsync();
            _itemsViewModel.MoveToSelected(moved);
            def.Complete();
        }

        private void lvAvailableItems_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            if (e.Items.First() is Contact contact)
            {
                e.Data.SetText(contact.Name);
                e.Data.RequestedOperation = DataPackageOperation.Move;
            }
        }

        private void lvAvailableItems_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = (e.DataView.AvailableFormats.Contains(StandardDataFormats.Text)) ? DataPackageOperation.Move : DataPackageOperation.None;
        }

        private async void lvAvailableItems_Drop(object sender, DragEventArgs e)
        {
            var def = e.GetDeferral();
            string moved = await e.DataView.GetTextAsync();
            _itemsViewModel.MoveToAvailable(moved);
            def.Complete();
        }
    }
}
