using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyDotNetApi.Models;

public class Patient
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Id tu dong tang boi co so du lieu
    public int Id { get; set; }

    public string PatientName { get; set; } = string.Empty; // ten benh nhan
    public string Department { get; set; } = string.Empty; // khoa / phong
    public HookingUpStatus Status {  get; set; }  // tinh trang
    public DateTime ScheduledHookUp {  get; set; } // ngay lap du kien
    public TimeSession Session { get; set; }
}

// Enum cho thoi diem sang chieu
public enum TimeSession
{
    SA = 0,  // sang
    CH = 1  // chieu
}

// Enum cho tinh trang lap dat
public enum HookingUpStatus
{
    ChuaLap = 0,
    DangLap = 1, 
    Dathao = 2, 
    Khac = 3
}