using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Path = System.IO.Path;

namespace FolderSync
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private SynchronizationManager manager { get; set; }
    public MainWindow()
    {
      InitializeComponent();
      LabelFromPath.Content = Properties.Settings.Default.FromPath;
      LabelToPath.Content = Properties.Settings.Default.ToPath;
      manager = new SynchronizationManager();
    }

    private void LabelFromPath_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      string dir = ShowPathDialog();
      if (!string.IsNullOrEmpty(dir))
      {
        LabelFromPath.Content = dir;
        Properties.Settings.Default.FromPath = dir;
      }
      Properties.Settings.Default.Save();
    }

    private void LabelToPath_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      string dir = ShowPathDialog();
      if (!string.IsNullOrEmpty(dir))
      {
        LabelToPath.Content = dir;
        Properties.Settings.Default.ToPath = dir;
      }
      Properties.Settings.Default.Save();
    }
    private string ShowPathDialog()
    {
      var dialog = new CommonOpenFileDialog();
      dialog.IsFolderPicker = true;
      dialog.ShowHiddenItems = true;
      if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
      {
        return dialog.FileName;
      }
      return string.Empty;

    }
    public async void Sync(string dirFrom, string dirTo)
    {
      try
      {
        manager.loading.Show();
        await Task.Run(async() => await manager.StartSynchronization(dirFrom,dirTo)).ContinueWith(task =>
        {
          manager.loading.Hide();
        }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        MessageBox.Show("Done");
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message);
      }
      finally {
        manager.loading.Hide();
      }

    }
    private void ButtonSync_Click(object sender, RoutedEventArgs e)
    {
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
