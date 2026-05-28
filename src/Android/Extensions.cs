using System;
using System.Linq;
using Android.BillingClient.Api;

namespace Companova.Maui.InAppPurchase.Service
{
    internal static class Extensions
    {
        public static PurchaseError ToPurchaseError(this BillingResponseCode code)
        {
            PurchaseError error;
            switch (code)
            {
                case BillingResponseCode.BillingUnavailable:
                    error = PurchaseError.BillingUnavailable;
                    break;
                case BillingResponseCode.DeveloperError:
                    error = PurchaseError.DeveloperError;
                    break;
                case BillingResponseCode.Error:
                    error = PurchaseError.GeneralError;
                    break;
                case BillingResponseCode.FeatureNotSupported:
                    error = PurchaseError.GeneralError;
                    break;
                case BillingResponseCode.ItemAlreadyOwned:
                    error = PurchaseError.AlreadyOwned;
                    break;
                case BillingResponseCode.ItemNotOwned:
                    error = PurchaseError.NotOwned;
                    break;
                case BillingResponseCode.ItemUnavailable:
                    error = PurchaseError.ItemUnavailable;
                    break;
                case BillingResponseCode.ServiceDisconnected:
                    error = PurchaseError.ServiceDisconnected;
                    break;
                case BillingResponseCode.ServiceTimeout:
                    error = PurchaseError.NetworkConnectionFailed;
                    break;
                case BillingResponseCode.ServiceUnavailable:
                    error = PurchaseError.ServiceUnavailable;
                    break;
                case BillingResponseCode.UserCancelled:
                    error = PurchaseError.UserCancelled;
                    break;
                default:
                    error = PurchaseError.Unknown;
                    break;
            }

            return error;
        }

        public static InAppPurchaseResult ToInAppPurchase(this Purchase p)
        {
            return new InAppPurchaseResult
            {
                TransactionDateUtc = new DateTime(p.PurchaseTime),
                Id = p.OrderId,
                ProductId = p.Products.FirstOrDefault(),
                Acknowledged = p.IsAcknowledged,
                AutoRenewing = p.IsAutoRenewing,
                State = p.GetPurchaseState(),
                PurchaseToken = p.PurchaseToken
            };
        }

        private static PurchaseState GetPurchaseState(this Purchase transaction)
        {
            if (transaction?.PurchaseState == null)
                return PurchaseState.Unknown;

            switch (transaction.PurchaseState)
            {
                case Android.BillingClient.Api.PurchaseState.Unspecified:
                    return PurchaseState.Unknown;
                case Android.BillingClient.Api.PurchaseState.Pending:
                    return PurchaseState.PaymentPending;
                case Android.BillingClient.Api.PurchaseState.Purchased:
                    return PurchaseState.Purchased;
            }

            return PurchaseState.Unknown;
        }

        public static Product ToProduct(this ProductDetails p, string billingProductType)
        {
            string formattedPrice = string.Empty;
            string currencyCode = string.Empty;
            long microsPrice = 0L;
            string localizedIntroductoryPrice = string.Empty;
            long microsIntroductoryPrice = 0L;

            if (billingProductType == BillingClient.ProductType.Subs)
            {
                // For subscriptions, use the first subscription offer's first pricing phase
                var subOfferDetails = p.SubscriptionOfferDetails;
                var firstOffer = subOfferDetails?.Count > 0 ? subOfferDetails[0] : null;
                var pricingPhases = firstOffer?.PricingPhases?.PricingPhaseList;
                if (pricingPhases != null && pricingPhases.Count > 0)
                {
                    formattedPrice = pricingPhases[0].FormattedPrice ?? string.Empty;
                    currencyCode = pricingPhases[0].PriceCurrencyCode ?? string.Empty;
                    microsPrice = pricingPhases[0].PriceAmountMicros;

                    // If there is more than one pricing phase, the second one is the introductory price
                    if (pricingPhases.Count > 1)
                    {
                        localizedIntroductoryPrice = pricingPhases[1].FormattedPrice ?? string.Empty;
                        microsIntroductoryPrice = pricingPhases[1].PriceAmountMicros;
                    }
                }
            }
            else
            {
                // For one-time in-app products
                var offerDetails = p.OneTimePurchaseOfferDetails;
                if (offerDetails != null)
                {
                    formattedPrice = offerDetails.FormattedPrice ?? string.Empty;
                    currencyCode = offerDetails.PriceCurrencyCode ?? string.Empty;
                    microsPrice = offerDetails.PriceAmountMicros;
                }
            }

            return new Product
            {
                Name = p.Name ?? string.Empty,
                Description = p.Description ?? string.Empty,
                ProductId = p.ProductId ?? string.Empty,
                FormattedPrice = formattedPrice,
                CurrencyCode = currencyCode,
                MicrosPrice = microsPrice,
                LocalizedIntroductoryPrice = localizedIntroductoryPrice,
                MicrosIntroductoryPrice = microsIntroductoryPrice
            };
        }
    }
}
