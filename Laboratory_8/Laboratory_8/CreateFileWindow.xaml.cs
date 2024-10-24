using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace Laboratory_8;

public partial class CreateFileWindow : Window
{
    private readonly DirectoryInfo myPath;
    public CreateFileWindow(DirectoryInfo path)
    {
        myPath = path;
        InitializeComponent();
    }

    private void onCreateClick(object sender, RoutedEventArgs e)
    {
        var name = nameTextBox.Text;
        var isFile = fileType.IsChecked;
        var attributes = FileAttributes.Normal
                         | (isReadOnlyFile.IsChecked == true ? FileAttributes.ReadOnly : FileAttributes.Normal)
                         | (isArchive.IsChecked == true ? FileAttributes.Archive : FileAttributes.Normal)
                         | (isHiddenFile.IsChecked == true ? FileAttributes.Hidden : FileAttributes.Normal)
                         | (isSystemFile.IsChecked == true ? FileAttributes.System : FileAttributes.Normal);

        if (isFile == true && !Regex.IsMatch(name, @"^[a-zA-Z0-9_~-]{1,8}\.(txt|php|html)$"))
        {
            MessageBox.Show("Invalid file name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else
        {
            if (isFile == false)
            {
                Directory.CreateDirectory(Path.Combine(myPath.FullName, name));
            }
            else
            {
                File.Create(Path.Combine(myPath.FullName, name)).Close();
                File.SetAttributes(Path.Combine(myPath.FullName, name), attributes);
            }
        }
        Close();
    }

    private void onCancelClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}