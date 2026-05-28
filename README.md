# MAUI In-App-Purchase Service

MAUI In-App-Purchase Plugin/Service for iOS and Android Mobile Applications.

Converted from [Companova.Xamarin.InAppPurchase.Service](https://github.com/companova/XamarinInAppPurchaseService) to MAUI targeting `net10.0`, `net10.0-android`, and `net10.0-ios`.

Supports Android In-App-Purchases using [Android Billing Client Library 8.3.0.2](https://developer.android.com/google/play/billing/release-notes).

### Setup:
<a href="https://www.nuget.org/packages/Companova.Maui.InAppPurchase.Service/">
  <img alt="Nuget" src="https://img.shields.io/nuget/v/Companova.Maui.InAppPurchase.Service">
</a>

**Available on NuGet:** [Companova.Maui.InAppPurchase.Service](https://www.nuget.org/packages/Companova.Maui.InAppPurchase.Service/)

### Key Changes from Xamarin Version

- Targets `net10.0;net10.0-android;net10.0-ios` instead of `netstandard2.0;Xamarin.iOS10;MonoAndroid13.0`
- Uses `Microsoft.NET.Sdk` instead of `MSBuild.Sdk.Extras`
- Updated namespace from `Companova.Xamarin.InAppPurchase.Service` to `Companova.Maui.InAppPurchase.Service`
- Android Billing Client upgraded from 5.2.0 to 8.3.0.2
  - Replaced deprecated `SkuDetails` API with `ProductDetails`
  - Replaced `QuerySkuDetailsAsync` with `QueryProductDetailsAsync`
  - Replaced `BillingClient.SkuType` with `BillingClient.ProductType`
  - Updated `EnablePendingPurchases()` to use `PendingPurchasesParams`
  - Updated purchase flow to use `BillingFlowParams.ProductDetailsParams`

### Android Usage

```csharp
// In your Activity OnCreate
var inAppPurchaseService = CrossInAppPurchaseService.Current;
inAppPurchaseService.SetActivity(this);
await inAppPurchaseService.StartAsync();

// Load products
var products = await inAppPurchaseService.LoadProductsAsync(
    new[] { "product_id_1", "product_id_2" },
    ProductType.Consumable);

// Purchase
var result = await inAppPurchaseService.PurchaseAsync("product_id_1");

// Finalize (acknowledge or consume)
await inAppPurchaseService.FinalizePurchaseAsync(result.PurchaseToken, ProductType.Consumable);

// Restore
var restoredPurchases = await inAppPurchaseService.RestoreAsync(ProductType.Consumable);

// In your Activity OnDestroy
await inAppPurchaseService.StopAsync();
```

### iOS Usage

```csharp
// In AppDelegate FinishedLaunching
var inAppPurchaseService = CrossInAppPurchaseService.Current;
await inAppPurchaseService.StartAsync();

// Optionally handle out-of-app purchases
InAppPurchaseService.OnPurchasedOutOfApp = (result) => { /* handle */ };

// Load products
var products = await inAppPurchaseService.LoadProductsAsync(
    new[] { "product_id_1", "product_id_2" },
    ProductType.Consumable);

// Purchase
var result = await inAppPurchaseService.PurchaseAsync("product_id_1");

// Restore
var restoredPurchases = await inAppPurchaseService.RestoreAsync(ProductType.Consumable);

// In AppDelegate WillTerminate
await inAppPurchaseService.StopAsync();
```

### Credits
This project was put together with a lot of help and reuse of code from these repos:
- https://github.com/companova/XamarinInAppPurchaseService
- https://github.com/jamesmontemagno/InAppBillingPlugin
- https://github.com/companova/MauiCommonAndroidServices