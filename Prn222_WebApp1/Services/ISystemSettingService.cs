using Domain;
using System.Collections.Generic;

namespace Services
{
    public interface ISystemSettingService
    {
        List<SystemSetting> GetAllSettings();
        SystemSetting? GetSettingById(int id);
        ServiceResult CreateSetting(SystemSetting setting);
        ServiceResult UpdateSetting(SystemSetting setting);
        void DeleteSetting(int id);
    }
}
