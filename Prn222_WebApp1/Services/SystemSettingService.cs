using Domain;
using Repositories;
using System.Collections.Generic;

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

        public void CreateSetting(SystemSetting setting)
        {
            _repo.Create(setting);
        }

        public void UpdateSetting(SystemSetting setting)
        {
            _repo.Update(setting);
        }

        public void DeleteSetting(int id)
        {
            _repo.Delete(id);
        }
    }
}
