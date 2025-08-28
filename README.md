# Inquiries API (.NET) — README

מסמך זה מתאר את פרויקט ה־**Inquiries API** שנבנה ב־.NET, כולל תיאור התכולה, שיטת הבנייה והרכיבים, יתרונות/חסרונות, אבטחה, טיפול בשגיאות, מנגנוני קישור (DB), CORS, והוראות התקנה/הרצה.  
> **שימו לב:** מסמך זה מכסה את צד ה־API בלבד (ללא Frontend).

---

## תכולת הפרויקט
- **CRUD לפניות** (`Inquiries`) עם השדות:
  - `Name`, `Phone`, `Email`, `DepartmentIds[]`, `Description`
- **מחלקות (Departments)**: טעינת 3 מחלקות ברירת־מחדל אם הטבלה ריקה.
- **דוח חודשי**: נקודת קצה שמחזירה את תוצאת ה־Stored Procedure `dbo.GetMonthlyInquiryReport` עם השוואה לחודש קודם ולאותו חודש בשנה שעברה.
- **בדיקות יחידה בסיסיות** לפרקי לוגיקה (Services).
- **Swagger/OpenAPI** לתיעוד ונסיון אינטראקטיבי.
- **CORS** פתוח ל־`http://localhost:4200` ו־`http://localhost:5173` (ניתן לשינוי).

---

## ארכיטקטורה ורכיבים
- **ASP.NET Core 8** — פרויקט Web API (`net8.0`).
- **Entity Framework Core 9** — ORM לניהול סכימה ו־Migrations, `DbContext` ו־Repositories.
- **SQL Server LocalDB** — בסיס נתונים מקומי קליל הבנוי אוטומטית בהרצה.
- **Stored Procedure** ב־T‑SQL: נמצא בקובץ `Infrastructure/Scripts/GetMonthlyInquiryReport.sql` ומותקן ב־startup.
- **Dapper + Microsoft.Data.SqlClient** — קריאה יעילה ל־Stored Procedure לדוח.
- **Middleware** לריכוז טיפול בשגיאות והחזרת תשובת בעיה תקנית (Problem Details).

**מבנה תיקיות רלוונטי (מקוצר):**
```
src/
  Controllers/
    InquiriesController.cs
    DepartmentsController.cs
    ReportsController.cs
  Application/
    DTOs/ (Inquiries, Reports)
    Interfaces/ (IInquiryService, IReportService, ...)
    Services/ (InquiryService, ReportServiceDb, ...)
  Infrastructure/
    Ef/ (AppDbContext, ישויות EF)
    Repositories/ (EfInquiryRepository, EfDepartmentRepository, ...)
    Scripts/
      GetMonthlyInquiryReport.sql   <-- ה־SP נשמר כאן
    Setup/
      DatabaseBootstrapper.cs       <-- מתקין SP ומזריע נתוני דמו (אופציונלי)
  Middleware/
    ExceptionHandlingMiddleware.cs
```

---

##  החלטות טכניות — יתרונות/חסרונות
**EF Core** (ל־CRUD ולמיגרציות):
-  פרודוקטיביות, מעקב שינויים, Migrations, LINQ, ולידציה קלה.
-  overhead קל בביצועים מול קריאות ידניות.

**Dapper/SP לדוחות:**
-  מהיר ויעיל לסיכומים/אגרגציות; ניצול אינדקסים; לוגיקת חישוב בצד ה־DB.
-  דורש ניהול T‑SQL בנפרד ובקרת גרסאות ל־SP.

**LocalDB**:
-  אין דרישה להתקנה כבדה; “עולה” אוטומטית אצל המריץ.
-  מיועד לפיתוח; בפרודקשן יש לעבור ל־SQL Server מלא/ענני.

---

## אבטחה (היי־לבל)
- **HTTPS**: מוגדר `UseHttpsRedirection()`.
- **ולידציה**: DTOs משתמשים ב־Data Annotations; Controllers מחזירים 400 בעת קלט שגוי.
- **SQL Injection**: כל הקריאות ל־DB פרמטריות (EF Core/Dapper); ה־SP לא מרכיב SQL דינמי מטקסט.
- **סודות**: מחרוזת החיבור ב־`appsettings.Development.json`/`User Secrets`; לפרודקשן — `Environment Variables`/Vault.
- **CORS**: מאפשר מקורות ידועים בלבד (ניתן להקשיח).

---

# טיפול בשגיאות
- **ExceptionHandlingMiddleware** מרכז חריגות ומחזיר `application/problem+json` עם `traceId` וסטטוס מתאים (400/404/500).
- לוגים נכתבים דרך `ILogger` ו־`Logging` של ASP.NET Core.

---

## מנגנוני קישור (DB)
- **Connection String** (ברירת מחדל):
  ```json
  "ConnectionStrings": {
    "Default": "Server=(localdb)\\MSSQLLocalDB;Database=InquiriesDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
  ```
