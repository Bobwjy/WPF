using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Lync.Model;

namespace Easi365UI.Lync
{
    public class LyncContract : INotifyPropertyChanged
    {
        Dispatcher _dispatcher;
        public Contact _Contact;
        public LyncContract(Contact contact, Dispatcher dispatcher)
        {
            _Contact = contact;
            _Contact.ContactInformationChanged += Contact_ContactInformationChanged;
            _dispatcher = dispatcher;

            this.Uri = contact.Uri;
            this.GetContactInfo();
        }
        public LyncGroup _Group;
        public LyncContract(Contact contact, LyncGroup group, Dispatcher dispatcher)
        {
            _Contact = contact;
            _Group = group;
            _Contact.ContactInformationChanged += Contact_ContactInformationChanged;
            _dispatcher = dispatcher;

            this.Uri = contact.Uri;
            this.GetContactInfo();
        }

        private string uri;
        public string Uri
        {
            get { return uri; }
            set { uri = value; OnPropertyChanged("Uri"); }
        }

        private string displayName;
        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; OnPropertyChanged("DisplayName"); }
        }

        private string state;
        public string State
        {
            get { return state; }
            set { state = value; OnPropertyChanged("State"); }
        }

        private string personNote;
        public string PersonNote
        {
            get { return personNote; }
            set { personNote = value; OnPropertyChanged("PersonNote"); }
        }

        private Brush color;
        public Brush Color
        {
            get { return color; }
            set { color = value; OnPropertyChanged("Color"); }
        }

        private BitmapImage photo;
        public BitmapImage Photo
        {
            get { return photo; }
            set { photo = value; OnPropertyChanged("Photo"); }
        }

        void GetDisplayName()
        {
            try
            {
                this.DisplayName = _Contact.GetContactInformation(ContactInformationType.DisplayName).ToString();
            }
            catch { }
        }

        void GetState()
        {
            try
            {
                this.State = _Contact.GetContactInformation(ContactInformationType.Activity).ToString();
            }
            catch { }
        }

        void GetPersonalNote()
        {
            try
            {
                this.PersonNote = _Contact.GetContactInformation(ContactInformationType.PersonalNote).ToString();
            }
            catch { }
        }

        void GetColor()
        {
            try
            {
                ContactAvailability currentAvailability = 0;
                currentAvailability = (ContactAvailability)_Contact.GetContactInformation(ContactInformationType.Availability);
                if (currentAvailability != 0)
                {
                    Brush availabilityColor;
                    switch (currentAvailability)
                    {
                        case ContactAvailability.Away:
                            availabilityColor = Brushes.Yellow;
                            break;
                        case ContactAvailability.Busy:
                            availabilityColor = Brushes.Red;
                            break;
                        case ContactAvailability.BusyIdle:
                            availabilityColor = Brushes.Red;
                            break;
                        case ContactAvailability.DoNotDisturb:
                            availabilityColor = Brushes.DarkRed;
                            break;
                        case ContactAvailability.Free:
                            availabilityColor = Brushes.LimeGreen;
                            break;
                        case ContactAvailability.FreeIdle:
                            availabilityColor = Brushes.LimeGreen;
                            break;
                        case ContactAvailability.Offline:
                            availabilityColor = Brushes.LightSlateGray;
                            break;
                        case ContactAvailability.TemporarilyAway:
                            availabilityColor = Brushes.Yellow;
                            break;
                        default:
                            availabilityColor = Brushes.LightSlateGray;
                            break;
                    }
                    this.Color = availabilityColor;
                }
            }
            catch { }
        }

        void GetPhoto()
        {
            try
            {
                using (Stream photoStream = _Contact.GetContactInformation(ContactInformationType.Photo) as Stream)
                {
                    BitmapImage bm = new BitmapImage();
                    bm.BeginInit();
                    bm.StreamSource = photoStream;
                    bm.EndInit();
                    this.Photo = bm;
                }
            }
            catch
            {
                try
                {
                    Uri uri = new Uri("pack://application:,,,/Assets/Images/photo.png");
                    this.Photo = new BitmapImage(uri);
                }
                catch { }
            }
        }

        public void GetContactInfo()
        {
            _dispatcher.Invoke(new Action(delegate()
            {
                GetDisplayName();
                GetState();
                GetPersonalNote();
                GetColor();
                GetPhoto();
            }), null);
        }

        void Contact_ContactInformationChanged(object sender, ContactInformationChangedEventArgs e)
        {
            if (e.ChangedContactInformation.Contains(ContactInformationType.DisplayName) ||
                e.ChangedContactInformation.Contains(ContactInformationType.Availability) ||
                e.ChangedContactInformation.Contains(ContactInformationType.PersonalNote) ||
                e.ChangedContactInformation.Contains(ContactInformationType.Photo))
            {
                GetContactInfo();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }

    public class LyncContractComparer : IEqualityComparer<LyncContract>
    {
        public static LyncContractComparer Default = new LyncContractComparer();

        public bool Equals(LyncContract x, LyncContract y)
        {
            return x.Uri.Equals(y.Uri);
        }

        public int GetHashCode(LyncContract obj)
        {
            return obj.GetHashCode();
        }
    }
}
