using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StudentList.View.Group
{
    /// <summary>
    /// Логика взаимодействия для GroupGist.xaml
    /// </summary>
    public partial class GroupGist : UserControl
    {
        private Model.Subject.StudySession StudySession { get; set; }
        private Model.Group.StudyGroup StudyGroup { get; set; }
        private ObservableCollection<Model.Student.StudentGrades> StudentGrades { get; set; }
        private bool IsGrad { get; set; }
        public GroupGist(Model.Group.StudyGroup StudyGroup, Model.Subject.StudySession StudySession, ObservableCollection<Model.Student.StudentGrades> StudentGrades, bool isGrad)
        {
            this.StudyGroup= StudyGroup;
            this.StudentGrades = StudentGrades;
            this.StudySession= StudySession;
            InitializeComponent();
            Loaded+=GroupGist_Loaded;
            IsGrad=isGrad;
        }

        private void GroupGist_Loaded(object sender, RoutedEventArgs e)
        {
            chart.Series.Clear();
            chart.BorderlineDashStyle = ChartDashStyle.Solid;
            chart.BorderlineColor = System.Drawing.Color.Gray;
            chart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;
            chart.ChartAreas.Add(new ChartArea());
            // Форматировать область диаграммы
            chart.ChartAreas[0].BackColor =System.Drawing.Color.Wheat;

            // Добавить и форматировать заголовок
            chart.Titles.Add($"Диаграмма успеваемости группы {StudyGroup.Name}  за {StudySession.AccusativeSession}");
            chart.Titles[0].Font = new Font("Utopia", 16);
            var grades = StudentGrades
                    .GroupBy(p => p.Grades)
                    .Select(g => new
                    {
                        Name = g.Key,
                        Count = g.Count(),
                    });
            var ser = new Series("");
            if (IsGrad)
            {
                ser.ChartType= SeriesChartType.Pie;
            }
            else
                ser.ChartType = SeriesChartType.Column;
            foreach (var x in grades.OrderBy(p => p.Name))
            {

                ser.Points.AddXY(Model.Student.StudentGrades.GetGradString(x.Name), x.Count);
                switch (x.Name)
                {
                    case 2:
                        ser.Points.LastOrDefault().Color = System.Drawing.Color.DarkRed;
                        break;
                    case 3:
                        ser.Points.LastOrDefault().Color= System.Drawing.Color.Orange;
                        break;
                    case 4:
                        ser.Points.LastOrDefault().Color = System.Drawing.Color.Yellow;
                        break;
                    case 5:
                        ser.Points.LastOrDefault().Color = System.Drawing.Color.Green;
                        break;

                };

                ser.Points.LastOrDefault().Font= new Font("Utopia", 13);

            }
            chart.Series.Add(ser);
            chart.ChartAreas[0].Area3DStyle.Enable3D = false;
        }
    }
}
