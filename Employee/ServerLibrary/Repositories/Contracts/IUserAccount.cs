﻿using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using BaseLibrary.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary.Repositories.Contracts
{
    public interface IUserAccount
    {
        Task<GeneralResponse> CreateAsync(Register user);
        Task<LoginResponse> SignInAsync(Login user);
        Task<LoginResponse> RefreshTokenAsync(RefreshToken token);

        Task<List<MangeUser>> GetUsers();
        Task<GeneralResponse> UpdateUser(MangeUser user);
        Task<List<SystemRole>> GetRoles();
        Task<GeneralResponse> DeleteUser(int id);
        Task<string> GetUserImage(int id);

        Task<bool>UpdateProfile(UserProfile profile);
    }
}
