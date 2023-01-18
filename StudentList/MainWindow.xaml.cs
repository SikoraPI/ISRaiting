using StudentList.Model;
using System;
using System.Collections.Generic;
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

namespace StudentList
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViewModel.MainViewModel viewModel;
        public MainWindow(User user)
        {
            InitializeComponent();
            viewModel=new ViewModel.MainViewModel(user);
            DataContext =viewModel;
        }
        private void change_Click(object sender, RoutedEventArgs e)
        {
            Autorization autorization = new Autorization();
            autorization.Show();
            Close();
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
