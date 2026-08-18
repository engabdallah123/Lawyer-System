# ⚖️ نظام إدارة مكاتب وشركات المحاماة (Legal Practice Management System)

<div align="center">

![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Blazor Web App](https://img.shields.io/badge/Blazor-Web%20App-512BD4?style=for-the-badge&logo=blazor&logoColor=white)
![MudBlazor](https://img.shields.io/badge/MudBlazor-Interactive%20Server-7E6FFF?style=for-the-badge&logo=mudblazor&logoColor=white)
![Clean Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture-blue?style=for-the-badge)
![CQRS & MediatR](https://img.shields.io/badge/Pattern-CQRS%20%2F%20MediatR-orange?style=for-the-badge)
![Entity Framework Core](https://img.shields.io/badge/EF%20Core-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/Database-SQL%20Server-CC292B?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)

<p align="center">
  <b>منظومة برمجية متكاملة وسحابية مصممة خصيصاً لإدارة مكاتب وشركات المحاماة والاستشارات القانونية وفق أفضل الممارسات والمعايير المهنية.</b>
  <br />
  <i>واجهات تفاعلية باللغة العربية بالكامل (RTL) تدعم الوضعين الفاتح والداكن (Light/Dark Mode) ومتجاوبة تماماً مع مختلف شاشات الهواتف والأجهزة الذكية.</i>
</p>

</div>

---

## 📑 فهرس المحتويات
- [نظرة عامة](#-نظرة-عامة)
- [المميزات الرئيسية والوحدات الوظيفية](#-المميزات-الرئيسية-والوحدات-الوظيفية)
- [الهندسة المعمارية (Clean Architecture)](#-الهندسة-المعمارية-clean-architecture)
- [التقنيات المستخدمة](#-التقنيات-المستخدمة)
- [متطلبات التشغيل](#-متطلبات-التشغيل)
- [طريقة التثبيت والتشغيل](#-طريقة-التثبيت-والتشغيل)
- [الحسابات الافتراضية المزروعة مسبقاً](#-الحسابات-الافتراضية-المزروعة-مسبقا)
- [الصلاحيات والأدوار (Security & RBAC)](#-الصلاحيات-والأدوار-security--rbac)
- [المساهمة والترخيص](#-المساهمة-والترخيص)

---

## 🌟 نظرة عامة

يقدم النظام حلاً شاملاً واحترافياً لأتمتة سير العمل داخل المكاتب القانونية، بدءاً من تسجيل الموكلين وإدارة القضايا والجلسات، مروراً بالتوكيلات والاستشارات القانونية، وانتهاءً بالفواتير وسندات القبض وإدارة الوثائق والمهام، مع تسجيل دقيق لكافة الحركات عبر سجل تدقيق شامل (Audit Trail).

---

## 🚀 المميزات الرئيسية والوحدات الوظيفية

### 👥 1. إدارة الموكلين (Clients Management)
- تسجيل الموكلين كأفراد أو شركات ومؤسسات.
- ربط السجلات التجارية للشركات وأرقام الهوية للأفراد مع منع التكرار.
- استعراض سجل القضايا والتوكيلات والفواتير المرتبطة بكل موكل.
- دعم الحذف المنطقي الآمن (Soft Delete).

### ⚖️ 2. إدارة ملفات القضايا والدعاوى (Cases & Litigation)
- فتح ملفات القضايا وتوليد أرقام داخلية وربطها برقم المحكمة والدائرة والقاضي.
- إسناد وتوزيع القضايا على المحامين (محامي رئيسي، مساعد، متدرب) مع توثيق التوجيهات.
- شجرة أطراف القضية (المدعي، المدعى عليه، الشهود، محامي الخصم).
- السجل الزمني التشغيلي للقضية (Case Timeline).

### 🏛️ 3. تقويم وجلسات المحاكم (Hearings & Court Calendar)
- تبويب خاص لجلسات اليوم العاجلة التي تتطلب الحضور.
- جدولة مواعيد الجلسات وربطها بالدوائر القضائية ونوع الجلسة.
- توثيق وقائع وقرارات الجلسات وتحديد مواعيد الجلسات القادمة بنقرة زر.
- إعادة جدولة وتأجيل الجلسات مع حفظ تاريخ الجلسة السابقة ومبررات التأجيل.

### 📜 4. التوكيلات والتفويضات (Power of Attorney)
- توثيق بيانات التوكيلات الرسمية، رقم التوكيل، جهة التوثيق، وتاريخ الانتهاء.
- شريط تنبيهات فوري بالتوكيلات التي قاربت على الانتهاء خلال 30 يوماً.
- إمكانية ربط التوكيل بموكل عام أو بقضية محددة.

### 💬 5. الاستشارات القانونية (Legal Consultations)
- حجز وإدارة مواعيد الاستشارات القانونية.
- تحديد أتعاب الاستشارة وتدوين ملخص الرأي القانوني والمشورة.
- إمكانية تحويل الاستشارة مباشرة إلى ملف قضية مفتوحة.

### 💰 6. الشؤون المالية والتحصيل (Finance & Billing)
- **لوحة مؤشرات مالية ذكية**: إجمالي العقود، المحصل، المستحقات المتأخرة، المصروفات، وصافي الأرباح.
- **الفواتير**: إصدار فواتير إلكترونية مع تفصيل بنود الخدمات، الخصومات، وضريبة القيمة المضافة.
- **سندات القبض (Payments)**: تسجيل التحصيلات الفورية وربطها بالفواتير وعقود الأتعاب.
- **سندات الصرف (Expenses)**: قيد المصروفات والتكاليف القضائية المرتبطة بالقضايا.
- **عقود الأتعاب (Fee Agreements)**: دعم عقود الأتعاب الثابتة، النسبة من المطالبة، والمحاسبة بالساعة.

### 📁 7. الأرشيف وإدارة المستندات (Document Repository & Versioning)
- تصنيف المستندات والمذكرات القانونية مع ربطها بالقضايا والموكلين.
- دعم الإصدارات المتعددة للمستند (Document Versioning) دون فقدان المسودات القديمة.

### ✅ 8. المهام والمتابعات (Legal Tasks & Workflows)
- تكليف ومتابعة مهام العمل بين فريق العمل مع درجات الأولوية (منخفضة، عادية، هامة، عاجلة).
- فلترة المهام المتأخرة والمهام المكتملة وتحديث الحالة فورياً بنقرة زر.

### 🔔 9. مركز التنبيهات والإشعارات (Notifications Center)
- تنبيهات النظام الفورية لجلسات المحاكم، انتهاء الوكالات، واستحقاقات المهام والفواتير.
- إمكانية تحديد الإشعارات كمقروءة بشكل فردي أو جماعي.

### 🛡️ 10. إدارة المستخدمين والصلاحيات الدقيقة (Users & Permissions)
- نظام مصادقة وصلاحيات متقدم مبني على **ASP.NET Core Identity** و **Permission-Based Authorization**.
- لوحة تحكم لإدارة الحسابات، وتعيين الأدوار (Roles)، ومنح أو سلب الصلاحيات الدقيقة لكل موظف.

---

## 🏛️ الهندسة المعمارية (Clean Architecture)

تم بناء المشروع باتباع أحدث مبادئ **Clean Architecture** ونمط **CQRS** (فصل أوامر التعديل عن استعلامات القراءة):

```text
Lawyer_System.sln
│
├── Shared.Domain           # الكيانات والـ Interfaces الأساسية المشتركة
├── Shared.Application      # عقود CQRS والـ Behaviors والـ Pipeline
├── Shared.Infrastructure   # الخدمات العامة (التخزين، التخزين المؤقت، البريد)
│
├── App.Domain              # الكيانات وقواعد العمل الخاصة بالنظام القانوني
├── App.Application         # الأوامر والاستعلامات (MediatR Commands & Queries)
├── App.Infrastructure      # EF Core + SQL Server + Identity + Migrations + Audit
│
└── App.Web                 # واجهة المستخدم Blazor Web App (Interactive Server) + MudBlazor
```

---

## 💻 التقنيات المستخدمة

| المجال | التقنية |
|---|---|
| **المنصة الأساسية** | .NET 10 (C# 13) |
| **واجهة المستخدم** | Blazor Web App (Interactive Server Mode) |
| **مكتبة المكونات والتصميم** | MudBlazor (مع دعم كامل للـ RTL والثيمات) |
| **الوصول لقواعد البيانات** | Entity Framework Core 10 + Dapper (High-Performance Queries) |
| **قاعدة البيانات** | Microsoft SQL Server |
| **نمط التطبيق و CQRS** | MediatR + Pipeline Behaviors |
| **الهوية والأمان** | ASP.NET Core Identity + Custom Permission-Based Authorization |
| **التحقق من البيانات** | FluentValidation |
| **سجل التدقيق** | Automatic Audit Logging via EF Core SaveChanges |
| **الحذف الآمن** | Global Query Filters for Soft Delete |

---

## ⚙️ متطلبات التشغيل

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) أو أحدث.
- [Microsoft SQL Server](https://www.microsoft.com/sql-server/) (أو LocalDB / Docker Container).
- بيئة تطوير متوافقة: Visual Studio 2026 / Visual Studio Code / JetBrains Rider.

---

## 🚀 طريقة التثبيت والتشغيل

### 1. استنساخ المستودع (Clone Repository)
```bash
git clone https://github.com/engabdallah123/Lawyer-System.git
cd Lawyer-System
```

### 2. إعداد نص الاتصال (Connection String)
قم بتعديل نص الاتصال بقاعدة البيانات في ملف `App.Web/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=.;Initial Catalog=Lawyer;Integrated Security=True;Trust Server Certificate=True;"
  }
}
```

### 3. تطبيق ترحيلات قاعدة البيانات (Database Migrations)
```bash
dotnet ef database update --project App.Infrastructure --startup-project App.Web
```

### 4. تشغيل التطبيق (Run Application)
```bash
dotnet run --project App.Web
```
افتح المتصفح وتوجه إلى: `https://localhost:5001` أو `http://localhost:5000`.

---

## 🔑 الحسابات الافتراضية المزروعة مسبقاً

يقوم النظام تلقائياً عند التشغيل الأول بزراعة الحسابات التالية لتسهيل التجربة والاختبار:

| الحساب | البريد الإلكتروني | كلمة المرور | الدور الوظيفي | الصلاحيات |
|---|---|---|---|---|
| **مدير النظام العام** | `admin@lawyer.com` | `Admin@123456` | `Administrator` | كامل صلاحيات النظام |
| **محامي ومستشار** | `lawyer@lawyer.com` | `Lawyer@123456` | `Lawyer` | إدارة القضايا، الجلسات، التوكيلات، المهام |
| **سكرتارية واستقبال** | `staff@lawyer.com` | `Staff@123456` | `Staff` | تسجيل الموكلين، المواعيد، وجدولة الجلسات |

---

## 🎨 تجربة المستخدم (UI / UX)

- 🌓 **دعم الثيم الفاتح والداكن (Light / Dark Mode)**: تم اختيار درجات ألوان متناسقة ومريحة للعين مع إمكانية التبديل الفوري بنقرة واحدة.
- 📱 **متجاوب بالكامل (Mobile Responsive)**: تتكيف الجداول تلقائياً إلى بطاقات تفاعلية أنيقة على الشاشات الصغيرة لتسهيل استخدام المحامي للنظام أثناء وجوده في قاعات المحاكم.
- 🌐 **دعم كامل للغة العربية (RTL First)**: نصوص وواجهات موجهة للمستخدم العربي مع معالجة احترافية للأرقام والتواريخ.

---

## 📄 الترخيص (License)

هذا المشروع مرخص بموجب رخصة [MIT License](LICENSE).
