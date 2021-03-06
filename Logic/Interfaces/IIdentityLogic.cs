﻿using Models.Enums;

namespace Logic.Interfaces
{
    public interface IIdentityLogic
    {        
        void TryLogin(string username, string password, out UserRoleEnum userRoleEnum, out bool result);

        void TryLogout(string username, string password, out bool resul);

        bool IsAuthenticated(string username, string password);
    }
}