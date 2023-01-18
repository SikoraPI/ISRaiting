using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentList.Model
{
    public interface ICommand
    {
        bool Update();
        bool Add();
        ObservableCollection<T> GetItemList<T>();
        T GetItem<T>();
        bool Remove();
    }
}
