using Microsoft.WindowsAPICodePack.Dialogs;
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

namespace FolderSync
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window {
    public MainWindow() {
      InitializeComponent();
    }

    private void LabelFromPath_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
      ShowPathDialog();
      //Properties.Settings.Default.FromPath =
    }

    private void LabelToPath_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
      ShowPathDialog();
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
  }
}
