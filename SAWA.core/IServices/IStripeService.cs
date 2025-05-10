using System.Collections.Generic;
using System.Threading.Tasks;
using SAWA.core.DTO;
using Stripe.Checkout;

namespace SAWA.core.IServices
{
    public interface IStripeService
    {
        Task<Session> CreateDonationSessionAsync(DonationRequestDto donation);
    }
}