- **Migrations**: מורצות אוטומטית ב־startup (`db.Database.MigrateAsync()`).
- **התקנת ה־SP**: בעת עליית היישום, קובץ `GetMonthlyInquiryReport.sql` נטען ומבוצע ב־`CREATE OR ALTER` כך שה־SP תמיד קיים/מעודכן.
- **זריעת נתונים**:
  - Departments נטענים כברירת־מחדל אם הטבלה ריקה.
  - (אופציונלי) זריעת **Demo Data** לפניות היסטוריות (חודש קודם/שנה שעברה) לצורך הדגמת הדוח — ניתן להפעיל/לכבות לפי `appsettings` או `IHostEnvironment`.

---

## CORS
כברירת־מחדל מופעלת מדיניות שמאפשרת בקשות מ־
`http://localhost:4200` ו־`http://localhost:5173`, עם `AllowAnyHeader` ו־`AllowAnyMethod`.  
ניתן להוסיף/להסיר מקורות בקוד ההגדרות (`AddCors`).

---

## בדיקות (Unit Tests)
- בדיקות לוגיקה ב־Services: יצירה/ולידציה, חישובי דוח (mock repositories), וכדומה.
- הרצה:
  ```bash
  dotnet test
  ```

---

## נקודות קצה עיקריות (API)
- **Inquiries**
  - `GET /api/inquiries`
  - `GET /api/inquiries/{id}`
  - `POST /api/inquiries`
  - `PUT  /api/inquiries/{id}`
  - `DELETE /api/inquiries/{id}`

- **Departments**
  - `GET /api/departments`

- **Reports**
  - `GET /api/reports/monthly?year=YYYY&month=MM`

**דוגמה (curl) — דוח חודשי:**
```bash
curl "https://localhost:5005/api/reports/monthly?year=2025&month=8"
```

---

## התקנה והרצה

### דרישות מקדימות
- **.NET SDK 8**
- **SQL Server LocalDB** (מגיע עם Visual Studio / ניתן להתקנה בנפרד)
- (אופציונלי) **dotnet-ef** לכלי מיגרציות:
  ```bash
  dotnet tool update -g dotnet-ef
  ```

### שלבים
```bash
# 1) שחזור תלויות ובנייה
dotnet restore
dotnet build

# 2) הרצה (ה־DB והמיגרציות יטופלו אוטומטית)
dotnet run --project ./src/Inquiries.Api.csproj

# 3) גלישה ל־Swagger
# (הפורט מוגדר ב-launchSettings.json, לדוגמה 5005)
https://localhost:5005/swagger
```

### בדיקת חיבור ל־LocalDB (אופציונלי)
```powershell
sqllocaldb info MSSQLLocalDB      # בדיקת אינסטנס
sqllocaldb start MSSQLLocalDB     # הפעלה אם צריך
sqlcmd -S "(localdb)\MSSQLLocalDB" -d InquiriesDb -Q "SELECT TOP 5 * FROM dbo.Inquiries;"
```

---

## הערות יישום
- קובץ ה־SP (`GetMonthlyInquiryReport.sql`) נכלל בפרויקט כ־`Content` עם `CopyToOutputDirectory=PreserveNewest`, ומותקן אוטומטית ב־startup.
- עבור **Seed היסטורי** לצורך בדיקת הדוח: הקוד מוסיף מספר פניות עם תאריכים בחודש קודם/בשנה קודמת — כך שהבודק רואה השוואות כבר בהרצה ראשונה.
- ניתן לכבות זריעת דמו לפי `IHostEnvironment` (Production ללא דמו).

---

## בנייה/פריסה
- **Build**: `dotnet build -c Release`
- **Publish Self-Contained** (דוגמה, win-x64):
  ```bash
  dotnet publish ./src/Inquiries.Api.csproj -c Release -r win-x64 --self-contained true -o ./publish
  ```

---

## שאלות נפוצות (FAQ)
**אין לי נתונים היסטוריים — הדוח ריק.**  
הפעלת זריעת הדמו מבטיחה נתוני עבר לצורך בדיקה. לחלופין, ניתן לשלוח פניות ידניות ולהתאים את התאריך ידנית בבסיס הנתונים לצורך בדיקה.

**שגיאת 404 מה-Frontend**  
ודאו שה־API רץ, שה־CORS/Proxy מוגדרים נכון, ושנתיב ה־endpoint תואם (למשל `/api/departments` לעומת `/api/inquiries/departments`).

---

## תלויות עיקריות
- `Microsoft.EntityFrameworkCore` / `SqlServer` / `Design`
- `Microsoft.Data.SqlClient`
- `Dapper`
- `Swashbuckle.AspNetCore` (Swagger)

---

© Inquiries API — קוד לדוגמה למטלת בית (.NET).

