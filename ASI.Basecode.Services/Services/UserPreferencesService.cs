using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Data.Repositories;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Services
{
    public class UserPreferencesService : IUserPreferencesService
    {
        private readonly IUserPreferencesRepository _repository;

        public UserPreferencesService(IUserPreferencesRepository repository)
        {
            _repository = repository;
        }

        public UserPreferences GetPreferencesByUserId(string userId)
        {
            return _repository.GetPreferencesByUserId(userId);
        }

        public void AddPreferences(UserPreferences preferences)
        {
            var existingPreferences = _repository.GetPreferencesByUserId(preferences.UserId);

            if (existingPreferences == null)
            {
                preferences.CreatedTime = DateTime.UtcNow;
                preferences.UpdatedTime = DateTime.UtcNow;

                _repository.AddPreferences(preferences);
            }
            else
            {
                throw new InvalidOperationException("Preferences for this user already exist.");
            }
        }

        public void UpdatePreferences(UserPreferences preferences)
        {
            var existingPreferences = _repository.GetPreferencesByUserId(preferences.UserId);

            if (existingPreferences != null)
            {
                existingPreferences.DefaultCategoryId = preferences.DefaultCategoryId;
                existingPreferences.DefaultStatusId = preferences.DefaultStatusId;
                existingPreferences.DefaultPriorityId = preferences.DefaultPriorityId;
                existingPreferences.UpdatedTime = DateTime.UtcNow;

                _repository.UpdatePreferences(existingPreferences);
            }
            else
            {
                throw new InvalidOperationException("Preferences for this user do not exist.");
            }
        }

        public void DeletePreferences(string userId)
        {
            var preferences = _repository.GetPreferencesByUserId(userId);

            if (preferences != null)
            {
                _repository.DeletePreferences(preferences);
            }
            else
            {
                throw new InvalidOperationException("Preferences for this user do not exist.");
            }
        }
    }
}
