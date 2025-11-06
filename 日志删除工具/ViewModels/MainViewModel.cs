using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using 日志删除工具.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Timers;

namespace 日志删除工具.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly int _daysThreshold = 30;
        private readonly System.Timers.Timer _cleanupTimer;

        [ObservableProperty]
        private ObservableCollection<LogDirectory> _logDirectories = new();

        [ObservableProperty]
        private CleanupResult _lastCleanupResult = new();

        [ObservableProperty]
        private ObservableCollection<CleanupResult> _cleanupHistory = new();

        [ObservableProperty]
        private bool _isCleaningUp;

        public MainViewModel()
        {
            // 添加示例目录
            // LogDirectories.Add(new LogDirectory { Path = @"C:\Logs", IsEnabled = true });
            // LogDirectories.Add(new LogDirectory { Path = @"D:\ApplicationLogs", IsEnabled = true });

            // 设置定时器，每天执行一次清理 (24小时 = 86400000毫秒)
            _cleanupTimer = new System.Timers.Timer(86400000);
            _cleanupTimer.Elapsed += OnTimedEvent;
            _cleanupTimer.AutoReset = true;
            _cleanupTimer.Enabled = true;

            // 立即执行一次清理
            CleanupLogs();
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                CleanupLogs();
            });
        }

        [RelayCommand]
        private void AddDirectory()
        {
            LogDirectories.Add(new LogDirectory { Path = "", IsEnabled = true });
        }

        [RelayCommand]
        private void RemoveDirectory(LogDirectory logDir)
        {
            if (logDir != null)
            {
                LogDirectories.Remove(logDir);
            }
        }

        [RelayCommand]
        private void CleanupLogs()
        {
            if (IsCleaningUp) return;

            IsCleaningUp = true;
            var result = new CleanupResult
            {
                CleanupTime = DateTime.Now
            };

            try
            {
                foreach (var logDir in LogDirectories)
                {
                    if (!logDir.IsEnabled || string.IsNullOrWhiteSpace(logDir.Path))
                        continue;

                    if (!Directory.Exists(logDir.Path))
                        continue;

                    var files = Directory.GetFiles(logDir.Path, "*.txt", SearchOption.TopDirectoryOnly);
                    foreach (var file in files)
                    {
                        try
                        {
                            var fileInfo = new FileInfo(file);
                            if (fileInfo.LastWriteTime < DateTime.Now.AddDays(-_daysThreshold))
                            {
                                // 先获取文件大小再删除
                                long fileSize = fileInfo.Length;
                                string fileName = fileInfo.Name;
                                
                                fileInfo.Delete();
                                
                                result.DeletedFilesCount++;
                                result.TotalSizeDeleted += fileSize;
                                result.DeletedFiles.Add(fileName);
                            }
                        }
                        catch (Exception ex)
                        {
                            result.ErrorMessage += $"删除文件 {file} 时出错: {ex.Message}\n";
                        }
                    }
                }

                LastCleanupResult = result;
                CleanupHistory.Insert(0, result);
            }
            catch (Exception ex)
            {
                LastCleanupResult = new CleanupResult
                {
                    CleanupTime = DateTime.Now,
                    ErrorMessage = $"清理过程中发生错误: {ex.Message}"
                };
                CleanupHistory.Insert(0, LastCleanupResult);
            }
            finally
            {
                IsCleaningUp = false;
            }
        }
    }
}