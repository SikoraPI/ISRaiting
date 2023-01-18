using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StudentList.Model;
using StudentList.Model.Group;
using StudentList.Model.Student;
using StudentList.Model.Subject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace StudentList.ViewModel.Student
{
    public delegate void UpdateControl(UserControl control);
    public class StudentPageViewModel:GeneralViewModel
    {

        private UpdateControl UpdateMainControl;
        public Model.Student.Student Student { get; set; }
        [Reactive]
        public ObservableCollection<Model.Group.StudyGroup> GroupList { get; set; }
        [Reactive]
        public ObservableCollection<Model.Subject.StudySession>  StudySessions { get; set; }
        [Reactive]
        public ObservableCollection<Model.Subject.StudySubject> StudySubject { get; set; }
        [Reactive]
        public StudySession SessionId { get; set; }
        [Reactive]
        public  Model.Student.StudentGrades StudentGrades { get; set; }
        [Reactive]
        public ObservableCollection<Model.Student.StudentGrades> StudentGradesList { get; set; }
        public StudentPageViewModel(Model.Student.Student Student, UpdateControl UpdateMainControl) { 
        
            this.UpdateMainControl= UpdateMainControl;
            this.Student= Student;
            ICommand command = new StudyGroupCommand<StudyGroup>();
            GroupList=command.GetItemList<StudyGroup>();
            UserControls=new List<Tuple<int, string, UserControl>> { new Tuple<int, string, UserControl>(0, "StudentPage", new View.Student.StudentPage()) };
            var us = UserControls.Where(p => p.Item2=="StudentPage").FirstOrDefault().Item3;
            us.DataContext=this;
            UpdateMainControl(us);
            
            command = new SubjectCommand<StudySession>();
            StudySessions=command.GetItemList<StudySession>();
            command = new SubjectCommand<StudySubject>();
            StudySubject=command.GetItemList<StudySubject>();
            StudentGrades=new Model.Student.StudentGrades(Student.Id);

        }
        public ReactiveCommand<Unit, Unit> UpdateStudent => ReactiveCommand.Create(() => {
            ICommand command = new Model.Student.StudentCommand<Model.Student.Student>(Student);
            command.Update();
        });
        public ReactiveCommand<Unit, Unit> SelectSession => ReactiveCommand.Create(() => {

            ICommand command = new Model.Student.StudentCommand<StudentGrades>(new StudentGrades { StudentId=Student.Id, StudySessionId=SessionId.Id });
            StudentGradesList=new ObservableCollection<StudentGrades>( Student.StudentGrades.Where(p => p.StudySession.Name==SessionId.Name && p.StudySession.WinterSummer==SessionId.WinterSummer));
        });
        public ReactiveCommand<Unit, Unit> StudentGradAdd => ReactiveCommand.Create(() => {

            this.StudentGrades.StudySessionId= SessionId.Id;
            ICommand command = new Model.Student.StudentCommand<StudentGrades>(StudentGrades);
            command.Add();
            StudentGradesList=command.GetItemList<StudentGrades>();
            StudentGrades=new Model.Student.StudentGrades(Student.Id);
        });
        public ReactiveCommand<StudentGrades, Unit> DeleteGrad => ReactiveCommand.Create<StudentGrades>(DeleteGradCommand);
        private void DeleteGradCommand(StudentGrades grad)
        {
            if (grad==null) return;
            ICommand command = new Model.Student.StudentCommand<StudentGrades>(grad);
            command.Remove();
            StudentGradesList=command.GetItemList<StudentGrades>();
        }
        public ReactiveCommand<Unit, Unit> OpenGist => ReactiveCommand.Create(() => {

            var us=new View.Student.StudentGraph(Student, SessionId, StudentGradesList, false);
            us.DataContext=this;
            UpdateMainControl(us);
        });
        public ReactiveCommand<Unit, Unit> OpenDiag => ReactiveCommand.Create(() => {

            var us = new View.Student.StudentGraph(Student, SessionId, StudentGradesList, true);
            us.DataContext=this;
            UpdateMainControl(us);
        });
        public ReactiveCommand<Unit, Unit> Back => ReactiveCommand.Create(() => {

            var us = UserControls.Where(p => p.Item2=="StudentPage").FirstOrDefault().Item3;
            us.DataContext=this;
            UpdateMainControl(us);
        });
    }
}
