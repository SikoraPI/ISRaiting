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
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace StudentList.ViewModel.Group
{
    public class GroupViewModel:GeneralViewModel
    {
        [Reactive]
        public Model.Group.StudyGroup NewStudyGroup { get; set; }
        [Reactive]
        public ObservableCollection<StudyGroup> StudyGroups { get; set; }
        [Reactive]
        public ObservableCollection<Model.Subject.StudySession> StudySessions { get; set; }
        [Reactive]
        public string GroupFind { get; set; }
        [Reactive]
        public Model.Subject.StudySession SessionId { get; set; }
        private bool IsGist { get; set; }
        [Reactive]
        public StudyGroup SelectedStudy { get; set; }
        [Reactive]
        public UserControl GroupGist { get; set; }
        public GroupViewModel() {
            UserControls=new List<Tuple<int, string, UserControl>> { new Tuple<int, string, UserControl>(0, "AddGroup", new View.Group.GroupAdd()), new Tuple<int, string, UserControl>(0, "GroupTable", new View.Group.GroupTable()) };
            NewStudyGroup =new Model.Group.StudyGroup();
            GroupFind="";
        }
        protected override void OpenPageCommand(string i)
        {
            var user_contr = UserControls.Where(p => p.Item2==i).FirstOrDefault();
            if (user_contr == null)
                return;
            CurrentPage=user_contr.Item1;
            UpdateControl(user_contr.Item3);
            if (i=="GroupTable") GetGroup();
        }
        private void GetGroup()
        {
            ICommand command = new StudyGroupCommand<string>(GroupFind.ToLower());
            StudyGroups =command.GetItemList<StudyGroup>();
        }
        public ReactiveCommand<Unit, Unit> AddGroup => ReactiveCommand.Create(() => {
            ICommand command = new StudyGroupCommand<StudyGroup>(NewStudyGroup);
            command.Add();
            NewStudyGroup =new Model.Group.StudyGroup();
        });
        public ReactiveCommand<Unit, Unit> FindGroup => ReactiveCommand.Create(GetGroup);
        public ReactiveCommand<StudyGroup, Unit> DeleteGroup => ReactiveCommand.Create<StudyGroup>(DeleteGroupCommand);
        private void DeleteGroupCommand(StudyGroup study)
        {
            if(study==null) return;
            System.Windows.Forms.DialogResult dialogResult = System.Windows.Forms.MessageBox.Show($"Вы действительно хотите удалить группу {study.Name}", "Внимание", System.Windows.Forms.MessageBoxButtons.YesNo);
            if (dialogResult == System.Windows.Forms.DialogResult.No)
                return;
            ICommand command = new StudyGroupCommand<StudyGroup>(study);
            command.Remove();
            GetGroup();
        }
        public ReactiveCommand<Unit, Unit> UpdateGroup => ReactiveCommand.Create(() => {
            ICommand command = new StudyGroupCommand<ObservableCollection<StudyGroup>>(StudyGroups);
            command.Update();
            GetGroup();
        });
        private void GetSession()
        {
            SessionId=null;
            ICommand command = new SubjectCommand<StudySession>();
            StudySessions=command.GetItemList<StudySession>();
            MainControl=new View.Group.GroupView();
            MainControl.DataContext=this;
            GroupGist=null;
        }
     
        public ReactiveCommand<StudyGroup, Unit> OpenGistPage => ReactiveCommand.Create<StudyGroup>(OpenGistPageCommand);
        public ReactiveCommand<StudyGroup, Unit> OpenDioPage => ReactiveCommand.Create<StudyGroup>(OpenDioPageCommand);

        private void OpenGistPageCommand(StudyGroup study)
        {
            if(study==null) return; 
            SelectedStudy=study;
            GetSession(); IsGist=true;
            

        }

        private void OpenDioPageCommand(StudyGroup study)
        {
            if (study==null) return;
            SelectedStudy=study;
            GetSession(); IsGist=false;
        }
        public ReactiveCommand<Unit, Unit> SelectSession => ReactiveCommand.Create(() => {
            if(SessionId==null) return;
            ICommand command = new Model.Student.StudentCommand<StudentGradesFind>(new StudentGradesFind(SelectedStudy.Id, SessionId.Id));
           var StudentGradesList=command.GetItemList<StudentGrades>();
            GroupGist=new View.Group.GroupGist(SelectedStudy, SessionId, StudentGradesList, IsGist );
        });
        public ReactiveCommand<Unit, Unit> Back => ReactiveCommand.Create(() => {
            OpenPageCommand("GroupTable");
        });
        public ReactiveCommand<Unit, Unit> OpenRaiting => ReactiveCommand.Create(() => {
            MainControl=new View.Group.GroupRaiting(StudyGroups);
            MainControl.DataContext=this;
        });
    }
}
