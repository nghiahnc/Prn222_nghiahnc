using Domain;
using System.Collections.Generic;

namespace Services
{
    public interface ISystemSettingService
    {
        List<SystemSetting> GetAllSettings();
        SystemSetting? GetSettingById(int id);
        void CreateSetting(SystemSetting setting);
        void UpdateSetting(SystemSetting setting);
        void DeleteSetting(int id);
    }
}
