using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using SARecorder.GeneralUI;
using System.IO;

namespace SARecorder
{
    public delegate void QuestionnaireFinishedEventHandler(object sender, EventArgs e);

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class QuestionnaireWindow : Window
    {
        public string SubjectName { get; set; } = "Unknown";
        public string SubjectTag { get; set; } = "Unknown";

        private int question7Index = 0;
        public int Question7Index
        {
            get { return question7Index; }
            set
            {
                try
                {
                    ImageBrush background = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/Resources/Q6_{value}.png")));
                    background.Stretch = Stretch.Uniform;

                    Question7.CanvasBackground = background;
                }
                catch (IOException)
                {
                    Debug.WriteLine("Warning: Background file not found");
                }
                finally
                {
                    question7Index = value;
                }
            }
        }

        public event QuestionnaireFinishedEventHandler QuestionnaireFinished;

        public QuestionnaireWindow()
        {
            InitializeComponent();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            int expectedIndex = QuestionSpace.SelectedIndex + 1;
            if (expectedIndex < QuestionSpace.Items.Count)
            {
                QuestionSpace.SelectedIndex = expectedIndex;
            }
            else
            {
                MessageBoxResult result = BigMessageBox.ShowWithAnswer("설문지를 종료할까요?", "SAGAT", MessageBoxButton.YesNo);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        try
                        {
                            Save();
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            MessageBox.Show("아마도 모든 문제를 다 풀지 않은 것 같습니다.");
                            break;
                        }
                        QuestionnaireFinished?.Invoke(this, new EventArgs());
                        Close();
                        break;
                    case MessageBoxResult.No:
                        break;
                    default:
                        NextButton.Content = "Unknown";
                        break;
                }
            }
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            int expectedIndex = QuestionSpace.SelectedIndex - 1;
            if (expectedIndex >= 0)
            {
                QuestionSpace.SelectedIndex = expectedIndex;
            }
        }

        private void Save()
        {
            Question1.SaveAsFile(SubjectName, SubjectTag);
            Question2.SaveAsFile(SubjectName, SubjectTag);
            Question3.SaveAsFile(SubjectName, SubjectTag);
            Question4.SaveAsFile(SubjectName, SubjectTag);
            Question5.SaveAsFile(SubjectName, SubjectTag);
            Question6.SaveAsFile(SubjectName, SubjectTag);
            Question7.SaveAsFile(SubjectName, SubjectTag);
        }
    }
}
