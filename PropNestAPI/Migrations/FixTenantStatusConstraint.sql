-- Migration: Fix CHK_Tenant_Status to allow Active and Inactive
-- Drop the existing constraint
IF EXISTS (
    SELECT 1 FROM sys.check_constraints 
    WHERE name = 'CHK_Tenant_Status'
)
BEGIN
    ALTER TABLE dbo.Tenant DROP CONSTRAINT CHK_Tenant_Status;
END

-- Re-add with both Active and Inactive allowed
ALTER TABLE dbo.Tenant
ADD CONSTRAINT CHK_Tenant_Status CHECK (Status IN ('Active', 'Inactive'));
