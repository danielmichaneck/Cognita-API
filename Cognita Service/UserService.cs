using System;
using System.Linq;
using AutoMapper;
using Cognita_Domain.Contracts;
using Cognita_Infrastructure.Models.Dtos;
using Cognita_Service.Contracts;
using Cognita_Shared.Dtos.User;
using Cognita_Shared.Entities;
using Microsoft.VisualBasic;

namespace Cognita_Service;

public class UserService : IUserService
{
    private readonly IMapper _mapper;
    private readonly IUoW _uow;

    public UserService(IMapper mapper, IUoW uow) {
        _mapper = mapper;
        _uow = uow;
    }

    public async Task<UserDto> AddUserAsync(UserForRegistrationDto dto) {
        var user = _mapper.Map<User>(dto);
        await _uow.UserRepository.CreateUserAsync(user);
        return _mapper.Map<UserDto>(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync() {
        var users = await _uow.UserRepository.GetAllUsersAsync();
        var userDtos = new List<UserDto>();
        foreach (User user in users) {
            var userDto = _mapper.Map<UserDto>(user);
            userDto.CourseName = "user.Course.CourseName";
            userDtos.Add(userDto);
        }
        return userDtos.AsEnumerable();
    }

    public async Task<IEnumerable<UserDto>> GetUsersInCourseAsync(int courseId) {
        var users = await _uow.UserRepository.GetUsersInCourseAsync(courseId);
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto> GetSingleUserAsync(int userId) {
        var user = await _uow.UserRepository.GetSingleUserAsync(userId);
        var userDto = _mapper.Map<UserDto>(user);
        userDto.CourseName = "user.Course.CourseName;";
        return userDto;
    }

    public async Task<bool> UpdateUserAsync(int userId, UserForUpdateDto dto) {
        var user = await _uow.UserRepository.GetSingleUserAsync(userId, trackChanges: true);

        if (user is null) {
            return false;
        }

        _mapper.Map(dto, user);
        try {
            await _uow.CompleteAsync();
            return true;
        }
        catch (Exception ex) {
            Console.WriteLine(ex);
            return false;
        }
    }
}
