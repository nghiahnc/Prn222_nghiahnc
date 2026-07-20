using Domain;
using System.Collections.Generic;

namespace Services
{
    public interface IPaymentGatewayConfigService
    {
        List<PaymentGatewayConfig> GetAllConfigs();
        PaymentGatewayConfig? GetConfigById(int id);
        ServiceResult CreateConfig(PaymentGatewayConfig config);
        ServiceResult UpdateConfig(PaymentGatewayConfig config);
        void DeleteConfig(int id);
    }
}
