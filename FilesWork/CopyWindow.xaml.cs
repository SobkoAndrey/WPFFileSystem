using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FilesWork
{
    public partial class CopyWindow : Window
    {
        private CancellationTokenSource cts;
        private PauseTokenSource pts;

        public string FilePath { get; set; }

        public CopyWindow()
        {
            InitializeComponent();
        }

        public async void CopyFiles(FileInfo copyfile, ListView destinationListView)
        {
            cts = new CancellationTokenSource();
            pts = new PauseTokenSource();

            if (destinationListView.SelectedValue != null)
            {
                var destinationPath = (destinationListView.SelectedValue as DirectoryInfo).FullName;
                var fullDestinationPath = destinationPath + "\\copy_" + copyfile.Name;
                var dictionary = new Dictionary<string, string> { { copyfile.FullName, fullDestinationPath } };
                progressBar.Maximum = 10000.0;
                
                try
                {
                    await Copier.CopyFiles(dictionary, cts.Token, pts.Token, prog => progressBar.Value = prog);
                }
                catch (OperationCanceledException exception)
                {
                    File.Delete(fullDestinationPath);
                }

                Close();
            }
            else
            {
                var destinationPath = "D:";
                var fullDestinationPath = destinationPath + "\\copy_" + copyfile.Name;

                var dictionary = new Dictionary<string, string>
                        {
                            {copyfile.FullName, fullDestinationPath}
                        };

                progressBar.Maximum = 10000.0;

                try
                {
                    await Copier.CopyFiles(dictionary, cts.Token, pts.Token, prog => progressBar.Value = prog);
                }
                catch (OperationCanceledException exception)
                {
                    File.Delete(fullDestinationPath);
                }

                Close();
            }
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            if (cts != null)
            {
                cts.Cancel();
            }
        }

        private void pause_Click(object sender, RoutedEventArgs e)
        {
            pts.IsPaused = !pts.IsPaused;
        }
    }
}