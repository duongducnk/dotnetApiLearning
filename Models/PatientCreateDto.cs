using System.Text.Json.Serialization;

namespace MyDotNetApi.Models;

public class PatientCreateDto
{
    public string PatientName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public HookingUpStatus Status { get; set; }
    
    [JsonConverter(typeof(CustomDateTimeConverter))]
    public DateTime ScheduledHookUp { get; set; }
    //public TimeSession Session { get; set; }
}