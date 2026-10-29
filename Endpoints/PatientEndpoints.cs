using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyDotNetApi.Data;
using MyDotNetApi.Models;

namespace MyDotNetApi.Endpoints;

public static class PatientEndpoints
{
    public static void MapPatientEndpoints(this IEndpointRouteBuilder routes)
    {
        // Nhóm tất cả các API có chung tiền tố /api/patients
        var group = routes.MapGroup("/api/patients").WithTags("Patient Management");

        // 1. GET: Lấy toàn bộ danh sách bệnh nhân
        group.MapGet("/", async (AppDbContext db) =>
            Results.Ok(await db.Patients.ToListAsync())
        )
        .WithSummary("Lấy danh sách tất cả bệnh nhân")
        .WithDescription("Trả về danh sách đầy đủ các bản ghi bệnh nhân và lịch hẹn lắp đặt thiết bị có trong cơ sở dữ liệu.")
        .Produces<List<Patient>>(StatusCodes.Status200OK);

        // 2. GET: Lấy thông tin bệnh nhân theo ID
        group.MapGet("/{id}", async (int id, AppDbContext db) =>
            await db.Patients.FindAsync(id) is Patient patient
            ? Results.Ok(patient) 
            : Results.NotFound(new { message = "Không tìm thấy bênh nhân" })
        )
        .WithSummary("Lấy thông tin bệnh nhân theo ID")
        .WithDescription("Tìm kiếm và trả về thông tin chi tiết của một bệnh nhân dựa trên ID trong hệ thống.")
        .Produces<Patient>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // 3. POST: Thêm mới bệnh nhân
        group.MapPost("/", async (PatientCreateDto newPatientDto, AppDbContext db, IMapper mapper) =>
        {
            if (string.IsNullOrWhiteSpace(newPatientDto.PatientName) || string.IsNullOrWhiteSpace(newPatientDto.Department))
            {
                return Results.BadRequest(new { message = "Tên bệnh nhân và khoa/phòng không được để trống" });
            }
            
            //su dung automapper doi dto thanh model entity
            var patient = mapper.Map<Patient>(newPatientDto);
            db.Patients.Add(patient);
            await db.SaveChangesAsync();

            return Results.Created($"/api/patients/{patient.Id}", patient);
        })
        .WithSummary("Thêm mới một bệnh nhân.")
        .WithDescription("Tạo mới một bản ghi lịch hẹn lắp đặt thiết bị cho bệnh nhân. Trường Session (Buổi SA/CH) sẽ được hệ thống tự động tính toán dựa theo giờ của ScheduledHookUp (Sáng: <12h, Chiều: >=12h)")
        .Produces<Patient>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        // 4. PUT: Sửa thông tin bệnh nhân
        group.MapPut("/{id}", async (int id, PatientUpdateDto updatedPatientDto, AppDbContext db, IMapper mapper) =>
        {
            var patient = await db.Patients.FindAsync(id);
            if (patient is null)
            {
                return Results.NotFound(new { message = "Không tìm thấy bệnh nhân để cập nhật" });
            }

            // patient.PatientName = updatedPatient.PatientName;
            // patient.Department = updatedPatient.Department;
            // patient.Status = updatedPatient.Status;
            // patient.ScheduledHookUp = updatedPatient.ScheduledHookUp;
            // patient.Session = updatedPatient.Session;
            
            // Dùng AutoMapper để đổ toàn bộ dữ liệu từ updateDto đè vào entity patient đang được EF Core theo dõi
            mapper.Map(updatedPatientDto, patient);

            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithSummary("Cập nhật thông tin bệnh nhân")
        .WithDescription("Cập nhật lại thông tin của bệnh nhân theo ID. Trường Session cũng sẽ được tính toán lại tự động")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        // 5. DELETE: Xoá bệnh nhân
        group.MapDelete("/api/patients/{id}", async (int id, AppDbContext db) =>
        {
            var patient = await db.Patients.FindAsync(id);
            if (patient is null)
            {
                return Results.NotFound(new { message = "Không tìm thấy bệnh nhân để xoá" });
            }

            db.Patients.Remove(patient);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithSummary("Xóa bệnh nhân theo ID")
        .WithDescription("Xóa hoàn toàn bản ghi thông tin bệnh nhân ra khỏi cơ sở dữ liệu dựa trên ID truyền vào")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}