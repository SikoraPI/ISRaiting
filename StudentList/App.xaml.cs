using StudentList.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace StudentList
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            int year=DateTime.Now.AddYears(-1).Year;
            for(int i=year;i<=year+2;i++)
            {
                HaveSession(i);
            }

        }
        [STAThread]
        static void Main()
        {

            App app = new App();
            Autorization window = new Autorization();
            app.Run(window);
        }
        private void HaveSession(int year)
        {
            using var DB = new ConnectDB();
            if (!DB.StudentSessions.Any(p=>p.Name==year && p.WinterSummer==false))
                DB.Add(new Model.Subject.StudySession { Name=year, WinterSummer=false });
            if (!DB.StudentSessions.Any(p => p.Name==year && p.WinterSummer==true))
                DB.Add(new Model.Subject.StudySession { Name=year, WinterSummer=true });
            DB.SaveChanges();
        }
    }
}
