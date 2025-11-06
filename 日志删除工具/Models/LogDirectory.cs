using System.ComponentModel.DataAnnotations;

namespace 日志删除工具.Models
{
    /// <summary>
    /// 日志目录配置模型
    /// </summary>
    public class LogDirectory
    {
        /// <summary>
        /// 目录路径
        /// </summary>
        [Required]
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; } = true;
    }
}