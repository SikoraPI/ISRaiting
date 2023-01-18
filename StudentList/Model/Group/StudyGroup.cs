using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StudentList.Model.Subject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StudentList.Model.Group
{
    public class StudyGroup: MainObject
    {
        [Reactive]
        [System.Xml.Serialization.XmlIgnore]
        public ObservableCollection<Student.Student> Students { get; set; }
        [NotMapped]
        public double MiddleGrades { get; set; }
        public StudyGroup()
        {

        }
        public StudyGroup(int id,string name)
        {
            this.Id=id;
            this.Name=name;
            using var db = new ConnectDB();
            if(db.StudentGrades.Include(p => p.Student).Any(p => p.Student.StudyGroupId==Id))
                MiddleGrades=db.StudentGrades.Include(p => p.Student).Where(p => p.Student.StudyGroupId==Id).Average(p => p.Grades);
        }
    }
    public class StudyGroupCommand<Y> : ICommand
    {
        private ConnectDB db = new ConnectDB();
        private Y group;
        public StudyGroupCommand(Y Group)
        {
            this.group = Group;
        }
        public StudyGroupCommand()
        {

        }
        public bool Add()
        {
            if (group is not StudyGroup)
                return false;
            var gr=group as StudyGroup;
            if(db.Groups.Any(p=>p.Name==gr.Name))
            {
                MessageBox.Show("Такая группа уже существует", "Ошибка");
                return false;
            }
            db.Add(group);
            db.SaveChanges();
            return true;
        }

        public T GetItem<T>()
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<T> GetItemList<T>()
        {
            if (typeof(StudyGroup).IsAssignableFrom(typeof(T)) && group is string group_find )
                return new ObservableCollection<T>(db.Groups.Where(p => p.Name.ToLower().Contains(group_find)).Select(p => new StudyGroup(p.Id, p.Name)) as IEnumerable<T>);
            if (group ==  null)
                return new ObservableCollection<T>(db.Groups.Select(p=>new StudyGroup(p.Id, p.Name))  as IEnumerable<T>);
            return null;
        }

        public bool Remove()
        {
            if (group is not StudyGroup)
                    return false;
            db.Remove(group);
            db.SaveChanges();
            return true;
        }
        public bool Update()
        {
            if(group is ObservableCollection<StudyGroup> groups)
            {
                db.UpdateRange(groups);
                db.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
