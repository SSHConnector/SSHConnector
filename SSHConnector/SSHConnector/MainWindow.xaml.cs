using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SSHConnector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo driveInfo in drives)
            {
                if (driveInfo.RootDirectory.Exists)
                {
                    TreeViewItem subitem = CreateTreeItem(driveInfo,"drive.png");
                    trvStructure.Items.Add(subitem);
                    foreach (FileInfo file in driveInfo.RootDirectory.GetFiles())
                    {
                        subitem.Items.Add(file);
                    }
                }
            }
        }

        public void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.Source as TreeViewItem;
            {
                item.Items.Clear();

                DirectoryInfo expandedDir = null;
                if (item.Tag is DriveInfo)
                    expandedDir = (item.Tag as DriveInfo).RootDirectory;
                if (item.Tag is DirectoryInfo)
                    expandedDir = (item.Tag as DirectoryInfo);

                if (item.Tag is FileInfo) return;

                    // load directories
                    foreach (DirectoryInfo subDir in expandedDir.GetDirectories())
                {
                    TreeViewItem subitem = CreateTreeItem(subDir, "folder.ico");
                    item.Items.Add(subitem);
                }

                // load files
                try
                {
                    foreach (FileInfo file in expandedDir.GetFiles())
                    {
                        TreeViewItem subitem = CreateTreeItem(file, "file.png");
                        subitem.IsExpanded = true;
                        item.Items.Add(subitem);
                    }
                }
                catch { }
            }
        }

        private TreeViewItem CreateTreeItem(object o, string iconName)
        {
            TreeViewItem item = new TreeViewItem();
            item.Header = GetHeader(o, iconName);
            item.Tag = o;
            if(iconName != "file.png")
                item.Items.Add("Loading...");
            return item;
        }

        private TextBlock GetHeader(object o, string iconName)
        {
            string itemString = o.ToString();
            Image tempImage = new Image();
            Uri uri = new Uri(@"pack://application:,,,/Resources/" + iconName);
            BitmapImage source = new BitmapImage();
            source.BeginInit();
            source.UriSource = uri;
            source.DecodePixelHeight = 20;
            source.DecodePixelWidth = 30;
            source.EndInit();
            tempImage.Source = source;

            TextBlock tempTextBlock = new TextBlock();
            tempTextBlock.Inlines.Add(tempImage);
            tempTextBlock.Inlines.Add(itemString);
            ContextMenu menu = new ContextMenu();

            MenuItem mitem1 = new MenuItem();
            TextBlock v1 = new TextBlock();
            v1.Inlines.Add("View");
            mitem1.Header = v1;
            mitem1.Tag = o;
            mitem1.Click += MenuItem_View;

            MenuItem mitem2 = new MenuItem();
            TextBlock v2 = new TextBlock();
            v2.Inlines.Add("View2");
            mitem2.Header = v2;
            mitem2.Tag = o;
            mitem2.Click += MenuItem_View;

            menu.Items.Add(mitem1);
            menu.Items.Add(mitem2);
            tempTextBlock.ContextMenu = menu;

            return tempTextBlock;
        }

        private void MenuItem_View(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            TextBlock tb = item.Header as TextBlock;
            MessageBox.Show(tb.Text);

            if (item.Tag is DriveInfo)
            {
                DriveInfo currentItem = item.Tag as DriveInfo;
                MessageBox.Show(currentItem.Name);
            }
            if (item.Tag is DirectoryInfo)
            {
                DirectoryInfo currentItem = item.Tag as DirectoryInfo;
                MessageBox.Show(currentItem.Name);
            }
            if (item.Tag is FileInfo)
            {
                FileInfo currentItem = item.Tag as FileInfo;
                MessageBox.Show(currentItem.Name);
            }
        }

    }
}
