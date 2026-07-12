using Domain;
using System.Collections.Generic;

namespace Repositories
{
    public interface IPaymentGatewayConfigRepository
    {
        List<PaymentGatewayConfig> GetAll();
        PaymentGatewayConfig? GetById(int id);
        void Create(PaymentGatewayConfig config);
        void Update(PaymentGatewayConfig config);
        void Delete(int id);
    }
}
