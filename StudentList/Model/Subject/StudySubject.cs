using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StudentList.Model.Student;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace StudentList.Model.Subject
{
    public class StudySubject : MainObject
    {
    }

    public class StudySession
    {
        [Reactive]
        public int Id { get; set; }
        [Reactive]
        new public int Name { get; set; }
        [Reactive]
        //True - summer;False - winter
        public bool WinterSummer { get; set; }
        [NotMapped]
        public string Session { get; set; }
        [NotMapped]
        public string AccusativeSession
        {
            get
            {
                string ses = "";
                if (WinterSummer)
                    ses+="летнюю";
                else
                    ses+="зимнюю";
                ses+=$" сессию  {Name} года";
                return ses;
            }
        }
        public StudySession() { Name=DateTime.Now.Year; }
        public StudySession(int Id, int name, bool winterSummer)
        {
            this.Id=Id;
            this.Name = name;
            this.WinterSummer= winterSummer;
            Session="";
            if (winterSummer)
                Session+="Летняя";
            else
                Session+="Зимняя";
            Session+=$" сессия {name} года";

        }
    }

    public class SubjectCommand<Y> : ICommand
    {
        private Y subject { get; set; }
        private ConnectDB db = new ConnectDB();
        public SubjectCommand(Y subject)
        {

            this.subject = subject;
        }
        public SubjectCommand()
        {


        }
        public bool Add()
        {
            switch (subject)
            {
                case StudySession studySession:
                    if (db.StudentSessions.Any(p => p.Name==studySession.Name && p.WinterSummer==studySession.WinterSummer))
                    {
                        MessageBox.Show("Такая сессия уже существует", "Ошибка");
                        return false;
                    }
                    db.StudentSessions.Add(studySession);
                    break;
                case StudySubject subject:
                    if (db.StudySubjects.Any(p => p.Name==subject.Name))
                    {
                        MessageBox.Show("Такой предмет уже существует", "Ошибка");
                        return false;
                    }
                    db.StudySubjects.Add(subject);
                    break;
                default: return false;
            }
            db.SaveChanges();
            return true;
        }

        public T GetItem<T>()
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<T> GetItemList<T>()
        {

            if (subject==null  && typeof(StudySession).IsAssignableFrom(typeof(T)))
                return new ObservableCollection<T>(db.StudentSessions.OrderBy(p=>p.Name).Select(p => new StudySession(p.Id,p.Name, p.WinterSummer)) as IEnumerable<T>);
            if (subject==null  && typeof(StudySubject).IsAssignableFrom(typeof(T)))
                return new ObservableCollection<T>(db.StudySubjects as IEnumerable<T>);
              if (subject is string find_sub  && typeof(StudySubject).IsAssignableFrom(typeof(T)))
                return new ObservableCollection<T>(db.StudySubjects.Where(p=>p.Name.ToLower().Contains(find_sub)) as IEnumerable<T>);
            return null;
        }

        public bool Remove()
        {
            if(subject is StudySubject sub)
            {
                db.Remove(subject);
                db.SaveChanges();
                return true;
            }
            return false;
        }

        public bool Update()
        {
            if(subject is ObservableCollection<StudySubject> sub)
            {
                db.StudySubjects.UpdateRange(sub);
                db.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
