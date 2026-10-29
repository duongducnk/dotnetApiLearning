using AutoMapper;
using MyDotNetApi.Models;

namespace MyDotNetApi.Mappings;

public class PatientProfile : Profile
{
    public PatientProfile()
    {
        // Định nghĩa chiều map: Từ DTO sang Model Entity
        CreateMap<PatientCreateDto, Patient>()
            .ForMember(dest => dest.Session, opt => opt.MapFrom(src =>
            src.ScheduledHookUp.Hour < 12 ? TimeSession.SA : TimeSession.CH)
        );
        
        // Nếu bạn muốn map ngược lại từ Model sang DTO (để trả về client nếu cần)
        // CreateMap<Patient, PatientResponseDto>();

        CreateMap<PatientUpdateDto, Patient>()
            .ForMember(dest => dest.Session, opt => opt.MapFrom(src =>
            src.ScheduledHookUp.Hour < 12 ? TimeSession.SA : TimeSession.CH)
        );
    }
}