using Microsoft.AspNetCore.Mvc;
using MyDotNetApi.Models;

namespace MyDotNetApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private static List<Patient> patients = new List<Patient>
        {
            new Patient
            {
                Id = 1,
                PatientName = "Nguyễn Văn A",
                Department = "Khoa Cấp Cứu",
                Status = HookingUpStatus.Đanglắp,
                ScheduledHookUp = DateTime.Now.AddDays(1),
                Session = TimeSession.CH
            },
            new Patient
            {
                Id = 2,
                PatientName = "Trần Thị B",
                Department = "Khoa Nội Tổng Quát",
                Status = HookingUpStatus.Chưalắp,
                ScheduledHookUp = DateTime.Now.AddDays(2),
                Session = TimeSession.SA
            }
        };

    //get: lay danh sach toan bo benh nhan /api/patients
        [HttpGet]
        public ActionResult<IEnumerable<Patient>> GetAllPatient()
        {
            return Ok(patients);
        }

        //get: lay thong tin benh nhan theo id /api/patients/1
        [HttpGet("{id}")]
        public ActionResult<Patient> GetPatientById(int id)
        {
            var patient = patients.FirstOrDefault(p => p.Id == id);
            if (patient == null)
            {
                return NotFound(new { message = "Không tìm thấy bệnh nhân" });
            }
            return Ok(patient);
        }

        //post: them benh nhan /api/patients
        [HttpPost]
        public ActionResult<Patient> CreatePatient([FromBody] Patient newPatient)
        {
            //tu dong tang id
            newPatient.Id = patients.Count > 0 ? patients.Max(p => p.Id) + 1 : 1;
            patients.Add(newPatient);
            return CreatedAtAction(nameof(GetPatientById), new { id = newPatient.Id}, newPatient);
        }

        //put: cap nhat thong tin benh nhan /api/patients/1
        [HttpPut("{id}")]
        public ActionResult UpdatePatient(int id, [FromBody] Patient updatedPatient)
        {
            var patient = patients.FirstOrDefault(p => p.Id == id);
            if (patient == null)
            {
                return NotFound(new { message = "Không tìm thấy bệnh nhân để cập nhật" });
            }

            //cap nhat thong tin
            patient.PatientName = updatedPatient.PatientName;
            patient.Department = updatedPatient.Department;
            patient.Status = updatedPatient.Status;
            patient.ScheduledHookUp = updatedPatient.ScheduledHookUp;
            patient.Session = updatedPatient.Session;

            return Ok(new { message = "Cập nhật thành công", data = patient });
        }

        //delete: xoa benh nhan /api/patients/1
        [HttpDelete("{id}")]
        public ActionResult DeletePatient(int id)
        {
            var patient = patients.FirstOrDefault(p => p.Id == id);
            if (patient == null)
            {
                return NotFound(new { message = "Không tìm thấy bệnh nhân để xoá" });
            }

            patients.Remove(patient);
            return Ok(new { message = "Xoá bệnh nhân thành công" });
        }
    }
}
