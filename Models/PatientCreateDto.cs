using System.ComponentModel;
using System.Text.Json.Serialization;

namespace MyDotNetApi.Models;

public class PatientCreateDto
{
    /// <summary>
    /// Họ và tên đầy đủ của bệnh nhân
    /// </summary>
    public string PatientName { get; set; } = string.Empty;

    /// <summary>
    /// Khoa hoặc phòng bệnh nhân đang điều trị
    /// </summary>
    public string Department { get; set; } = string.Empty;

    /// <summary>
    /// Tình trạng lắp đặt (0: Chưa lắp, 1: Đang lắp, 2: Đã tháo, 3: Khác)
    /// </summary>
    public HookingUpStatus Status { get; set; }
    
    /// <summary>
    /// Thời gian dự kiến lắp đặt (Hỗ trợ định dạng: yyyy-MM-dd HH:mm)
    /// </summary>
    [JsonConverter(typeof(CustomDateTimeConverter))]
    public DateTime ScheduledHookUp { get; set; }
    //public TimeSession Session { get; set; }

}