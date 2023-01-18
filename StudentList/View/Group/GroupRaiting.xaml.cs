using StudentList.Model.Student;
using StudentList.Model.Subject;
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
    /// Логика взаимодействия для GroupRaiting.xaml
    /// </summary>
    public partial class GroupRaiting : UserControl
    {
        private ObservableCollection<Model.Group.StudyGroup> StudyGroup { get; set; }
        public GroupRaiting(ObservableCollection<Model.Group.StudyGroup> StudyGroup)
        {
            this.StudyGroup= StudyGroup;
            InitializeComponent();
            Loaded+=GroupRaiting_Loaded;
        }

        private void GroupRaiting_Loaded(object sender, RoutedEventArgs e)
        {
            chart.Series.Clear();
            chart.BorderlineDashStyle = ChartDashStyle.Solid;
            chart.BorderlineColor = System.Drawing.Color.Gray;
            chart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;
            chart.ChartAreas.Add(new ChartArea());
            // Форматировать область диаграммы
            chart.ChartAreas[0].BackColor =System.Drawing.Color.Wheat;

            // Добавить и форматировать заголовок
            chart.Titles.Add($"Гистограмма рейтингов учебных групп");
            chart.Titles[0].Font = new Font("Utopia", 16);

            var ser = new Series("");
            
                ser.ChartType = SeriesChartType.Column;
            foreach (var x in StudyGroup.OrderBy(p=>p.MiddleGrades))
            {

                ser.Points.AddXY(x.Name, x.MiddleGrades);
                ser.Points.LastOrDefault().Font= new Font("Utopia", 13);
                Random random = new Random();
                ser.Points.LastOrDefault().Color= System.Drawing.Color.FromArgb((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255));

            }
            chart.Series.Add(ser);
            chart.ChartAreas[0].Area3DStyle.Enable3D = false;
        }
    }
    }

