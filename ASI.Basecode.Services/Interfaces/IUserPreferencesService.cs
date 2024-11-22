using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IUserPreferencesService
    {
        UserPreferences GetPreferencesByUserId(string userId);
        void AddPreferences(UserPreferences preferences);
        void UpdatePreferences(UserPreferences preferences);
        void DeletePreferences(string userId);
    }
}
