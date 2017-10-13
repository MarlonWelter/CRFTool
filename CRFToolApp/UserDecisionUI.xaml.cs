using CodeBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CRFToolApp
{
    /// <summary>
    /// Interaktionslogik für UserDecision.xaml
    /// </summary>

    public partial class UserDecisionUI : Window
    {
        private UserDecisionViewModel viewModel;

        public UserDecisionViewModel ViewModel
        {
            get { return viewModel; }
            set
            {
                viewModel = value;
                DataContext = viewModel;
            }
        }

        public UserDecisionUI()
        {
            InitializeComponent();
            ViewModel = new UserDecisionViewModel();
        }

        public void SetUserOptions(UserDecision request)
        {
            foreach (var item in request.Options)
            {
                ViewModel.Options.Add(item);
            }
            ViewModel.Request = request;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Request.Decision = listBox.SelectedIndex;
            this.DialogResult = true;
        }
    }
    public class UserDecisionViewModel : INotifyPropertyChanged
    {
        public UserDecision Request { get; set; }
        private ObservableCollection<string> options = new ObservableCollection<string>();

        public ObservableCollection<string> Options
        {
            get { return options; }
            set
            {
                options = value;
            }
        }



        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion

    }
}
