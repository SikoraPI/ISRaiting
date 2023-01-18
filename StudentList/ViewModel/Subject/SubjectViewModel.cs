using Microsoft.EntityFrameworkCore.Query.Internal;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StudentList.Model;
using StudentList.Model.Group;
using StudentList.Model.Subject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace StudentList.ViewModel.Subject
{
    public class SubjectViewModel:GeneralViewModel
    {
        [Reactive]
        public Model.Subject.StudySubject NewSubject { get; set; }
        public List<int> Years { get; set; }
        [Reactive]
        public ObservableCollection<StudySubject> StudySubject { get; set; }
        [Reactive]
        public string SubjectFind { get; set; }
        public SubjectViewModel()
        {
            UserControls=new List<Tuple<int, string, UserControl>> { new Tuple<int, string, UserControl>(0, "AddSubject", new View.Subject.SubjectAdd()), new Tuple<int, string, UserControl>(0, "TableSubject", new View.Subject.SubjectTable()) };
            NewSubject=new Model.Subject.StudySubject();
            SubjectFind="";
        }
        protected override void OpenPageCommand(string i)
        {
            var user_contr = UserControls.Where(p => p.Item2==i).FirstOrDefault();
            if (user_contr == null)
                return;
            CurrentPage=user_contr.Item1;
            UpdateControl(user_contr.Item3); 
            if (i=="TableSubject") GetSubject();
        }
        private void GetSubject()
        {
            ICommand command = new  SubjectCommand<string>(SubjectFind.ToLower());
            StudySubject =command.GetItemList<StudySubject>();
        }
        public ReactiveCommand<Unit, Unit> FindSubject => ReactiveCommand.Create(GetSubject);
        public ReactiveCommand<Unit, Unit> AddSubject => ReactiveCommand.Create(() => {
            ICommand command = new SubjectCommand<StudySubject>(NewSubject);
            command.Add();
            NewSubject=new Model.Subject.StudySubject();
        });
        public ReactiveCommand<StudySubject, Unit> DeleteSubject => ReactiveCommand.Create<StudySubject>(DeleteSubjectCommand);
        private void DeleteSubjectCommand(StudySubject study)
        {
            if (study==null) return;
            System.Windows.Forms.DialogResult dialogResult = System.Windows.Forms.MessageBox.Show($"Вы действительно хотите удалить предмет {study.Name}", "Внимание", System.Windows.Forms.MessageBoxButtons.YesNo);
            if (dialogResult == System.Windows.Forms.DialogResult.No)
                return;
            ICommand command = new SubjectCommand<StudySubject>(study);
            command.Remove();
            GetSubject();
        }
        public ReactiveCommand<Unit, Unit> UpdateSubject => ReactiveCommand.Create(() => {
            ICommand command = new SubjectCommand<ObservableCollection<StudySubject>>(StudySubject);
            command.Update();
            GetSubject();
        });
    }
}
