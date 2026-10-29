using System.ComponentModel;
using AutoMapper;
using Azure;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OpenApi;
using MyDotNetApi.Data;
using MyDotNetApi.Models;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Builder;
using MyDotNetApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);

//dang ky db context voi sql server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Title = "Hệ thống quản lý lắp đặt thiết bị y tế Khoa Nội";
        document.Info.Version = "v1.0";
        document.Info.Description = "API phục vụ quản lý danh sách bệnh nhân và lịch lắp đặt Holter ECG";
        return Task.CompletedTask;
    });
});
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MyDotNetApi.Mappings.PatientProfile>();
});
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new CustomDateTimeConverter());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.MapGet("/", () => Results.Redirect("/scalar")).ExcludeFromDescription();
}

app.UseHttpsRedirection();


// //get: lay toan bo danh sach
// app.MapGet("/api/patients", async (AppDbContext db) =>
//     Results.Ok(await db.Patients.ToListAsync())
// )
// .WithSummary("Lấy danh sách tất cả bệnh nhân")
// .WithDescription("Trả về danh sách đầy đủ các bản ghi bệnh nhân và lịch hẹn lắp đặt thiết bị có trong cơ sở dữ liệu.")
// .Produces<List<Patient>>(StatusCodes.Status200OK);

// //get: lay theo id
// app.MapGet("/api/patients/{id}", async (int id, AppDbContext db) =>
//     await db.Patients.FindAsync(id) is Patient patient
//     ? Results.Ok(patient) 
//     : Results.NotFound(new { message = "Không tìm thấy bênh nhân" })
// )
// .WithSummary("Lấy thông tin bệnh nhân theo ID")
// .WithDescription("Tìm kiếm và trả về thông tin chi tiết của một bệnh nhân dựa trên ID trong hệ thống.")
// .Produces<Patient>(StatusCodes.Status200OK)
// .Produces(StatusCodes.Status404NotFound);


// // post: them moi benh nhan
// /// <summary>
// /// Thêm mới một bản ghi lịch hẹn lắp đặt thiết bị cho bệnh nhân.
// /// </summary>
// /// <remarks>
// /// Lưu ý: Trường **Session** (Buổi SA/CH) sẽ được hệ thống tự động tính toán dựa theo giờ của `ScheduledHookUp`.
// /// </remarks>
// /// <response code="201">Thêm mới thành công và trả về thông tin bản ghi.</response>
// /// <response code="400">Dữ liệu đầu vào không hợp lệ (Tên hoặc Khoa trống).</response>
// app.MapPost("/api/patients", async (PatientCreateDto newPatientDto, AppDbContext db, IMapper mapper) =>
// {
//     if (string.IsNullOrWhiteSpace(newPatientDto.PatientName) || string.IsNullOrWhiteSpace(newPatientDto.Department))
//     {
//         return Results.BadRequest(new { message = "Tên bệnh nhân và khoa/phòng không được để trống" });
//     }
    
//     //su dung automapper doi dto thanh model entity
//     var patient = mapper.Map<Patient>(newPatientDto);
//     db.Patients.Add(patient);
//     await db.SaveChangesAsync();

//     return Results.Created($"/api/patients/{patient.Id}", patient);
// })
// .WithSummary("Thêm mới một bệnh nhân.")
// .WithDescription("Tạo mới một bản ghi lịch hẹn lắp đặt thiết bị cho bệnh nhân. Trường Session (Buổi SA/CH) sẽ được hệ thống tự động tính toán dựa theo giờ của ScheduledHookUp (Sáng: <12h, Chiều: >=12h)")
// .Produces<Patient>(StatusCodes.Status201Created)
// .Produces(StatusCodes.Status400BadRequest);

// // put: sua thong tin benh nhan
// app.MapPut("/api/patients/{id}", async (int id, PatientUpdateDto updatedPatientDto, AppDbContext db, IMapper mapper) =>
// {
//     var patient = await db.Patients.FindAsync(id);
//     if (patient is null)
//     {
//         return Results.NotFound(new { message = "Không tìm thấy bệnh nhân để cập nhật" });
//     }

//     // patient.PatientName = updatedPatient.PatientName;
//     // patient.Department = updatedPatient.Department;
//     // patient.Status = updatedPatient.Status;
//     // patient.ScheduledHookUp = updatedPatient.ScheduledHookUp;
//     // patient.Session = updatedPatient.Session;
    
//     // Dùng AutoMapper để đổ toàn bộ dữ liệu từ updateDto đè vào entity patient đang được EF Core theo dõi
//     mapper.Map(updatedPatientDto, patient);

//     await db.SaveChangesAsync();
//     return Results.NoContent();
// })
// .WithSummary("Cập nhật thông tin bệnh nhân")
// .WithDescription("Cập nhật lại thông tin của bệnh nhân theo ID. Trường Session cũng sẽ được tính toán lại tự động")
// .Produces(StatusCodes.Status204NoContent)
// .Produces(StatusCodes.Status404NotFound);

// //delete: xoa benh nhan 
// app.MapDelete("/api/patients/{id}", async (int id, AppDbContext db) =>
// {
//     var patient = await db.Patients.FindAsync(id);
//     if (patient is null)
//     {
//         return Results.NotFound(new { message = "Không tìm thấy bệnh nhân để xoá" });
//     }

//     db.Patients.Remove(patient);
//     await db.SaveChangesAsync();
//     return Results.NoContent();
// })
// .WithSummary("Xóa bệnh nhân theo ID")
// .WithDescription("Xóa hoàn toàn bản ghi thông tin bệnh nhân ra khỏi cơ sở dữ liệu dựa trên ID truyền vào")
// .Produces(StatusCodes.Status204NoContent)
// .Produces(StatusCodes.Status404NotFound);

// Đăng ký nhóm các endpoint quản lý bệnh nhân
app.MapPatientEndpoints();

app.Run();
// var summaries = new[]
// {
//     "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
// };

// app.MapGet("/weatherforecast", () =>
// {
//     var forecast =  Enumerable.Range(1, 5).Select(index =>
//         new WeatherForecast
//         (
//             DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//             Random.Shared.Next(-20, 55),
//             summaries[Random.Shared.Next(summaries.Length)]
//         ))
//         .ToArray();
//     return forecast;
// })
// .WithName("GetWeatherForecast");

// app.Run();

// record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
// {
//     public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
// }
