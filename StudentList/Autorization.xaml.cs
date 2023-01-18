using StudentList.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StudentList
{
    /// <summary>
    /// Логика взаимодействия для Autorization.xaml
    /// </summary>
    public partial class Autorization : Window
    {
        private int error;
        private double time;
        private double end_time;
        private System.Timers.Timer tmr { get; set; }
        public Autorization()
        {
            error = 0;
            InitializeComponent();
            end_time=30;
            tmr = new System.Timers.Timer();
            tmr.Interval = 1000;
            tmr.Elapsed += Timer;
        }

        private void Timer(object sender, ElapsedEventArgs e)
        {
            time++;
            if (time>=end_time)
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    Auth.IsEnabled = true;
                });
                tmr.Stop();
            }
        }

        private void AuthClick(object sender, RoutedEventArgs e)
        {

            Model.ICommand command = new UserCommand<User>(new Model.User(Login.Text, Password.Password));
            var user=command.GetItem<User>(); 
            if (user == null)
            {
                error++;
                if (error==3)
                {
                    time=0;
                    Auth.IsEnabled = false;
                    tmr.Start();

                    MessageBox.Show("Вы ввели логин или  пароль неверно более 3х раз, доступ заблокирован на 30 секунд");

                }
                else
                    MessageBox.Show("Неверный логин или пароль");

            }
            else
            {
                
                MainWindow mainWindow = new MainWindow(user);
                mainWindow.Show();
                Close();

            }

        }
    }
}
