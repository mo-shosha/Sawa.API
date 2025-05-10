using Microsoft.Extensions.Options;
using SAWA.core.DTO;
using SAWA.core.IServices;
using Stripe;
using Stripe.Checkout;
namespace SAWA.infrastructure.Services
{
    public class StripeService : IStripeService
    {
        private readonly StripeSettings _settings;

        public StripeService(IOptions<StripeSettings> options)
        {
            _settings = options.Value;
            StripeConfiguration.ApiKey = _settings.SecretKey;
        }

        public async Task<Session> CreateDonationSessionAsync(DonationRequestDto donation)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                CustomerEmail = donation.DonorEmail,
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "usd",
                            UnitAmount = donation.AmountInCents,
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Donation"
                            }
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = "https://localhost:3000/donation-success",
                CancelUrl = "https://localhost:3000/donation-cancelled"

            };

            var service = new SessionService();
            return await service.CreateAsync(options);
        }
    }
}
