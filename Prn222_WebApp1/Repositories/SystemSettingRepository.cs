using Domain;
using MVC.Data2;
using System.Collections.Generic;
using System.Linq;

namespace Repositories
{
    public class SystemSettingRepository : ISystemSettingRepository
    {
        private readonly DemoMVC2Context _context;

        public SystemSettingRepository(DemoMVC2Context context)
        {
            _context = context;
        }

        public List<SystemSetting> GetAll()
        {
            return _context.SystemSettings.ToList();
        }

        public SystemSetting? GetById(int id)
        {
            return _context.SystemSettings.FirstOrDefault(s => s.Id == id);
        }

        public void Create(SystemSetting setting)
        {
            _context.SystemSettings.Add(setting);
            _context.SaveChanges();
        }

        public void Update(SystemSetting setting)
        {
            _context.SystemSettings.Update(setting);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var setting = _context.SystemSettings.FirstOrDefault(s => s.Id == id);
            if (setting != null)
            {
                _context.SystemSettings.Remove(setting);
                _context.SaveChanges();
            }
        }
    }
}
