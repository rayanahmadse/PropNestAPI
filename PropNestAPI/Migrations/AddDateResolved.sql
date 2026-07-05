-- Add DateResolved nullable column to MaintenanceRequest
ALTER TABLE MaintenanceRequest
ADD DateResolved DATETIME NULL;
