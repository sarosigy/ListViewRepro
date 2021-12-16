using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace ListViewRepro
{
    internal class ItemsViewModel
    {
        public ObservableCollection<GroupInfoList> Available { get; set; }
        public ObservableCollection<Contact> Selected { get; set; }

        private ItemsViewModel() { }

        public static async Task<ItemsViewModel> CreateAsync()
        {
            ItemsViewModel itemsViewModel = new ItemsViewModel();
            itemsViewModel.Available = await GetContactsGroupedAsync();
            itemsViewModel.Selected = new ObservableCollection<Contact>();

            return itemsViewModel;
        }

        // C# Code

        // To create a collection of grouped items, create a query that groups
        // an existing list, or returns a grouped collection from a database.
        // The following method is used to create the ItemsSource for our CollectionViewSource:

        public static async Task<ObservableCollection<GroupInfoList>> GetContactsGroupedAsync()
        {
            // Grab Contact objects from pre-existing list (list is returned from function GetContactsAsync())
            var query = from item in await GetContactsAsync()

                            // Group the items returned from the query, sort and select the ones you want to keep
                        group item by item.LastName.Substring(0, 1).ToUpper() into g
                        orderby g.Key

                        // GroupInfoList is a simple custom class that has an IEnumerable type attribute, and
                        // a key attribute. The IGrouping-typed variable g now holds the Contact objects,
                        // and these objects will be used to create a new GroupInfoList object.
                        select new GroupInfoList(g) { Key = g.Key };
            
            return new ObservableCollection<GroupInfoList>(query);
        }

        public static async Task<ObservableCollection<Contact>> GetContactsAsync()
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Contacts.txt", UriKind.Absolute)).AsTask().ConfigureAwait(false);
            IList<string> lines = await FileIO.ReadLinesAsync(file);

            ObservableCollection<Contact> contacts = new ObservableCollection<Contact>();

            for (int i = 0; i < lines.Count; i += 3)
            {
                contacts.Add(new Contact(lines[i], lines[i + 1], lines[i + 2]));
            }
            return contacts;
        }

        private (GroupInfoList, Contact) FindInAvailable(string name)
        {
            foreach(var gitem in Available)
                foreach(var item in gitem)
                    if (item is Contact c && c.Name == name)
                        return (gitem, c);
            return (null, null);
        }

        internal void MoveToSelected(string moved)
        {
            (GroupInfoList groupInfo, Contact contact) = FindInAvailable(moved);
            if (groupInfo != null)
            {
                groupInfo.Remove(contact);
                if (groupInfo.Count == 0)
                    Available.Remove(groupInfo);
                Selected.Add(contact);
            }
        }

        internal void MoveToAvailable(string moved)
        {
            Contact contact = Selected.FirstOrDefault(i => i.Name == moved);
            string key = contact.LastName.Substring(0, 1).ToUpper();
            Selected.Remove(contact);
            
            GroupInfoList list = Available.FirstOrDefault(i => (string)i.Key == key);
            if (list == null)
            {
                list = new GroupInfoList(new[] { contact }) { Key = key };
                Available.Add(list);
            }
            else
            {
                list.Add(contact);
            }
        }
    }

    // GroupInfoList class definition:
    public class GroupInfoList : ObservableCollection<object>
    {
        public GroupInfoList(IEnumerable<object> items) : base(items)
        {
        }
        public object Key { get; set; }
    }

    // Contact class definition:
    public class Contact
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Company { get; private set; }
        public string Name => FirstName + " " + LastName;

        public Contact(string firstName, string lastName, string company)
        {
            FirstName = firstName;
            LastName = lastName;
            Company = company;
        }

        // ... Methods ...
    }
}
