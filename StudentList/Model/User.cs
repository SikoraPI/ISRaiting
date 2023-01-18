using ReactiveUI.Fody.Helpers;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentList.Model.Student;
using System.Collections.ObjectModel;

namespace StudentList.Model
{
    public class User 
    {
        [Reactive]
        public int Id { get; set; }
        public string Login { get; set; }   
        [Reactive]
        public string Password { get; set; }
        [Reactive]
        public bool IsAdmin { get; set; }
       
        public User()
        {
        }
        public User(string login, string password)
        {
            Login = login;
            Password = password;
        }
       
    }
    public class UserCommand<Y> : ICommand
    {
        private Y user;
        private ConnectDB db = new ConnectDB();
        public UserCommand(Y student)
        {
            
            this.user= student;
        }
        public UserCommand() { }
        public bool Add()
        {
            if(user is not User)
                return false;
            db.Add(user);
            db.SaveChanges();
            return true;
        }

        public T GetItem<T>()
        {
           if(user is User us)
            {
                return (T)(object)db.User.Where(p => p.Login==us.Login && p.Password==us.Password).FirstOrDefault();
            }
            return default(T);
        }

        public ObservableCollection<T> GetItemList<T>()
        {

            return new ObservableCollection<T>(db.User as IEnumerable<T>);

        }

        public bool Remove()
        {
            if (user is not User)
                return false;
            db.Remove(user);
            db.SaveChanges();
            return true;

        }

        public bool Update()
        {
            if (user is ObservableCollection<User> users)
            {
                var admin = users.Where(p => p.Id==1).FirstOrDefault();
                if(admin!=null) admin.IsAdmin= true;
                db.UpdateRange(users);
                db.SaveChanges();
            }

            return false;
        }
    }
}
