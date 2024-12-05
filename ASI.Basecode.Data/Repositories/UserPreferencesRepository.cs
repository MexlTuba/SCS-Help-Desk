using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;

namespace ASI.Basecode.Data.Repositories
{
    public class UserPreferencesRepository : BaseRepository, IUserPreferencesRepository
    {
        public UserPreferencesRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public UserPreferences GetPreferencesByUserId(string userId)
        {
            return this.GetDbSet<UserPreferences>().FirstOrDefault(up => up.UserId == userId);
        }

        public void AddPreferences(UserPreferences preferences)
        {
            this.GetDbSet<UserPreferences>().Add(preferences);
            UnitOfWork.SaveChanges();
        }

        public void UpdatePreferences(UserPreferences preferences)
        {
            var existingPreferences = this.GetDbSet<UserPreferences>().FirstOrDefault(up => up.UserId == preferences.UserId);

            if (existingPreferences != null)
            {
                existingPreferences.DefaultCategoryId = preferences.DefaultCategoryId;
                existingPreferences.DefaultStatusId = preferences.DefaultStatusId;
                existingPreferences.DefaultPriorityId = preferences.DefaultPriorityId;
                existingPreferences.UpdatedTime = DateTime.UtcNow;

                this.GetDbSet<UserPreferences>().Update(existingPreferences);
                UnitOfWork.SaveChanges();
            }
            else
            {
                // Add new preferences if none exist
                AddPreferences(preferences);
            }
        }

        public void DeletePreferences(UserPreferences preferences)
        {
            this.GetDbSet<UserPreferences>().Remove(preferences);
            UnitOfWork.SaveChanges();
        }
    }
}
