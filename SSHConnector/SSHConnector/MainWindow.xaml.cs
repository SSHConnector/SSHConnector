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
        public static string host = string.Empty;
        public static string user = string.Empty;
        public static string password = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
        }
        public void LoadTree()
        {
            FSInfo fsinfo = new FSInfo();
            fsinfo.FullPath = @"/";
            fsinfo.Name = @"/";
            fsinfo.FSType = "FOLDER";
            TreeViewItem subitem = CreateTreeItem(fsinfo, "drive.png");
            trvStructure.Items.Add(subitem);
        }

        public void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.Source as TreeViewItem;
            {
                item.Items.Clear();
                FSInfo fsinfo = item.Tag as FSInfo;

                // load directories
                foreach (FSInfo subFSInfo in GetSubFSInfos(fsinfo))
                {
                    if (subFSInfo.FSType == "FOLDER")
                    {
                        TreeViewItem subitem = CreateTreeItem(subFSInfo, "folder.ico");
                        if (subFSInfo.Name == "." || subFSInfo.Name == "..")
                            subitem.IsExpanded = true;
                        item.Items.Add(subitem);
                    }

                    if (subFSInfo.FSType == "FILE")
                    {
                        TreeViewItem subitem = CreateTreeItem(subFSInfo, "file.png");
                        subitem.IsExpanded = true;
                        item.Items.Add(subitem);
                    }
                }
            }
        }

        private IEnumerable<FSInfo> GetSubFSInfos(FSInfo fsinfo)
        {
            List<FSInfo> list = new List<FSInfo>();
            if (fsinfo.Name != "." && fsinfo.Name != "..")
            {
                InvokeSSH ssh = new InvokeSSH(host, user, password);
                string output = ssh.Run(string.Format("cd {0};ls -al", fsinfo.FullPath));
                string[] lines = output.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.None);
                foreach (string line in lines)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        if (line.StartsWith("d"))
                        {
                            FSInfo folder = new FSInfo();
                            string[] its = line.Split(' ', '\t');
                            folder.Name = its[its.Length - 1];
                            folder.FullPath = fsinfo.FullPath + "/" + folder.Name;
                            folder.FSType = "FOLDER";
                            //if (folder.Name != "." && folder.Name != "..")
                            {
                                list.Add(folder);
                            }
                        }

                        if (line.StartsWith("-"))
                        {
                            FSInfo file = new FSInfo();
                            string[] its = line.Split(' ', '\t');
                            file.Name = its[its.Length - 1];
                            file.FullPath = fsinfo.FullPath + "/" + file.Name;
                            file.FSType = "FILE";
                            list.Add(file);
                        }
                    }
                }
            }

            return list;
        }

        private TreeViewItem CreateTreeItem(object o, string iconName)
        {
            TreeViewItem item = new TreeViewItem();
            FSInfo fsinfo = o as FSInfo;
            item.Header = GetHeader(o, iconName);
            item.Tag = o;
            if (iconName != "file.png" && fsinfo.Name != "." && fsinfo.Name != "..")
                item.Items.Add("Loading...");
            return item;
        }

        private TextBlock GetHeader(object o, string iconName)
        {
            FSInfo fsinfo = o as FSInfo;
            string itemString = fsinfo.Name.ToString();
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
            v2.Inlines.Add("Upload");
            mitem2.Header = v2;
            mitem2.Tag = o;
            mitem2.Click += MenuItem_View;

            MenuItem mitem3 = new MenuItem();
            TextBlock v3 = new TextBlock();
            v3.Inlines.Add("Download");
            mitem3.Header = v3;
            mitem3.Tag = o;
            mitem3.Click += MenuItem_View;

            MenuItem mitem4 = new MenuItem();
            TextBlock v4 = new TextBlock();
            v4.Inlines.Add("Run Terminal");
            mitem4.Header = v4;
            mitem4.Tag = o;
            mitem4.Click += MenuItem_View;

            MenuItem mitem5 = new MenuItem();
            TextBlock v5 = new TextBlock();
            v5.Inlines.Add("Run Command");
            mitem5.Header = v5;
            mitem5.Tag = o;
            mitem5.Click += MenuItem_View;

            MenuItem mitem6 = new MenuItem();
            TextBlock v6 = new TextBlock();
            v6.Inlines.Add("Move");
            mitem6.Header = v6;
            mitem6.Tag = o;
            mitem6.Click += MenuItem_View;

            MenuItem mitem7 = new MenuItem();
            TextBlock v7 = new TextBlock();
            v7.Inlines.Add("Delete");
            mitem7.Header = v7;
            mitem7.Tag = o;
            mitem7.Click += MenuItem_View;

            MenuItem mitem8 = new MenuItem();
            TextBlock v8 = new TextBlock();
            v8.Inlines.Add("Copy");
            mitem8.Header = v8;
            mitem8.Tag = o;
            mitem8.Click += MenuItem_View;

            MenuItem mitem9 = new MenuItem();
            TextBlock v9= new TextBlock();
            v9.Inlines.Add("Paste");
            mitem9.Header = v9;
            mitem9.Tag = o;
            mitem9.Click += MenuItem_View;

            MenuItem mitem10 = new MenuItem();
            TextBlock v10 = new TextBlock();
            v10.Inlines.Add("Property");
            mitem10.Header = v10;
            mitem10.Tag = o;
            mitem10.Click += MenuItem_View;

            menu.Items.Add(mitem1);
            menu.Items.Add(mitem2);
            menu.Items.Add(mitem3);
            menu.Items.Add(mitem4);
            menu.Items.Add(mitem5);
            menu.Items.Add(mitem6);
            menu.Items.Add(mitem7);
            menu.Items.Add(mitem8);
            menu.Items.Add(mitem9);
            menu.Items.Add(mitem10);
            tempTextBlock.ContextMenu = menu;

            return tempTextBlock;
        }

        private void MenuItem_View(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            TextBlock tb = item.Header as TextBlock;
            MessageBox.Show(tb.Text);

            FSInfo currentItem = item.Tag as FSInfo;
            MessageBox.Show(currentItem.Name);
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            host = HostTextBox.Text.Trim();
            user = UserTextBox.Text.Trim();
            password = PasswordTextBox.Text.Trim();
            LoadTree();
        }
    }
}
