using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Input;
using MessageBox = System.Windows.MessageBox;
using System.Windows.Forms;
using System.Windows.Interop;

namespace Laboratory_8
{
    public partial class MainWindow : Window
    {
        private DirectoryInfo? currentDirectory = null;

        public MainWindow()
        {
            InitializeComponent();
            TreeView.ContextMenu = new ContextMenu();
        }

        private void onMenuExitClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void onMenuOpenClick(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderBrowserDialog() { Description = "Select directory to open", UseDescriptionForTitle=true };
            var win32Parent = new NativeWindow();
            win32Parent.AssignHandle(new WindowInteropHelper(this).Handle);
            var result = dlg.ShowDialog(win32Parent);
            if (result != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            if (!Directory.Exists(dlg.SelectedPath))
            {
                MessageBox.Show(this, "Invalid path!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            currentDirectory = new DirectoryInfo(dlg.SelectedPath);
            displayFiles();
        }

        private void displayFiles()
        {
            TreeView.Items.Clear();

            if (currentDirectory is null)
            {
                return;
            }

            var subDirectories = currentDirectory.EnumerateDirectories().Select(getTVitem);
            var subFiles = currentDirectory.EnumerateFiles().Select(getTVitem);
            foreach (var item in subDirectories.Concat(subFiles))
            {
                TreeView.Items.Add(item);
            }

            TreeView.ContextMenu?.Items.Clear();
            var createMenuItem = new MenuItem
            {
                Header = "Create",
                Tag = currentDirectory.FullName,
            };
            createMenuItem.Click += TreeViewDirectoryItem_OnCreate;
            TreeView.ContextMenu?.Items.Add(createMenuItem);
        }
        private TreeViewItem getTVitem(DirectoryInfo directoryInfo)
        {
            var subDirectories = directoryInfo.EnumerateDirectories().Select(getTVitem);
            var subFiles = directoryInfo.EnumerateFiles().Select(getTVitem);

            var root = new TreeViewItem
            {
                Header = directoryInfo.Name,
                Tag = directoryInfo.FullName,
                ContextMenu = new ContextMenu()
            };

            var createMenuItem = new MenuItem
            {
                Header = "Create",
                Tag = directoryInfo.FullName,
            };
            createMenuItem.Click += TreeViewDirectoryItem_OnCreate;
            root.ContextMenu.Items.Add(createMenuItem);

            var deleteMenuItem = new MenuItem
            {
                Header = "Delete",
                Tag = directoryInfo.FullName
            };
            deleteMenuItem.Click += TreeViewDirectoryItem_OnDelete;
            root.ContextMenu.Items.Add(deleteMenuItem);

            foreach (var item in subDirectories.Concat(subFiles))
            {
                root.Items.Add(item);
            }

            return root;
        }

        private TreeViewItem getTVitem(FileInfo fileInfo)
        {
            var item = new TreeViewItem
            {
                Header = fileInfo.Name,
                Tag = fileInfo.FullName,
                ContextMenu = new ContextMenu(),
            };
            item.Selected += TreeViewFileItem_OnSelected;
            item.MouseDoubleClick += TreeViewFileItem_OnDoubleClick;

            var openMenuItem = new MenuItem
            {
                Header = "Open",
                Tag = fileInfo.FullName
            };
            openMenuItem.Click += TVitemOnOpen;
            item.ContextMenu.Items.Add(openMenuItem);

            var deleteMenuItem = new MenuItem
            {
                Header = "Delete",
                Tag = fileInfo.FullName
            };
            deleteMenuItem.Click += TVitemOnDelete;
            item.ContextMenu.Items.Add(deleteMenuItem);

            return item;
        }
        private void TreeViewFileItem_OnDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = e.Source as TreeViewItem;

            if (item?.Tag is not string path || !File.Exists(path))
            {
                MessageBox.Show(this, "Invalid path", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            fileViewer.Text = File.ReadAllText(path, Encoding.UTF8);
        }

        private void TreeViewFileItem_OnSelected(object sender, RoutedEventArgs e)
        {
            var menuItem = e.Source as TreeViewItem;

            if (menuItem?.Tag is not string path || !File.Exists(path))
            {
                MessageBox.Show(this, "Invalid path", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var attributes = File.GetAttributes(path);
            var dosAttributes = new StringBuilder();
            dosAttributes.Append((attributes & FileAttributes.ReadOnly) != 0 ? 'R' : '-');
            dosAttributes.Append((attributes & FileAttributes.Archive) != 0 ? 'A' : '-');
            dosAttributes.Append((attributes & FileAttributes.Hidden) != 0 ? 'H' : '-');
            dosAttributes.Append((attributes & FileAttributes.System) != 0 ? 'S' : '-');

            AttributeTextBlock.Text = dosAttributes.ToString();
        }

        private void TVitemOnOpen(object sender, RoutedEventArgs e)
        {
            var menuItem = e.Source as MenuItem;

            if (menuItem?.Tag is not string path || !File.Exists(path))
            {
                MessageBox.Show(this, "Invalid path", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            fileViewer.Text = File.ReadAllText(path, Encoding.UTF8);
        }

        private void TreeViewDirectoryItem_OnCreate(object sender, RoutedEventArgs e)
        {
            var menuItem = e.Source as MenuItem;

            if (menuItem?.Tag is not string path || !Directory.Exists(path))
            {
                MessageBox.Show("Invalid path", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var window = new CreateFileWindow(new DirectoryInfo(path));
            window.ShowDialog();
            displayFiles();
        }

        private void TreeViewDirectoryItem_OnDelete(object sender, RoutedEventArgs e)
        {
            var menuItem = e.Source as MenuItem;

            if (menuItem?.Tag is not string path || !Directory.Exists(path))
            {
                MessageBox.Show("Invalid path", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            foreach (var file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
            {
                if (File.GetAttributes(file).HasFlag(FileAttributes.ReadOnly))
                {
                    File.SetAttributes(file, File.GetAttributes(file) & ~FileAttributes.ReadOnly);
                }

                File.Delete(file);
            }
            Directory.Delete(path);
            displayFiles();
        }

        private void TVitemOnDelete(object sender, RoutedEventArgs e)
        {
            var menuItem = e.Source as MenuItem;

            if (menuItem?.Tag is not string path || !File.Exists(path))
            {
                MessageBox.Show("Invalid path", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (File.GetAttributes(path).HasFlag(FileAttributes.ReadOnly))
            {
                File.SetAttributes(path, File.GetAttributes(path) & ~FileAttributes.ReadOnly);
            }

            File.Delete(path);
            displayFiles();
        }
    }
}
