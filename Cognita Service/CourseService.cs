using System;
using AutoMapper;
using Cognita_Domain.Contracts;
using Cognita_Service.Contracts;
using Cognita_Shared.Dtos.Course;
using Cognita_Shared.Entities;

namespace Cognita_Service;

public class CourseService : ICourseService
{
    private readonly IMapper _mapper;
    private readonly IUoW _uow;

    public CourseService(IMapper mapper, IUoW uow)
    {
        _mapper = mapper;
        _uow = uow;
    }

    public async Task<CourseDto> CreateCourseAsync(CourseForCreationDto dto)
    {
        var course = _mapper.Map<Course>(dto);
        await _uow.CourseRepository.CreateCourseAsync(course);
        return _mapper.Map<CourseDto>(course);
    }

    public async Task<bool> EditCourseAsync(int id, CourseForUpdateDto dto)
    {
        var course = await _uow.CourseRepository.GetSingleCourseAsync(id, trackChanges: true);

        if (course is null)
        {
            return false;
        }

        _mapper.Map(dto, course);

        try
        {
            await _uow.CompleteAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    public async Task<IEnumerable<CourseDto>> GetCoursesAsync()
    {
        var courses = await _uow.CourseRepository.GetAllCoursesAsync();
        return _mapper.Map<IEnumerable<CourseDto>>(courses);
    }

    public async Task<CourseDto> GetSingleCourseAsync(int id)
    {
        var course = await _uow.CourseRepository.GetSingleCourseAsync(id);
        return _mapper.Map<CourseDto>(course);
    }
}
