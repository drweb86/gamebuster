using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using GameBuster.Annotations;

namespace GameBuster.ViewModel
{
    class CheckItem: DependencyObject, INotifyPropertyChanged
    {
        public CheckItem(string title, bool isChecked)
        {
            Title = title;
            IsChecked = isChecked;
        }

        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(
            "IsChecked", typeof (bool), typeof (CheckItem), new PropertyMetadata(default(bool)));

        public bool IsChecked
        {
            get { return (bool) GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof (string), typeof (CheckItem), new PropertyMetadata(default(string)));

        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
