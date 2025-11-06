using System.ComponentModel;

namespace 日志删除工具.Models
{
    /// <summary>
    /// 清理结果模型
    /// </summary>
    public class CleanupResult : INotifyPropertyChanged
    {
        /// <summary>
        /// 删除的文件数量
        /// </summary>
        public int DeletedFilesCount { get; set; }

        /// <summary>
        /// 删除的总大小（字节）
        /// </summary>
        public long TotalSizeDeleted { get; set; }

        /// <summary>
        /// 清理时间
        /// </summary>
        public DateTime CleanupTime { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);

        /// <summary>
        /// 已删除文件列表
        /// </summary>
        public List<string> DeletedFiles { get; set; } = new List<string>();

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}