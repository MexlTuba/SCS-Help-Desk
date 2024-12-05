using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IUserPreferencesRepository
    {
        UserPreferences GetPreferencesByUserId(string userId);
        void AddPreferences(UserPreferences preferences);
        void UpdatePreferences(UserPreferences preferences);
        void DeletePreferences(UserPreferences preferences);
    }
}
