using System;
using AutoMapper;
using Cognita_Domain.Contracts;
using Cognita_Service.Contracts;
using Cognita_Shared.Dtos.User;
using Cognita_Shared.Entities;
using Microsoft.VisualBasic;

namespace Cognita_Service;

public class UserService : IUserService
{
    private readonly IMapper _mapper;
    private readonly IUoW _uow;

    public UserService(IMapper mapper, IUoW uow)
    {
        _mapper = mapper;
        _uow = uow;
    }

    public async Task<UserDto> AddUserAsync(UserForCreationDto dto)
    {
        var user = _mapper.Map<User>(dto);
        await _uow.UserRepository.CreateUserAsync(user);
        return _mapper.Map<UserDto>(user);
    }

    public async Task<IEnumerable<UserDto>> GetParticipantsAsync(int courseId)
    {
        var users = await _uow.UserRepository.GetAllParticipantsAsync(courseId);
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto> GetSingleUserAsync(int userId)
    {
        var user = await _uow.UserRepository.GetSingleUserAsync(userId);
        return _mapper.Map<UserDto>(user);
    }

    public async Task<bool> UpdateUserAsync(int userId, UserForUpdateDto dto)
    {
        var user = await _uow.UserRepository.GetSingleUserAsync(userId, trackChanges: true);

        if (user is null)
        {
            return false;
        }

        _mapper.Map(dto, user);
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
}
