using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentList.Model
{
    public abstract class MainObject:ReactiveObject
    {
        [Reactive]
        public int Id { get; set; }
        [Reactive]
        public string Name { get; set; }
    }
}
