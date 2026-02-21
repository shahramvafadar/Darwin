# گزارش بررسی کامل مستندات Markdown (تمرکز: موبایل + بک‌لاگ)

این فایل یک جمع‌بندی اجرایی از مستندات فعلی ریپو است تا در گفتگوهای بعدی، رفع باگ‌ها و توسعه ویژگی‌ها با «منبع حقیقت = کد و مستندات داخل گیت» پیش برویم.

## 1) اسناد بررسی‌شده

- `README.md`
- `CONTRIBUTING.md`
- `BACKLOG.md`
- `DarwinMobile.md`
- `DarwinMobile.Guidelines.md`
- `DarwinTesting.md`
- `howto-identity-access.md`

## 2) تصویر کلان معماری مرتبط با موبایل

- راهکار روی .NET 10، C# 14، MAUI 10 و VS 2026 هدف‌گذاری شده است.
- قراردادهای API باید فقط از `Darwin.Contracts` مصرف شوند (Contracts-first).
- دو اپ موبایل وجود دارد:
  - `Darwin.Mobile.Consumer`
  - `Darwin.Mobile.Business`
- لایه مشترک `Darwin.Mobile.Shared` باید مسئول HTTP client، retry policy، token storage abstraction، scanner/location abstraction و سرویس‌های façade باشد.
- جریان QR باید session-based باشد و QR فقط یک `ScanSessionToken` کوتاه‌عمر و opaque حمل کند (بدون internal IDs).

## 3) وضعیت فعلی و اولویت‌ها (طبق بک‌لاگ)

### Completed (پایدار)
- هسته معماری Clean Architecture، مدل دامنه، امنیت پایه، EF + migration + seeding، و بخش‌های مهمی از پنل ادمین.

### In Progress (ACTIVE)
- WebApi برای identity + loyalty + discovery.
- `Darwin.Mobile.Shared` (HTTP/retry/storage/abstractions/DI).
- اپ Consumer (لاگین، QR، Discover، Rewards، Profile).
- اپ Business (لاگین، Scan → Process → Confirm flow).

### Planned Next
- فاز 2 و 3 موبایل برای map/detail/favorites/reviews/promotions/history و analytics/subscriptions/push.

## 4) نکات اجرایی حیاتی برای پروژه‌های موبایل

1. **Solution Filter درست**
   - برای کار موبایل فقط از `Darwin.MobileOnly.slnf` استفاده شود.

2. **DI الزامی**
   - هر اپ موبایل باید `AddDarwinMobileShared(ApiOptions)` را در composition root ثبت کند.
   - `Microsoft.Extensions.Http` باید در host اپ‌ها موجود باشد.

3. **قانون Contracts-first**
   - اپ‌ها و WebApi باید روی DTOهای `Darwin.Contracts` همگرا باشند.
   - هیچ نوع Domain/EF نباید به mobile/web قرارداد شود.

4. **جریان امنیتی QR**
   - Consumer: `PrepareScanSession`
   - Business: `ProcessScanSessionForBusiness`
   - سپس `ConfirmAccrual` یا `ConfirmRedemption`
   - خطاهای expiration/replay باید با UX واضح مدیریت شوند.

5. **راهنمای UX فازمحور**
   - آیتم‌های فاز 2/3 در ناوبری قابل مشاهده باشند اما در Phase 1 یا disabled باشند یا Coming Soon نشان دهند.
   - زبان UI فعلاً آلمانی (DE) است.

## 5) لیست کارهای باز مهم (از مستندات)

### Mobile / API
- تکمیل endpointهای توسعه‌محور فاز بعد (onboarding, reward config, push device registration, advanced discovery filters).
- تکمیل flowهای Phase 2/3 در Consumer و Business.

### Security / Platform
- enforce TOTP برای admin.
- magic-link login.
- تکمیل مستندسازی key rotation و multi-instance setup.
- اضافه‌شدن cloud-native storage برای Data Protection key ring.

### Admin/Web
- تکمیل SiteSettings (SMTP, analytics, WebAuthn origins, WhatsApp).
- تکمیل UI نقش/دسترسی و مدیریت کاربر + 2FA/WebAuthn.

## 6) تست و کیفیت (نکات قابل اتکا)

- برای تست جریان‌های چندمرحله‌ای loyalty، integration tests با WebApplicationFactory + SQLite-in-memory توصیه شده است.
- برای سازگاری قراردادها، contract serialization tests باید در CI اجرا شوند.
- برای Mobile.Shared، unit test روی API client/retry/token logic باید وجود داشته باشد.

## 7) نکات هماهنگی برای کارهای بعدی

- برای هر درخواست تغییر، قبل از پیشنهاد کد باید فایل‌های مرتبط در همان لحظه از ریپو بررسی شوند.
- در اصلاحات بعدی، تمرکز بر:
  - backward compatibility قراردادها
  - جلوگیری از API drift بین WebApi و اپ‌های موبایل
  - مدیریت خطای استاندارد (`ApiProblem`) در UI
  - رعایت کامل الگوهای فازمحور UX

---

این فایل فقط «جمع‌بندی قابل اقدام» از مستندات فعلی است و جایگزین منبع حقیقت (کد + قراردادهای فعلی ریپو) نیست.
