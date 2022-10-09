using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
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
using Path = System.IO.Path;

namespace FolderSync
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow() {
      InitializeComponent();
      LabelFromPath.Content = Properties.Settings.Default.FromPath;
      LabelToPath.Content = Properties.Settings.Default.ToPath;
    }

    private void LabelFromPath_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
      string dir = ShowPathDialog();
      if (!string.IsNullOrEmpty(dir))
      {
        LabelToPath.Content = dir;
        Properties.Settings.Default.FromPath = dir;
      }
      Properties.Settings.Default.Save();
    }

    private void LabelToPath_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
      string dir = ShowPathDialog();
      if (!string.IsNullOrEmpty(dir))
      {
        LabelToPath.Content = dir;
        Properties.Settings.Default.ToPath = dir;
      }
      Properties.Settings.Default.Save();
    }
    private string ShowPathDialog() {
      var dialog = new CommonOpenFileDialog();
      dialog.IsFolderPicker = true;
      dialog.ShowHiddenItems = true;
      if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
      {
        return dialog.FileName;
      }
      return string.Empty;

    }
    public static void Sync(string dirFrom, string dirTo) {
      var result1 = GetFiles(dirFrom);
      var result2 = GetFiles(dirTo);

      var dirsToRemove = result2.dirs.Where(x => !result1.dirs.Contains(x));
      var filesToRemove = result2.files.Where(x => !result1.files.Contains(x));
      var filesToAdd = result1.files.Where(x => !result2.files.Contains(x));
      // sync
      foreach (var fileToRemove in filesToRemove) 
        Console.WriteLine("Deleting : " + Path.Combine(dirTo, fileToRemove.file));

      foreach (var dirToRemove in dirsToRemove)
        Console.WriteLine("Deleting : " + Path.Combine(dirTo, dirToRemove));

      foreach (var fileToAdd in filesToAdd)
        Console.WriteLine("Adding : " + Path.Combine(dirTo, fileToAdd.file));
    }
    public static (HashSet<string> dirs, HashSet<(string file, long tick)> files) GetFiles(string dir) {

      string CleanPath(string path) => path.Substring(dir.Length).TrimStart('/', '\\');

      // replace this with a hashing method if you need
      long GetUnique(string path) => new FileInfo(path).LastWriteTime.Ticks;

      var directories = Directory.GetDirectories(dir, "*.*", SearchOption.AllDirectories);

      var dirHash = directories.Select(CleanPath).ToHashSet();

      // this could be paralleled if need be (if using a hash) 
      var fileHash = directories.SelectMany(Directory.EnumerateFiles)
         .Select(file => (name: CleanPath(file), ticks: GetUnique(file)))
         .ToHashSet();

      return (dirHash, fileHash);
    }

    private void ButtonSync_Click(object sender, RoutedEventArgs e) {
      try
      {
        Sync(Properties.Settings.Default.FromPath, Properties.Settings.Default.ToPath);
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message);
      }
    }
  }
}
