using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;
using SARecorder;
using SARecorder.Entertainment;
using SARecorder.Automation;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using SARecorder.GeneralUI;

namespace SARecorder
{
    /// <summary>
    /// ExpController.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ExpController : Window
    {
        private ScanerFilter Scaner;
        private int prevScenario = -1;
        private bool prevSAGATState = false;
        private bool prevAutonomousState = false;
        private DispatcherTimer WaitTimer = new DispatcherTimer();

        private QuestionnaireWindow currentQuestionnaire;
        private VideoWindow mediaWindow = new VideoWindow();
        private Process webYouTube = null;

        public string CurrentSubjectTag
        {
            get { return SubjectLabel.Text; }
            set { SubjectLabel.Text = value; }
        }

        public ExpController()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Scaner = new ScanerFilter(Dispatcher);
            Scaner.Connect();
            Scaner.OnScanerDataReceived += Scaner_OnScanerDataReceived;

            WaitTimer.Interval = TimeSpan.FromSeconds(2);
            WaitTimer.Tick += WaitTimer_Tick;

            mediaWindow.Show();
            Debug.WriteLine("Keys");
            foreach (var key in Resources.Keys)
            {
                Debug.WriteLine(key);
            }
        }

        /// <summary>
        /// SCANeR에서 SAGAT을 수행할 시점에 설문을 시작 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Scaner_OnScanerDataReceived(object sender, ScanerEventArgs e)
        {
            if (e.CurrentStep != prevScenario)
            {
                Debug.WriteLine($"Switching from {prevScenario} to {e.CurrentStep}...");
                if (e.CurrentStep > 0 && e.CurrentStep < TargetScenarioList.Items.Count - 1)
                {
                    TargetScenarioList.SelectedIndex = e.CurrentStep;
                }
            }

            if (e.SAGATState != prevSAGATState && e.SAGATState == true)
            {
                WaitTimer.Start();
            }
            prevSAGATState = e.SAGATState;

            if (e.AutonomousState != prevAutonomousState)
            {
                if (e.AutonomousState)
                {
                    mediaWindow.PlayWithActive();
                }
                else
                {
                    mediaWindow.StopWithHide();
                }
            }
            prevAutonomousState = e.AutonomousState;
            prevScenario = e.CurrentStep;

            //추후 이벤트로 수정

            if (Scaner.IsConnected)
            {
                ScanerStateLabel.Content = "연결됨";
            }
            else
            {
                ScanerStateLabel.Content = "Error";
            }
        }

        private void TextBox_PreviewTextInput_OnlyNumber(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SubjectInfo_Changed(object sender, EventArgs e)
        {
            string gender = GenderComboBox.SelectedIndex == 0 ? "M" : "F";
            CurrentSubjectTag = $"{NameTextBox.Text}_{AgeTextBox.Text}_{gender}";
        }

        private void StartQuestionnaire()
        {
            currentQuestionnaire = new QuestionnaireWindow();
            currentQuestionnaire.SubjectName = CurrentSubjectTag;
            currentQuestionnaire.SubjectTag = $"Scenario{TargetScenarioList.SelectedIndex:00}";
            currentQuestionnaire.QuestionnaireFinished += CurrentQuestionnaire_QuestionnaireFinished;

            currentQuestionnaire.Left = -1080;
            currentQuestionnaire.Top = 0;
            currentQuestionnaire.Width = 720;
            currentQuestionnaire.Height = 1280;
            // 깔끔한 방법이 없을까?

            currentQuestionnaire.Question7Index = TargetScenarioList.SelectedIndex;

            currentQuestionnaire.Show();
        }

        private void WaitTimer_Tick(object sender, EventArgs e)
        {
            StartQuestionnaire();
            WaitTimer.Stop();
        }

        /// <summary>
        /// 설문 창이 닫히기 전 마지막 실행, 여기서는 동영상 창 띄움
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentQuestionnaire_QuestionnaireFinished(object sender, EventArgs e)
        {
            
        }

        private void OpenYouTubeWeb(string url)
        {
            webYouTube = new Process();
            webYouTube.StartInfo.FileName = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
            webYouTube.StartInfo.Arguments = url + " --new-window";

            webYouTube.Start();
            Debug.WriteLine(webYouTube.Handle);
        }

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        private void Window_Closed(object sender, EventArgs e)
        {
            Scaner.Dispose();
            //currentQuestionnaire.Close();
            mediaWindow.Close();
        }

        private void TargetScenarioList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mediaWindow.CurrentVideoID = TargetScenarioList.SelectedIndex;
        }

        private void QuitMenu_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void NewQuestionnaireMenu_Click(object sender, RoutedEventArgs e)
        {
            StartQuestionnaire();
        }

        private void VideoMenu_Click(object sender, RoutedEventArgs e)
        {
            if (mediaWindow.IsLoaded == false)
            {
                mediaWindow = new VideoWindow();
            }
            mediaWindow.Show();
        }

        private void VideoCloseButton_Click(object sender, RoutedEventArgs e)
        {
            mediaWindow.Close();
        }

        private void VideoControlOptionButton_Click(object sender, RoutedEventArgs e)
        {
            if (mediaWindow.VideoControlPanel.Visibility == Visibility.Hidden)
            {
                mediaWindow.VideoControlPanel.Visibility = Visibility.Visible;
            }
            else
            {
                mediaWindow.VideoControlPanel.Visibility = Visibility.Hidden;
            }
        }

        private void OpenFolderMenu_Click(object sender, RoutedEventArgs e)
        {
            string filePath = Environment.CurrentDirectory + @$"\Answers\";
            _ = Process.Start("explorer", filePath);
        }
    }
}
