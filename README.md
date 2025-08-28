# Inquiries – Web API (.NET) + Angular UI

מטלת בית: מערכת ניהול **פניות** עם Web API ב‑.NET, פעולות CRUD, Stored Procedure לדוח חודשי, בדיקות יחידה, ו‑UI באנגולר 19 עם Angular Material.

---

## תוכן עניינים
- [תקציר](#תקציר)
- [ארכיטקטורה בקצרה](#ארכיטקטורה-בקצרה)
- [דרישות מוקדמות](#דרישות-מוקדמות)
- [התקנה והרצה — Backend (.NET)](#התקנה-והרצה--backend-net)
- [התקנה והרצה — Frontend (Angular 19)](#התקנה-והרצה--frontend-angular-19)
- [קונפיגורציה](#קונפיגורציה)
- [Endpoints עיקריים](#endpoints-עיקריים)
- [Stored Procedure — דוח פניות חודשי](#stored-procedure--דוח-פניות-חודשי)
- [שיפורי ביצועים מוצעים](#שיפורי-ביצועים-מוצעים)
- [בדיקות יחידה](#בדיקות-יחידה)
- [אבטחה](#אבטחה)
- [טיפול בשגיאות ולוגינג](#טיפול-בשגיאות-ולוגינג)
- [CORS](#cors)
- [מבנה הפרויקט](#מבנה-הפרויקט)
- [תקלות נפוצות (FAQ)](#תקלות-נפוצות-faq)

---

## תקציר
- **Backend**: ASP.NET Core (TargetFramework: `net8.0`) עם **EF Core** ו‑SQL Server **LocalDB**.  
  כולל CRUD לפניות, ניהול מחלקות, ו‑endpoint לדוח חודשי על בסיס Stored Procedure.
- **DB**: יצירת המסד וה‑migrations מתבצעת אוטומטית בהרצה (`Database.MigrateAsync`).  
  נזרעים **מחלקות** בסיסיות, ואפשר לזרוע **נתוני דמו** לפניות הכנסה מרובות חודשים (אופציונלי).
- **Frontend**: Angular 19 + Angular Material, טופס יצירת פנייה + מסך דוח חודשי.
- **בדיקות**: xUnit + Moq + FluentAssertions עבור שכבת השירותים והבקרי‑דוחות.
- **תיעוד**: Swagger UI זמין בזמן פיתוח.

---

## ארכיטקטורה בקצרה
```
Inquiries (Solution Root)
├─ src/                   # קוד ה-API
│  ├─ Controllers/        # Inquiries, Reports, Departments
│  ├─ Application/        # DTOs + Services Interfaces/Impl
│  ├─ Domain/             # ישויות דומיין
│  ├─ Infrastructure/
│  │  ├─ Ef/              # DbContext, מיפויים, Repositories EF
│  │  ├─ Repositories/    # EF/Interfaces
│  │  ├─ Scripts/         # GetMonthlyInquiryReport.sql
│  │  └─ Setup/           # DatabaseBootstrapper (SP + Seeding)
│  └─ Program.cs          # הרכבת היישום (ללא T-SQL קשיח)
├─ tests/                 # בדיקות יחידה
└─ Inquiries.sln
```

---

## דרישות מוקדמות
- **.NET SDK 8+**
- **SQL Server LocalDB** (מגיע עם Visual Studio / אפשר התקנה נפרדת)
- **Node.js 20+** ו‑**npm**
- (אופציונלי) **Git**

---

## התקנה והרצה — Backend (.NET)

1. שכפול והתקנת תלויות:
   ```bash
   git clone <repo-url> inquiries-api
   cd inquiries-api
   dotnet build
   ```

2. קובץ קונפיגורציה לפיתוח: `src/appsettings.Development.json` (דוגמה):
   ```json
   {
     "Logging": {
       "LogLevel": {
         "Default": "Information",
         "Microsoft.AspNetCore": "Warning"
       }
     },
     "ConnectionStrings": {
       "Default": "Server=(localdb)\\MSSQLLocalDB;Database=InquiriesDb;Trusted_Connection=True;TrustServerCertificate=True"
     },
     "Reporting": {
       "Backend": "StoredProcedure"
     },
     "DemoData": {
       "SeedOnStartup": true,
       "SampleSize": 60
     },
     "AllowedHosts": "*"
   }
   ```

3. הרצה (מיישם Migrations, יוצר SP, וזורע נתונים במידת הצורך):
   ```bash
   dotnet run --project .\src\Inquiries.Api.csproj
   ```
   כברירת מחדל ה‑API מאזין ב‑`http://localhost:5005` (ראו `launchSettings.json`).

4. Swagger UI (פיתוח): `http://localhost:5005/swagger`

> **הערה**: אם LocalDB כבוי — הפעילו:
> ```powershell
> sqllocaldb start MSSQLLocalDB
> ```

---

## התקנה והרצה — Frontend (Angular 19)

1. התקנת תלויות והפעלה:
   ```bash
   cd inquiries-ui
   npm i
   npm start        # ng serve --proxy-config proxy.conf.json
   ```

2. `src/proxy.conf.json` (נדרש לפיתוח, מנתב `/api` ל‑API המקומי):
   ```json
   {
     "/api": {
       "target": "http://localhost:5005",
       "secure": false,
       "changeOrigin": false,
       "logLevel": "debug"
     }
   }
   ```

3. `src/environments/environment.ts`:
   ```ts
   export const environment = {
     production: false,
     apiBaseUrl: '/api'
   };
   ```

4. ב‑root providers (למשל `app.config.ts`):
   ```ts
   import { provideHttpClient, withInterceptorsFromDi, withFetch } from '@angular/common/http';
   // ...
   provideHttpClient(withInterceptorsFromDi(), withFetch());
   ```

האפליקציה תרוץ ב‑`http://localhost:4200/`.

---

## קונפיגורציה
- **ConnectionStrings:Default** — שרת/מסד היעד.
- **Reporting:Backend** — כרגע `StoredProcedure` (ניתן להחליף למימוש Dapper/EF בעתיד).
- **DemoData:SeedOnStartup** — הדלקת זריעת נתוני דמו (false בפרודקשן).
- **DemoData:SampleSize** — כמות רשומות דמו (מפוזרות אחורה בזמן 12 חודשים).

---

## Endpoints עיקריים

### Inquiries
- `GET /api/inquiries` — כל הפניות
- `GET /api/inquiries/{id}` — פנייה בודדת
- `POST /api/inquiries` — יצירה (Body: `CreateInquiryDto`)
- `PUT /api/inquiries/{id}` — עדכון
- `DELETE /api/inquiries/{id}` — מחיקה

### Departments
- `GET /api/departments` — רשימת מחלקות (נזרעות: "כללי", "תפעול", "מערכות מידע")

### Reports
- `GET /api/reports/monthly?year=YYYY&month=MM` — דוח חודשי לכל מחלקה
  - החזרה: `MonthlyReportItemDto[]` עם:
    - `DepartmentId`, `DepartmentName`
    - `CurrentMonthCount`, `PrevMonthCount`, `SameMonthLastYearCount`

---

## Stored Procedure — דוח פניות חודשי
קוד ה‑SP נמצא בקובץ:
```
src/Infrastructure/Scripts/GetMonthlyInquiryReport.sql
```
ויוּצר אוטומטית בעליית היישום (דרך `DatabaseBootstrapper`).

**לוגיקה**:
- קלט: `@Year INT`, `@Month INT`
- מחשב חלונות תאריכים:
  - חודש נוכחי, החודש הקודם, אותו חודש בשנה שעברה
- מחשב סכום פניות לכל מחלקה בשלושת החלונות
- מחזיר רשומה לכל מחלקה, גם אם אין לה פניות (באמצעות `LEFT JOIN`)

**ולידציה**:
- בדיקת טווחים סבירים ל‑Year/Month, ו‑`THROW` במקרה שגיאה.

---

## שיפורי ביצועים מוצעים
1. **אינדקסים**:  
   - על `Inquiries.CreatedAtUtc` (רצוי `INCLUDE (Id)` או אינדקס מכסה לפי הצורך).
   - על טבלת הקישור `InquiryDepartments(DepartmentId, InquiryId)`.
2. **SARGability**: טווחי תאריכים בטופס `>=` ו‑`<` לשימוש באינדקס זמן.
3. **סינון ב‑JOIN**: סינון טווח בזמן על `Inquiries` לפני האגרגציה מפחית סריקה.
4. **סטטיסטיקות מעודכנות** ו‑**`NOCOUNT ON`** בתוך ה‑SP.
5. **Caching אפליקטיבי** לדוחות חודשיים (תלוי דרישות רענון).
6. **Partitioning** (אם הטבלה תגדל מאוד) לפי תאריך.

---

## בדיקות יחידה
- פרויקט: `tests/Inquiries.Tests`
- כלים: **xUnit**, **Moq**, **FluentAssertions**
- דוגמאות נבדקות:
  - `InquiryService.CreateAsync` — ולידציה של מחלקות קיימות והחזרת מזהה חדש
  - `InquiryService.GetAllAsync` — מיפוי והחזרת רשימה
  - `ReportsController.GetMonthly` — החזרת DTO תקין לפי פרמטרים
- הרצה:
  ```bash
  dotnet test
  ```

---

## אבטחה
- **CORS**: פתוח ל‑`http://localhost:4200` (ול‑`5173` לפיתוח Vite). בפרודקשן לסגור לדומיינים מורשים בלבד.
- **קלטים**: ולידציה ב‑DTOs (Email, חובה, וכו').
- **סודות**: לא לשמור מפתחות/סיסמאות בקוד; להשתמש ב‑User Secrets/Env Vars.
- **Swagger**: להשאיר רק בסביבות פיתוח או לאבטח מאחורי Auth.

> **הערה**: אין דרישת Auth למטלת הבית; בפרודקשן מומלץ JWT/OIDC, Rate Limiting, ותיעוד ביקורות.

---

## טיפול בשגיאות ולוגינג
- **ExceptionHandlingMiddleware**: יירוט חריגות והחזרת תגובת Problem Details עקבית ללקוח.
- **Logging**: ברירת המחדל של ASP.NET Core + סינון רמות לוג ב‑`appsettings.*.json`.
- **EF Core**: שגיאות DB נלכדות וממופות לשגיאת API ברורה.

---

## CORS
מוגדר בזמן Startup:  
`WithOrigins("http://localhost:4200", "http://localhost:5173").AllowAnyHeader().AllowAnyMethod()`

---

## מבנה הפרויקט
```
src/
 ├─ Controllers/
 ├─ Application/
 │   ├─ DTOs/
 │   └─ Services/
 ├─ Domain/
 ├─ Infrastructure/
 │   ├─ Ef/
 │   ├─ Repositories/
 │   ├─ Scripts/
 │   │   └─ GetMonthlyInquiryReport.sql
 │   └─ Setup/
 │       ├─ DatabaseBootstrapper.cs
 │       ├─ StoredProceduresInstaller.cs
 │       └─ DemoDataSeeder.cs      # אופציונלי (נשלט ב-Config)
 └─ Program.cs
tests/
 └─ Inquiries.Tests/
```

---

## תקלות נפוצות (FAQ)

**404 מה‑Angular ל‑API**  
- ודאו ש‑`npm start` משתמש ב‑`proxy.conf.json` כפי שמוגדר למעלה.
- ודאו שה‑API רץ ב‑`http://localhost:5005` (או עדכנו את ה‑proxy).

**אזהרת SSR (`withFetch`)**  
- הוסיפו `withFetch()` ל‑`provideHttpClient` (ראו קטע קוד ב‑Frontend).

**LocalDB לא רץ / אין Pipe**  
```powershell
sqllocaldb start MSSQLLocalDB
sqllocaldb info MSSQLLocalDB   
```

**בדיקת נתונים ב‑DB דרך sqlcmd**  
```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -d InquiriesDb -Q "SELECT TOP 10 * FROM dbo.Inquiries"
```

---

בהצלחה!
