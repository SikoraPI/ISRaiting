using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StudentList.Model.Subject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Management.Instrumentation;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StudentList.Model.Student
{
   public class Student:ReactiveObject
    {
        [Reactive]
        public int Id { get; set; }
        [Reactive]
        public string FirstName { get; set; }
        [Reactive]
        public string LastName { get; set; }
        [Reactive]
        public string SecondName { get; set; }
        [Reactive]
        public DateTime DateBirth { get; set; }
        [Reactive]
        public string CardNumber { get; set; }
        [Reactive]
        public Group.StudyGroup StudyGroup { get; set; }
        [Reactive]
        public int StudyGroupId { get; set; }
        public ObservableCollection<StudentGrades> StudentGrades { get; set; }
        [NotMapped]
        public string FIO { get => $"{FirstName} {SecondName} {LastName}"; }
        public Student()
        {
            DateBirth= DateTime.Now;
    
           
        }
    }
    public class StudentGrades : ReactiveObject
    {
        [Reactive]
        public int Id { get; set; }
        private int _grades;
        public int Grades
        {
            get => _grades;
            set
            {
                if (value<2)
                    value=2;
                if (value>5)
                    value=5;
                this.RaiseAndSetIfChanged(ref _grades, value);
            }
        }
        [System.Xml.Serialization.XmlIgnore]
        [Reactive]
        public Student Student { get; set; }
        [Reactive]
        public int StudentId { get; set; }
        [Reactive]
        public Model.Subject.StudySession StudySession { get; set; }
        [Reactive]
        public int StudySessionId { get; set; }
        [Reactive]
        public Model.Subject.StudySubject StudySubject { get; set; }
        [Reactive]
        public int StudySubjectId { get; set; }
        public StudentGrades ()
        {
            
        } 
        public StudentGrades(int stud_id)
        {
            StudentId= stud_id; Grades=2;
        }
        public static string GetGradString(int i)
        {
            switch(i)
            {
                case 2:

                    return "Неудовлетворительно";
                case 3:
                    return "Удовлетворительно";
                case 4:
                    return "Хорошо";
                case 5:
                    return "Отлично";
            }
            return "";
        }
    }
    public class StudentFind : ReactiveObject
    {
        public string Find { get; set; }
        public int GroupId { get; set; }
        public StudentFind()
        {
            Find="";
            GroupId=0;
        }
    }
    public class StudentGradesFind
    {
        public int GroupId { get; set; }
        public int SessionId { get; set; }
        public StudentGradesFind(int groupId, int sessionId)
        {
            GroupId=groupId;
            SessionId=sessionId;
        }
    }
    public class StudentCommand<Y> : ICommand
    {
        private Y student;
        private ConnectDB db;
        public StudentCommand(Y student)
        {
            db = new ConnectDB();
            this.student= student;
        }
        public bool Add()
        {
            if (student is Student stud)
            {
                if (db.Students.Any(p => p.CardNumber==stud.CardNumber))
                {
                    MessageBox.Show("Студент с таким номером зачетки уже существует", "Ошибка");
                    return false;
                }
                db.Add(student); 
                db.SaveChanges();
            }
            if (student is StudentGrades st_grad)
            {
                if (db.StudentGrades.Any(p=>p.StudySubjectId==st_grad.StudySubjectId && p.StudySessionId==st_grad.StudySessionId && p.StudentId==st_grad.StudentId))
                {
                    MessageBox.Show("Оценка по данному предмету уже выставлена", "Ошибка");
                    return false;
                }

                db.Add(student);
                db.SaveChanges();
            }

            return false;
        }

        public T GetItem<T>()
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<T> GetItemList<T>()
        {
            if (student is StudentGrades  stud_grad)
                return new ObservableCollection<T>(db.StudentGrades.Include(p => p.StudySubject).Where(p=>p.StudySessionId==stud_grad.StudySessionId && p.StudentId==stud_grad.StudentId).ToList() as List<T>);
            if (student==null)
                return new ObservableCollection<T>(db.Students.Include(p=>p.StudyGroup).ToList() as List<T>);
                 if(student is StudentFind find )
                return new ObservableCollection<T>(db.Students.Include(p => p.StudyGroup).Include(p=>p.StudentGrades).ThenInclude(p=>p.StudySubject).Include(p=>p.StudentGrades).ThenInclude(p=>p.StudySession).Where(p => (p.FirstName.ToLower().Contains(find.Find.ToLower()) || p.LastName.ToLower().Contains(find.Find.ToLower()) || p.SecondName.ToLower().Contains(find.Find.ToLower()) || p.CardNumber.ToLower().Contains(find.Find.ToLower())) && (p.StudyGroupId==find.GroupId || find.GroupId==0)).ToList() as List<T>);
             if(student is StudentGradesFind grad)
                return new ObservableCollection<T>(db.StudentGrades.Include(p => p.StudySubject).Include(p=>p.Student).Where(p => p.StudySessionId==grad.SessionId && p.Student.StudyGroupId==grad.GroupId).ToList() as List<T>);
            return null;

        }

        public bool Remove()
        {
            if (student is StudentGrades stud_grad)
            {
                db.Remove(stud_grad);
                db.SaveChanges();
                return true;
            }
            if (student is Student stud)
            {
                db.Remove(stud);
                db.SaveChanges();
                return true;
            }
            return false;
               

        }

        public bool Update()
        {
            if(student is Student)
            {
                db.Update(student);
                db.SaveChanges();
            }
            else
                return false;
            return true;
        }
    }
}
