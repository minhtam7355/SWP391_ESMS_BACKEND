using AutoMapper;
using SWP391_ESMS.Models.Domain;
using SWP391_ESMS.Models.ViewModels;
using SWP391_ESMS.Repositories;

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
                .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId))
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.CourseName))
                .ForMember(dest => dest.MajorName, opt => opt.MapFrom(src => src.Major!.MajorName));

            CreateMap<AddCourseModel, Course>()
                .ForMember(dest => dest.CourseId, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.CourseName))
                .ForMember(dest => dest.MajorId, opt => opt.MapFrom(src => src.MajorId));

            CreateMap<UpdateCourseModel, Course>()
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.CourseName))
                .ForMember(dest => dest.MajorId, opt => opt.MapFrom(src => src.MajorId));

            CreateMap<ExamRoom, ExamRoomModel>()
                .ForMember(dest => dest.RoomId, opt => opt.MapFrom(src => src.RoomId))
                .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.RoomName));

            CreateMap<AddExamRoomModel, ExamRoom>()
                .ForMember(dest => dest.RoomId, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.RoomName));

            CreateMap<UpdateExamRoomModel, ExamRoom>()
                .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.RoomName));

            CreateMap<ExamSession, ExamSessionModel>()
                .ForMember(dest => dest.ExamSessionId, opt => opt.MapFrom(src => src.ExamSessionId))
                .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId))
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course!.CourseName))
                .ForMember(dest => dest.ExamDate, opt => opt.MapFrom(src => src.ExamDate))
                .ForMember(dest => dest.RoomId, opt => opt.MapFrom(src => src.RoomId))
                .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.Room!.RoomName))
                .ForMember(dest => dest.TeacherId, opt => opt.MapFrom(src => src.TeacherId))
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher!.FullName))
                .ForMember(dest => dest.TeacherEmail, opt => opt.MapFrom(src => src.Teacher!.Email))
                .ForMember(dest => dest.TeacherPhoneNumber, opt => opt.MapFrom(src => src.Teacher!.PhoneNumber))
                .ForMember(dest => dest.StudentsEnrolled, opt => opt.MapFrom(src => src.StudentsEnrolled))
                .ForMember(dest => dest.StaffId, opt => opt.MapFrom(src => src.StaffId))
                .ForMember(dest => dest.StaffName, opt => opt.MapFrom(src => src.Staff!.FullName))
                .ForMember(dest => dest.StaffEmail, opt => opt.MapFrom(src => src.Staff!.Email))
                .ForMember(dest => dest.StaffPhoneNumber, opt => opt.MapFrom(src => src.Staff!.PhoneNumber))
                .ForMember(dest => dest.ShiftId, opt => opt.MapFrom(src => src.ShiftId))
                .ForMember(dest => dest.ShiftName, opt => opt.MapFrom(src => src.Shift!.ShiftName))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.Shift!.StartTime))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.Shift!.EndTime))
                .ForMember(dest => dest.IsPassed, opt => opt.MapFrom(src => src.IsPassed))
                .ForMember(dest => dest.IsPaid, opt => opt.MapFrom(src => src.IsPaid));

            CreateMap<ExamShift, ExamShiftModel>()
                .ReverseMap();

            CreateMap<AddExamShiftModel, ExamShift>()
                .ForMember(dest => dest.ShiftId, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.ShiftName, opt => opt.MapFrom(src => src.ShiftName))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime));

            CreateMap<UpdateExamShiftModel, ExamShift>()
                .ForMember(dest => dest.ShiftName, opt => opt.MapFrom(src => src.ShiftName))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime));

            CreateMap<Major, MajorModel>()
                .ReverseMap();

            CreateMap<AddMajorModel, Major>()
                .ForMember(dest => dest.MajorId, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.MajorName, opt => opt.MapFrom(src => src.MajorName));

            CreateMap<UpdateMajorModel, Major>()
                .ForMember(dest => dest.MajorName, opt => opt.MapFrom(src => src.MajorName));

            CreateMap<Student, StudentModel>()
                .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.StudentId))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.HomeAddress, opt => opt.MapFrom(src => src.HomeAddress))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.ProfilePicture))
                .ForMember(dest => dest.MajorId, opt => opt.MapFrom(src => src.MajorId));

            CreateMap<Teacher, TeacherModel>()
                .ForMember(dest => dest.TeacherId, opt => opt.MapFrom(src => src.TeacherId))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.HomeAddress, opt => opt.MapFrom(src => src.HomeAddress))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.ProfilePicture))
                .ForMember(dest => dest.MajorId, opt => opt.MapFrom(src => src.MajorId));

            CreateMap<Staff, StaffModel>()
                .ForMember(dest => dest.StaffId, opt => opt.MapFrom(src => src.StaffId))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.HomeAddress, opt => opt.MapFrom(src => src.HomeAddress))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.ProfilePicture))
                .ForMember(dest => dest.StaffRole, opt => opt.MapFrom(src => src.StaffRole));



            // ACCESS MAPPING
            CreateMap<Student, UserInfo>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.StudentId))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.HomeAddress, opt => opt.MapFrom(src => src.HomeAddress))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.ProfilePicture))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => "Student"));

            CreateMap<Teacher, UserInfo>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.TeacherId))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.HomeAddress, opt => opt.MapFrom(src => src.HomeAddress))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.ProfilePicture))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => "Teacher"));

            CreateMap<Staff, UserInfo>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.StaffId))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.HomeAddress, opt => opt.MapFrom(src => src.HomeAddress))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.ProfilePicture))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.StaffRole));

            CreateMap<SignupModel, Student>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName));
        }
    }
}
