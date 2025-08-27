DECLARE @now DATETIME2 = SYSUTCDATETIME();
DECLARE @curStart DATE = DATEFROMPARTS(YEAR(@now), MONTH(@now), 1);

DECLARE @prevStart DATE = DATEADD(MONTH,-1,@curStart);
DECLARE @prevEnd   DATE = @curStart;

DECLARE @lyStart   DATE = DATEADD(YEAR,-1,@curStart);
DECLARE @lyEnd     DATE = DATEADD(MONTH,1,@lyStart);

DECLARE @Target TABLE (Id INT PRIMARY KEY);

INSERT INTO @Target(Id)
SELECT i.Id
FROM dbo.Inquiries i
WHERE (i.CreatedAtUtc >= @prevStart AND i.CreatedAtUtc < @prevEnd)
   OR (i.CreatedAtUtc >= @lyStart   AND i.CreatedAtUtc < @lyEnd);

-- מחיקת צאצאים קודם
DELETE d
FROM dbo.InquiryDepartments AS d
JOIN @Target               AS t ON t.Id = d.InquiryId;

-- מחיקת הפניות עצמן
DELETE i
FROM dbo.Inquiries AS i
JOIN @Target       AS t ON t.Id = i.Id;
