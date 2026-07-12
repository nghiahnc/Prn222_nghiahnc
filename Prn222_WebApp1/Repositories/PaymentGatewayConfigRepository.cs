using Domain;
using MVC.Data2;
using System.Collections.Generic;
using System.Linq;

namespace Repositories
{
    public class PaymentGatewayConfigRepository : IPaymentGatewayConfigRepository
    {
        private readonly DemoMVC2Context _context;

        public PaymentGatewayConfigRepository(DemoMVC2Context context)
        {
            _context = context;
        }

        public List<PaymentGatewayConfig> GetAll()
        {
            return _context.PaymentGatewayConfigs.ToList();
        }

        public PaymentGatewayConfig? GetById(int id)
        {
            return _context.PaymentGatewayConfigs.FirstOrDefault(p => p.Id == id);
        }

        public void Create(PaymentGatewayConfig config)
        {
            _context.PaymentGatewayConfigs.Add(config);
            _context.SaveChanges();
        }

        public void Update(PaymentGatewayConfig config)
        {
            _context.PaymentGatewayConfigs.Update(config);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var config = _context.PaymentGatewayConfigs.FirstOrDefault(p => p.Id == id);
            if (config != null)
            {
                _context.PaymentGatewayConfigs.Remove(config);
                _context.SaveChanges();
            }
        }
    }
}
