using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StudentList.Model;
using StudentList.Model.Group;
using StudentList.View.Group;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace StudentList.ViewModel
{
    public class UserViewModel:GeneralViewModel
    {
        [Reactive]
        public ObservableCollection<User> Users { get; set; }
        [Reactive]
        public User NewUser { get; set; }
        public UserViewModel() {

            UserControls=new List<Tuple<int, string, UserControl>> { new Tuple<int, string, UserControl>(0, "AddUser", new View.User.UserAdd()), new Tuple<int, string, UserControl>(0, "UserTable", new View.User.UserTable()) };
            NewUser=new User(); 
        }
        protected override void OpenPageCommand(string i)
        {
            var user_contr = UserControls.Where(p => p.Item2==i).FirstOrDefault();
            if (user_contr == null)
                return;
            CurrentPage=user_contr.Item1;
            UpdateControl(user_contr.Item3);
            GetAllData();
        }
        private void GetAllData()
        {
            ICommand command = new UserCommand<User>();
            Users=command.GetItemList<User>();
        }
        public ReactiveCommand<Unit, Unit> AddUserCommand => ReactiveCommand.Create(() => {
            ICommand command = new UserCommand<User>(NewUser);
            command.Add();
            NewUser =new User(); });
        public ReactiveCommand<Model.User, Unit> DeleteUser => ReactiveCommand.Create<Model.User>(DeleteUserCommand);
        private void DeleteUserCommand(Model.User user)
        {
            if (user==null)
                return;
            if (user.Id==1)
            {
                MessageBox.Show("Удалить администратора нельзя", "Ошибка");
                return;
            }
            ICommand command = new UserCommand<User>(user);
            command.Remove();
            GetAllData();
        }
        public ReactiveCommand<Unit, Unit> CanselUser => ReactiveCommand.Create(() => GetAllData());
        public ReactiveCommand<Unit, Unit> SaveUser => ReactiveCommand.Create(() =>
        {
            ICommand command = new UserCommand<ObservableCollection<User>>(Users);
            command.Update();
            GetAllData();
        });
    }
}
