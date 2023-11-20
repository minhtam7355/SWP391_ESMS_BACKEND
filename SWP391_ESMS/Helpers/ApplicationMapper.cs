using AutoMapper;
using SWP391_ESMS.Models.Domain;
using SWP391_ESMS.Models.ViewModels;

namespace SWP391_ESMS.Helpers
{
    public class ApplicationMapper : Profile
    {

        public ApplicationMapper()
        {
            // CRUD MAPPING
            CreateMap<ConfigurationSetting, ConfigurationSettingModel>()
                .ReverseMap();

            CreateMap<Course, CourseModel>()
                .ForMember(dest => dest.MajorName, opt => opt.MapFrom(src => src.Major!.MajorName));

            CreateMap<CourseModel, Course>();

            CreateMap<ExamFormat, ExamFormatModel>()
                .ReverseMap();

            CreateMap<ExamPeriod, ExamPeriodModel>()
                .ForMember(dest => dest.ExamFormatCode, opt => opt.MapFrom(src => src.ExamFormat!.ExamFormatCode))
                .ForMember(dest => dest.ExamFormatName, opt => opt.MapFrom(src => src.ExamFormat!.ExamFormatName));

            CreateMap<ExamPeriodModel, ExamPeriod>();

            CreateMap<ExamRoom, ExamRoomModel>()
                .ReverseMap();

            CreateMap<ExamSession, ExamSessionModel>()
                .ForMember(dest => dest.ExamSessionId, opt => opt.MapFrom(src => src.ExamSessionId))
                .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId))
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course!.CourseName))
                .ForMember(dest => dest.ExamPeriodId, opt => opt.MapFrom(src => src.ExamPeriodId))
                .ForMember(dest => dest.ExamPeriodName, opt => opt.MapFrom(src => src.ExamPeriod!.ExamPeriodName))
                .ForMember(dest => dest.ExamFormatCode, opt => opt.MapFrom(src => src.ExamPeriod!.ExamFormat!.ExamFormatCode))
                .ForMember(dest => dest.ExamFormatName, opt => opt.MapFrom(src => src.ExamPeriod!.ExamFormat!.ExamFormatName))
                .ForMember(dest => dest.ExamDate, opt => opt.MapFrom(src => src.ExamDate))
                .ForMember(dest => dest.ShiftId, opt => opt.MapFrom(src => src.ShiftId))
                .ForMember(dest => dest.ShiftName, opt => opt.MapFrom(src => src.Shift!.ShiftName))
                .ForMember(dest => dest.RoomId, opt => opt.MapFrom(src => src.RoomId))
                .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.Room!.RoomName))
                .ForMember(dest => dest.StudentsEnrolled, opt => opt.MapFrom(src => src.StudentsEnrolled))
                .ForMember(dest => dest.TeacherId, opt => opt.MapFrom(src => src.TeacherId))
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher!.FullName))
                .ForMember(dest => dest.StaffId, opt => opt.MapFrom(src => src.StaffId))
                .ForMember(dest => dest.StaffName, opt => opt.MapFrom(src => src.Staff!.FullName))
                .ForMember(dest => dest.IsPassed, opt => opt.MapFrom(src => src.IsPassed))
                .ForMember(dest => dest.IsPaid, opt => opt.MapFrom(src => src.IsPaid));

            CreateMap<ExamSessionModel, ExamSession>();

            CreateMap<ExamShift, ExamShiftModel>()
                .ReverseMap();

            CreateMap<Major, MajorModel>()
                .ReverseMap();

            CreateMap<ProctoringRequest, ProctoringRequestModel>()
                .ForMember(dest => dest.ExamPeriodName, opt => opt.MapFrom(src => src.ExamSession!.ExamPeriod!.ExamPeriodName))
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.ExamSession!.Course!.CourseName))
                .ForMember(dest => dest.ExamDate, opt => opt.MapFrom(src => src.ExamSession!.ExamDate))
                .ForMember(dest => dest.ShiftName, opt => opt.MapFrom(src => src.ExamSession!.Shift!.ShiftName))
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher!.FullName));

            CreateMap<ProctoringRequestModel, ProctoringRequest>();

            CreateMap<Staff, StaffModel>()
                .ReverseMap();

            CreateMap<Student, StudentModel>()
                .ForMember(dest => dest.MajorName, opt => opt.MapFrom(src => src.Major!.MajorName));

            CreateMap<StudentModel, Student>();

            CreateMap<Teacher, TeacherModel>()
                .ForMember(dest => dest.MajorName, opt => opt.MapFrom(src => src.Major!.MajorName));

            CreateMap<TeacherModel, Teacher>();

            // ACCESS MAPPING
            CreateMap<Student, UserInfo>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.StudentId))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => "Student"))
                .ReverseMap();

            CreateMap<Teacher, UserInfo>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.TeacherId))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => "Teacher"))
                .ReverseMap();

            CreateMap<Staff, UserInfo>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.StaffId))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.StaffRole))
                .ReverseMap();

            CreateMap<SignupModel, Student>()
                .ForMember(dest => dest.StudentId, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => BC.EnhancedHashPassword(src.Password, 13)))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender));

        }
    }
}
