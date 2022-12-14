using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FolderSync
{
  public class SynchronizationManager
  {
    public readonly LoadingDialog loading;
    public SynchronizationManager() {
      loading = new LoadingDialog();
    }
    public (HashSet<string> dirs, HashSet<(string file, long tick)> files) GetFiles(string dir)
    {

      string CleanPath(string path) => path[dir.Length..].TrimStart('/', '\\');

      // replace this with a hashing method if you need
      long GetUnique(string path) => new FileInfo(path).LastWriteTime.Ticks;

      var directories = Directory.GetDirectories(dir, "*.*", SearchOption.AllDirectories);
      List<string> finalDirs = directories.ToList();
      finalDirs.Add(dir);
      var dirHash = finalDirs.Select(CleanPath).ToHashSet();
      // this could be paralleled if need be (if using a hash) 
      var fileHash = finalDirs.SelectMany(Directory.EnumerateFiles)
         .Select(file => (name: CleanPath(file), ticks: GetUnique(file)))
         .ToHashSet();

      return (dirHash, fileHash);
    }
    public async Task StartSynchronization(string dirFrom, string dirTo)
    {

      var result1 = GetFiles(dirFrom);
      var (dirs, files) = GetFiles(dirTo);

      var filesToAdd = result1.files.Where(x => !files.Contains(x));
      var dirsToAdd = result1.dirs.Where(x => !dirs.Contains(x));

      int counter = 0;
      int count = filesToAdd.Count() + dirsToAdd.Count();
      foreach (var dir in dirsToAdd)
      {
        if (System.IO.Directory.Exists(Path.Combine(dirTo, dir))) continue;

        System.IO.Directory.CreateDirectory(Path.Combine(dirTo, dir));
        loading.Dispatcher.Invoke(new Action(() => loading.SetStatusPercent(GetPercent(counter, count))));
      }

      foreach (var (file, tick) in filesToAdd)
      {
        counter++;
        string oldFileDir = Path.Combine(dirFrom, file);
        string newFileDir = Path.Combine(dirTo, file);
        await CompareAndCopy(oldFileDir, newFileDir);
        loading.Dispatcher.Invoke(new Action(() => loading.SetStatusPercent(GetPercent(counter, count))));
      }
    }
    public static Task CompareAndCopy(string firstFileDir, string secondFileDir)
    {
      if (File.Exists(firstFileDir))
      {
        FileInfo oldFile = new(secondFileDir);
        FileInfo newFile = new(firstFileDir);
        if (oldFile.LastAccessTime > newFile.LastAccessTime)
        {
          File.Copy(secondFileDir, firstFileDir, true);
        }
        else
        {
          File.Copy(firstFileDir, secondFileDir, true);
        }
      }
      else
      {
        File.Copy(secondFileDir, firstFileDir, false);
      }
      return Task.CompletedTask;
    }
    public static int GetPercent(int counter, int count)
    {
      double result = (double)counter / count * 100;
      return (int)Math.Round(result, 0);
    }
  }
}
