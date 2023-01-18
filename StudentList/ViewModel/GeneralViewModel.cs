using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace StudentList.ViewModel
{
    public delegate void UpdateView();
    public abstract class GeneralViewModel:ReactiveObject
    {
        protected Dictionary<string, UpdateView> UpdateViews;
        [Reactive]
        public UserControl MainControl { get; set; }
        protected List<Tuple<int,string,UserControl>> UserControls { get; set; }
        [Reactive]
        public int CurrentPage { get; set; }
        public ReactiveCommand<string, Unit> OpenPage => ReactiveCommand.Create<string>(OpenPageCommand);
        protected virtual void OpenPageCommand(string i)
        {
            var user_contr = UserControls.Where(p => p.Item2==i).FirstOrDefault();
            if (user_contr == null) 
                return;
            CurrentPage=user_contr.Item1 ;
            UpdateControl(user_contr.Item3);
            var s = user_contr.Item3.DataContext as GeneralViewModel;
            s.OpenPageCommand(i);
        }
        protected virtual void UpdateData() { }
        public void UpdateControl(UserControl control) => MainControl = control;
    }
}
