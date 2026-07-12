using Domain;
using System.Collections.Generic;

namespace Services
{
    public interface IPaymentGatewayConfigService
    {
        List<PaymentGatewayConfig> GetAllConfigs();
        PaymentGatewayConfig? GetConfigById(int id);
        void CreateConfig(PaymentGatewayConfig config);
        void UpdateConfig(PaymentGatewayConfig config);
        void DeleteConfig(int id);
    }
}
