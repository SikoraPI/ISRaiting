using Microsoft.Win32;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StudentList.Model;
using StudentList.Model.Group;
using StudentList.Model.Student;
using StudentList.Model.Subject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Xml.Serialization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace StudentList.ViewModel.Student
{
    public class StudentViewModel : GeneralViewModel
    {
        [Reactive]
        public Model.Student.Student NewStudent { get; set; }
        [Reactive]
        public ObservableCollection<Model.Group.StudyGroup> GroupList { get; set; }
        [Reactive]
        public Model.Student.StudentFind StudentFind { get; set; }
        [Reactive]
        public ObservableCollection<Model.Student.Student> Students { get; set; }
        public StudentViewModel()
        {
            UpdateViews= new Dictionary<string, UpdateView>()
            {
                { "StudentTable", OpenTableStudent},
                { "AddStudent", OpenAddStudent}
            };
            UserControls=new List<Tuple<int, string, UserControl>> { new Tuple<int, string, UserControl>(0, "AddStudent", new View.Student.AddStudent()), new Tuple<int, string, UserControl>(0, "StudentTable", new View.Student.StudentTable()) };

            StudentFind=new Model.Student.StudentFind();
        }
        protected override void OpenPageCommand(string i)
        {
            var user_contr = UserControls.Where(p => p.Item2==i).FirstOrDefault();
            if (user_contr == null)
                return;
            CurrentPage=user_contr.Item1;
            UpdateControl(user_contr.Item3);
            GetAllData();
            if (UpdateViews.Any(p => p.Key==i))
                UpdateViews[i]();
        }
        private void GetAllData()
        {
            ICommand command = new StudyGroupCommand<StudyGroup>();
            GroupList=command.GetItemList<StudyGroup>();
        }
        public ReactiveCommand<Model.Student.Student, Unit> DeleteStudent => ReactiveCommand.Create<Model.Student.Student>(DeleteStudentCommand);
        private void DeleteStudentCommand(Model.Student.Student Student)
        {
            if (Student==null) return;
            System.Windows.Forms.DialogResult dialogResult = System.Windows.Forms.MessageBox.Show($"Вы действительно хотите удалить студента {Student.FIO}", "Внимание", System.Windows.Forms.MessageBoxButtons.YesNo);
            if (dialogResult == System.Windows.Forms.DialogResult.No)
                return;
            ICommand command = new StudentCommand<Model.Student.Student>(Student);
            command.Remove();
            FindStudentCommand();
        }
        private void OpenTableStudent()
        {
            GroupList.Insert(0, new StudyGroup { Id=0, Name="Все группы" });
            StudentFind=new Model.Student.StudentFind { Find=StudentFind.Find, GroupId=StudentFind.GroupId };
            FindStudentCommand();
        }
        private void OpenAddStudent()
        {
            NewStudent=new Model.Student.Student();
        }
        public ReactiveCommand<Unit, Unit> AddStudent => ReactiveCommand.Create(() => {
            ICommand command = new StudentCommand<Model.Student.Student>(NewStudent);
            command.Add();
            NewStudent=new Model.Student.Student();
        });
        public ReactiveCommand<Unit, Unit> Find => ReactiveCommand.Create(FindStudentCommand);
        private void FindStudentCommand()
        {
            ICommand command = new StudentCommand<StudentFind>(StudentFind);
            Students=command.GetItemList<Model.Student.Student>();
        }
        public ReactiveCommand<Unit, Unit> FindClear => ReactiveCommand.Create(() => {
            StudentFind=new Model.Student.StudentFind();
        });
        public ReactiveCommand<Model.Student.Student, Unit> OpenUser => ReactiveCommand.Create<Model.Student.Student>(OpenUserCommand);
        private void OpenUserCommand(Model.Student.Student user)
        {
            if (user==null) return;
            StudentPageViewModel studentPageViewModel = new StudentPageViewModel(user, UpdateControl);

        }
        public ReactiveCommand<Unit, Unit> SaveXML => ReactiveCommand.Create(() => {

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ObservableCollection<Model.Student.Student>));

            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "XML Files(*.xml)|*.xml"
            };

            if (dialog.ShowDialog() == true)
            {
                using (FileStream fs = new FileStream(dialog.FileName, FileMode.OpenOrCreate))
                {
                    xmlSerializer.Serialize(fs, Students);

                    MessageBox.Show("Файл сохранен", "Успех");
                }
            }


        });
        public ReactiveCommand<Unit, Unit> OpenXML => ReactiveCommand.Create(() => {

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ObservableCollection<Model.Student.Student>));

            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {

                using (FileStream fs = new FileStream(openFileDialog.FileName, FileMode.OpenOrCreate))
                {
                    Students = xmlSerializer.Deserialize(fs) as ObservableCollection<Model.Student.Student>;

                    MessageBox.Show("Файл загружен", "Успех");
                }
            }
            foreach (var stud in Students)
            {
                using var db = new ConnectDB();

                if (!db.Groups.Any(p => p.Name==stud.StudyGroup.Name))
                {
                    var new_group = new StudyGroup { Name= stud.StudyGroup.Name };
                    db.Groups.Add(new_group);
                    db.SaveChanges();
                    stud.StudyGroup.Id=new_group.Id;
                }
                else
                {
                    stud.StudyGroup.Id=db.Groups.Where(p => p.Name==stud.StudyGroup.Name).FirstOrDefault().Id;
                }
                if (!db.Students.Any(p => p.CardNumber==stud.CardNumber))
                {
                    var new_stud = new Model.Student.Student { CardNumber=stud.CardNumber, FirstName=stud.FirstName, LastName=stud.LastName, DateBirth=stud.DateBirth, SecondName=stud.SecondName, StudyGroupId=stud.StudyGroup.Id };
                    db.Students.Add(new_stud);
                    db.SaveChanges();
                    stud.Id= new_stud.Id;
                }
                else
                {
                    stud.Id=db.Students.Where(p => p.CardNumber==stud.CardNumber).FirstOrDefault().Id;
                }
                foreach (var grad in stud.StudentGrades)
                {
                    if (!db.StudentSessions.Any(p => p.Name==grad.StudySession.Name && p.WinterSummer==grad.StudySession.WinterSummer))
                    {
                        var new_sess = new StudySession { Name=grad.StudySession.Name, WinterSummer =grad.StudySession.WinterSummer };
                        db.StudentSessions.Add(new_sess);
                        db.SaveChanges();
                        grad.StudySessionId=new_sess.Id;
                    }
                    else
                    {
                        grad.StudySessionId=db.StudentSessions.Where(p => p.Name==grad.StudySession.Name && p.WinterSummer==grad.StudySession.WinterSummer).FirstOrDefault().Id;
                    }
                    if (!db.StudySubjects.Any(p => p.Name==grad.StudySubject.Name))
                    {
                        var new_sub = new StudySubject { Name=grad.StudySubject.Name };
                        grad.StudySubject.Id=0;
                        db.StudySubjects.Add(new_sub);
                        db.SaveChanges();
                        grad.StudySubjectId=new_sub.Id;
                    }
                    else
                    {
                        grad.StudySubjectId=db.StudySubjects.Where(p => p.Name==grad.StudySubject.Name).FirstOrDefault().Id;
                    }

                    if (!db.StudentGrades.Any(p => p.StudentId==stud.Id && p.StudySubjectId==grad.StudySubjectId && p.StudySessionId==grad.StudySessionId))
                    {
                        grad.Id=0;
                        grad.StudySession=null;
                        grad.StudySubject=null;
                        grad.Student=null;
                        grad.StudentId=stud.Id;
                        db.StudentGrades.Add(grad);
                        db.SaveChanges();
                    }
                }


            }

            FindStudentCommand();


        });
    }
}
