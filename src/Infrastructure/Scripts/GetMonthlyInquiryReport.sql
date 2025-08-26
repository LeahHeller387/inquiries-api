CREATE OR ALTER PROCEDURE dbo.GetMonthlyInquiryReport
    @Year  INT,
    @Month INT
AS
BEGIN
    SET NOCOUNT ON;

    IF (@Month < 1 OR @Month > 12) OR (@Year < 2000 OR @Year > 2100)
    BEGIN
        THROW 50001, 'Invalid @Year/@Month. Expected @Month in 1-12 and reasonable @Year.', 1;
    END

    DECLARE @StartDate  DATE = DATEFROMPARTS(@Year, @Month, 1);
    DECLARE @NextMonth  DATE = DATEADD(MONTH, 1, @StartDate);
    DECLARE @PrevStart  DATE = DATEADD(MONTH, -1, @StartDate);
    DECLARE @PrevEnd    DATE = @StartDate;
    DECLARE @LYStart    DATE = DATEADD(YEAR, -1, @StartDate);
    DECLARE @LYEnd      DATE = DATEADD(MONTH, 1, @LYStart);

    
    SELECT
        d.Id   AS DepartmentId,
        d.Name AS DepartmentName,
        SUM(CASE WHEN i.CreatedAtUtc >= @StartDate AND i.CreatedAtUtc < @NextMonth THEN 1 ELSE 0 END) AS CurrentMonthCount,
        SUM(CASE WHEN i.CreatedAtUtc >= @PrevStart  AND i.CreatedAtUtc < @PrevEnd   THEN 1 ELSE 0 END) AS PrevMonthCount,
        SUM(CASE WHEN i.CreatedAtUtc >= @LYStart    AND i.CreatedAtUtc < @LYEnd     THEN 1 ELSE 0 END) AS SameMonthLastYearCount
    FROM dbo.Departments d
    LEFT JOIN dbo.InquiryDepartments id
           ON id.DepartmentId = d.Id
    LEFT JOIN dbo.Inquiries i
           ON i.Id = id.InquiryId
          AND i.CreatedAtUtc >= @LYStart
          AND i.CreatedAtUtc <  @NextMonth
    GROUP BY d.Id, d.Name
    ORDER BY d.Id;
END
