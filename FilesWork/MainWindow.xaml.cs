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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FilesWork
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            GetDrives(comboBox1);
            GetDrives(comboBox2);
        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = e.Source as ComboBox;

            var expandedDir = (box.SelectedValue as DriveInfo).RootDirectory;

            FillListView(expandedDir, listView1);
        }

        private void comboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = e.Source as ComboBox;

            var expandedDir = (box.SelectedValue as DriveInfo).RootDirectory;

            FillListView(expandedDir, listView2);
        }

        private async void copy_Click(object sender, RoutedEventArgs e)
        {
            var itemToCopy = listView1.SelectedItem;

            if (itemToCopy is FileInfo)
            {
                var item = itemToCopy as FileInfo;

                if (listView2.SelectedValue != null)
                {
                    var destinationPath = (listView2.SelectedValue as DirectoryInfo).FullName;
                    var fullDestinationPath = destinationPath + "\\copy_" + item.Name;

                    using (FileStream SourceStream = File.Open(item.FullName, FileMode.Open))
                    {
                        using (FileStream DestinationStream = File.Create(fullDestinationPath))
                        {
                            var copyWindow = new CopyWindow();
                            copyWindow.FilePath = fullDestinationPath;
                            copyWindow.Show();
                                            
                            await SourceStream.CopyToAsync(DestinationStream);
                            copyWindow.Close();
                        }
                    }
                } else
                {
                    var destinationPath = "D:";
                    var fullDestinationPath = destinationPath + "\\copy_" + item.Name;

                    using (FileStream SourceStream = File.Open(item.FullName, FileMode.Open))
                    {
                        using (FileStream DestinationStream = File.Create(fullDestinationPath))
                        {
                            var copyWindow = new CopyWindow();
                            copyWindow.FilePath = fullDestinationPath;
                            copyWindow.Show();
                            await SourceStream.CopyToAsync(DestinationStream);
                            copyWindow.Close();
                        }
                    }
                }
            }
        }

        private void GetDrives(ComboBox box)
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (var drive in drives)
            {
                box.Items.Add(drive);
            }
        }

        private void FillListView(DirectoryInfo info, ListView list)
        {
            list.Items.Clear();

            try
            {
                foreach (DirectoryInfo subDir in info.GetDirectories())
                {
                    list.Items.Add(subDir);
                }

                var files = info.GetFiles();

                foreach (FileInfo file in files)
                {
                    list.Items.Add(file);
                }
            }
            catch { }
        }

        private void listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListView list = e.Source as ListView;

            DirectoryInfo expandedDir = null;

            if (list.SelectedValue is DriveInfo)
                expandedDir = (list.SelectedValue as DriveInfo).RootDirectory;
            if (list.SelectedValue is DirectoryInfo)
                expandedDir = (list.SelectedValue as DirectoryInfo);

            if (expandedDir == null)
                return;

            list.Items.Clear();

            try
            {
                if (expandedDir.Parent != null)
                    list.Items.Add(expandedDir.Parent);

                foreach (DirectoryInfo subDir in expandedDir.GetDirectories())
                {
                    list.Items.Add(subDir);
                }

                var files = expandedDir.GetFiles();

                foreach (FileInfo file in files)
                {
                    list.Items.Add(file);
                }
            }
            catch { }
        }
    }
}
