using Domain;
using Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Services
{
    public class SystemSettingService : ISystemSettingService
    {
        private readonly ISystemSettingRepository _repo;

        public SystemSettingService(ISystemSettingRepository repo)
        {
            _repo = repo;
        }

        public List<SystemSetting> GetAllSettings()
        {
            return _repo.GetAll();
        }

        public SystemSetting? GetSettingById(int id)
        {
            return _repo.GetById(id);
        }

        public ServiceResult CreateSetting(SystemSetting setting)
        {
            if (string.IsNullOrWhiteSpace(setting.SettingKey)) return ServiceResult.Fail("Setting Key is required.");
            if (_repo.GetAll().Any(s => s.SettingKey.Equals(setting.SettingKey, System.StringComparison.OrdinalIgnoreCase)))
            {
                return ServiceResult.Fail($"A setting with the key '{setting.SettingKey}' already exists.");
            }

            _repo.Create(setting);
            return ServiceResult.Ok();
        }

        public ServiceResult UpdateSetting(SystemSetting setting)
        {
            if (string.IsNullOrWhiteSpace(setting.SettingKey)) return ServiceResult.Fail("Setting Key is required.");
            if (_repo.GetAll().Any(s => s.Id != setting.Id && s.SettingKey.Equals(setting.SettingKey, System.StringComparison.OrdinalIgnoreCase)))
            {
                return ServiceResult.Fail($"A setting with the key '{setting.SettingKey}' already exists.");
            }

            _repo.Update(setting);
            return ServiceResult.Ok();
        }

        public void DeleteSetting(int id)
        {
            _repo.Delete(id);
        }
    }
}
