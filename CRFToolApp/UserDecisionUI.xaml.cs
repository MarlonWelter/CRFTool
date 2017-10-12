using CodeBase;
using System;
using System.Collections.Generic;
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
    public partial class UserDecisionUI : UserControl
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

        public void SetUserOptions(string[] options)
        {
            ViewModel.Options = options;
        }

    }
    public class UserDecisionViewModel : INotifyPropertyChanged
    {
        private string[] options = new string[] { "no options found." };

        public string[] Options
        {
            get { return options; }
            set
            {
                options = value;
                NotifyPropertyChanged("Options");
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
