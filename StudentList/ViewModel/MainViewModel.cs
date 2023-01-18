using StudentList.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace StudentList.ViewModel
{
    public class MainViewModel : GeneralViewModel
    {
        public static MainViewModel ThisModel { get; set; }
        public  User ThisUser { get; private set; }
        public MainViewModel(User thisUser)
        {
            using var db = new ConnectDB();
            List<UserControl> users = new List<UserControl> { new View.Student.StudentMain(), new View.Group.GroupMain(), new View.Subject.SubjectMain(), new View.User.MainUser() };
            UserControls=new List<Tuple<int, string, UserControl>> { new Tuple<int, string, UserControl>(1, "AddStudent", users[0]), new Tuple<int, string, UserControl>(1, "StudentTable", users[0]), new Tuple<int, string, UserControl>(2, "AddGroup", users[1]), new Tuple<int, string, UserControl>(2, "GroupTable", users[1]), new Tuple<int, string, UserControl>(3, "AddSubject", users[2]), new Tuple<int, string, UserControl>(3, "TableSubject", users[2]), new Tuple<int, string, UserControl>(4, "AddUser", users[3]), new Tuple<int, string, UserControl>(4, "UserTable", users[3]) };
            ThisModel=this;
            OpenPageCommand("StudentTable");
            ThisUser=thisUser;
        }
    }
}
