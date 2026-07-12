using Domain;
using Repositories;
using System.Collections.Generic;

namespace Services
{
    public class PaymentGatewayConfigService : IPaymentGatewayConfigService
    {
        private readonly IPaymentGatewayConfigRepository _repo;

        public PaymentGatewayConfigService(IPaymentGatewayConfigRepository repo)
        {
            _repo = repo;
        }

        public List<PaymentGatewayConfig> GetAllConfigs()
        {
            return _repo.GetAll();
        }

        public PaymentGatewayConfig? GetConfigById(int id)
        {
            return _repo.GetById(id);
        }

        public void CreateConfig(PaymentGatewayConfig config)
        {
            _repo.Create(config);
        }

        public void UpdateConfig(PaymentGatewayConfig config)
        {
            _repo.Update(config);
        }

        public void DeleteConfig(int id)
        {
            _repo.Delete(id);
        }
    }
}
