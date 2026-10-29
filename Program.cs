using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyDotNetApi.Data;
using MyDotNetApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

//dang ky db context voi sql server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
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

//data gia lap trong bo nho
// var patients = new List<Patient>();
// var idCounter = 1;

//get: lay toan bo danh sach
app.MapGet("/api/patients", async (AppDbContext db) =>
    Results.Ok(await db.Patients.ToListAsync())
);

//get: lay theo id
app.MapGet("/api/patients/{id}", async (int id, AppDbContext db) =>
    await db.Patients.FindAsync(id) is Patient patient
    ? Results.Ok(patient) 
    : Results.NotFound(new { message = "Không tìm thấy bênh nhân" })
);



// post: them moi benh nhan
app.MapPost("/api/patients", async (PatientCreateDto newPatientDto, AppDbContext db, IMapper mapper) =>
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
});

// put: sua thong tin benh nhan
app.MapPut("/api/patients/{id}", async (int id, PatientUpdateDto updatedPatientDto, AppDbContext db, IMapper mapper) =>
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
});

//delete: xoa benh nhan 
app.MapDelete("/api/patients/{id}", async (int id, AppDbContext db) =>
{
    var patient = await db.Patients.FindAsync(id);
    if (patient is null)
    {
        return Results.NotFound(new { message = "Không tìm thấy bệnh nhân để xoá" });
    }

    db.Patients.Remove(patient);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

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
