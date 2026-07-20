using Domain;
using Repositories;
using System.Collections.Generic;
using System.Linq;

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

        public ServiceResult CreateConfig(PaymentGatewayConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.Name)) return ServiceResult.Fail("Provider Name is required.");
            if (_repo.GetAll().Any(c => c.Name.Equals(config.Name, System.StringComparison.OrdinalIgnoreCase)))
            {
                return ServiceResult.Fail($"A payment gateway with the name '{config.Name}' already exists.");
            }

            _repo.Create(config);
            return ServiceResult.Ok();
        }

        public ServiceResult UpdateConfig(PaymentGatewayConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.Name)) return ServiceResult.Fail("Provider Name is required.");
            if (_repo.GetAll().Any(c => c.Id != config.Id && c.Name.Equals(config.Name, System.StringComparison.OrdinalIgnoreCase)))
            {
                return ServiceResult.Fail($"A payment gateway with the name '{config.Name}' already exists.");
            }

            _repo.Update(config);
            return ServiceResult.Ok();
        }

        public void DeleteConfig(int id)
        {
            _repo.Delete(id);
        }
    }
}
