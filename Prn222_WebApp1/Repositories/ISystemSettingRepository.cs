using Domain;
using System.Collections.Generic;

namespace Repositories
{
    public interface ISystemSettingRepository
    {
        List<SystemSetting> GetAll();
        SystemSetting? GetById(int id);
        void Create(SystemSetting setting);
        void Update(SystemSetting setting);
        void Delete(int id);
    }
}
