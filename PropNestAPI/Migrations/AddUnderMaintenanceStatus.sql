-- Migration: Allow 'Under Maintenance' status in PropertyUnit
IF EXISTS (
    SELECT 1 FROM sys.check_constraints 
    WHERE name = 'CHK_Unit_Status'
)
BEGIN
    ALTER TABLE dbo.PropertyUnit DROP CONSTRAINT CHK_Unit_Status;
END

ALTER TABLE dbo.PropertyUnit
ADD CONSTRAINT CHK_Unit_Status CHECK (Status IN ('Vacant', 'Occupied', 'Under Maintenance'));
