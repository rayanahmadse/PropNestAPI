-- ============================================================
-- PropNest Seed Data Script
-- Covers: Staff, PropertyUnit, Tenant, RentalAgreement,
--         RentPayment, MaintenanceRequest
-- ============================================================

SET IDENTITY_INSERT Staff ON;

INSERT INTO Staff (StaffID, FullName, ContactNumber, Specialty, Status) VALUES
(1, 'Asim Raza',       '+92-300-1234567', 'Plumbing',    'Active'),
(2, 'Bilal Shaikh',    '+92-321-9876543', 'Electrical',  'Active'),
(3, 'Kamran Akhtar',   '+92-333-5551234', 'Structural',  'Active'),
(4, 'Nadeem Hussain',  '+92-311-7778899', 'General',     'Active'),
(5, 'Tariq Mehmood',   '+92-345-3334455', 'Other',       'Inactive');

SET IDENTITY_INSERT Staff OFF;

-- ============================================================
-- Property Units
-- Allowed Status: 'Vacant', 'Occupied'
-- Allowed PropertyType: 'Residential', 'Commercial'
-- ============================================================

SET IDENTITY_INSERT PropertyUnit ON;

INSERT INTO PropertyUnit (UnitID, UnitNumber, PropertyType, FloorLevel, AreaSqFt, Amenities, Status, AskingRent, VacantSince) VALUES
(1,  'A-101', 'Residential', 'Ground', 850.00,  'Parking, Generator Backup, 24/7 Security',            'Occupied', 35000.00, NULL),
(2,  'A-102', 'Residential', 'Ground', 750.00,  'Parking, CCTV, Water Tank',                            'Occupied', 30000.00, NULL),
(3,  'A-201', 'Residential', 'First',  950.00,  'Parking, Generator Backup, Elevator, Security',        'Occupied', 40000.00, NULL),
(4,  'A-202', 'Residential', 'First',  900.00,  'Parking, Elevator, Gas Geysers',                       'Vacant',   38000.00, '2026-05-01'),
(5,  'B-101', 'Residential', 'Ground', 1100.00, 'Parking, Generator Backup, Servant Quarter',           'Occupied', 55000.00, NULL),
(6,  'B-102', 'Residential', 'Ground', 1050.00, 'Parking, CCTV, Lawn Area',                             'Vacant',   50000.00, '2026-06-15'),
(7,  'B-201', 'Residential', 'First',  1200.00, 'Parking, Rooftop Access, Generator Backup, Elevator',  'Occupied', 60000.00, NULL),
(8,  'C-001', 'Commercial',  'Ground', 500.00,  'Main Road Facing, Signage Rights, 3-Phase Power',      'Occupied', 80000.00, NULL),
(9,  'C-002', 'Commercial',  'Ground', 650.00,  '3-Phase Power, Storage Room, CCTV',                    'Occupied', 90000.00, NULL),
(10, 'C-003', 'Commercial',  'Ground', 400.00,  'Corner Unit, High Footfall Area',                      'Vacant',   70000.00, '2026-04-01');

SET IDENTITY_INSERT PropertyUnit OFF;

-- ============================================================
-- Tenants
-- Allowed Status: 'Active', 'Inactive'
-- Active = has an agreement, Inactive = no current agreement
-- ============================================================

SET IDENTITY_INSERT Tenant ON;

INSERT INTO Tenant (TenantID, FullName, CNIC, Email, ContactNumber, EmergencyContact, Status) VALUES
(1,  'Ahmed Raza Mirza',      '35202-1234567-1', 'ahmed.mirza@gmail.com',     '+92-300-1110001', '+92-321-2220001', 'Active'),
(2,  'Sara Qureshi',          '35202-2345678-2', 'sara.qureshi@outlook.com',  '+92-321-1110002', '+92-333-2220002', 'Active'),
(3,  'Mohammad Usman',        '35301-3456789-3', 'usman.biz@gmail.com',       '+92-333-1110003', '+92-300-2220003', 'Active'),
(4,  'Nadia Tariq',           '35201-4567890-4', 'nadia.tariq@gmail.com',     '+92-311-1110004', '+92-321-2220004', 'Active'),
(5,  'Hamza Siddiqui',        '35202-5678901-5', 'hamza.sid@hotmail.com',     '+92-345-1110005', '+92-311-2220005', 'Active'),
(6,  'Zara Malik',            '35301-6789012-6', 'zara.malik@gmail.com',      '+92-300-1110006', '+92-345-2220006', 'Active'),
(7,  'Faisal Iqbal',          '35401-7890123-7', 'faisal.iqbal@outlook.com',  '+92-321-1110007', '+92-300-2220007', 'Active'),
(8,  'Amina Baig',            '35202-8901234-8', 'amina.baig@gmail.com',      '+92-333-1110008', '+92-321-2220008', 'Active'),
(9,  'Tariq Mehmood Khan',    '35301-9012345-9', 'tariq.khan@gmail.com',      '+92-311-1110009', '+92-333-2220009', 'Inactive'),
(10, 'Rukhsana Parveen',      '35201-0123456-0', 'rukhsana.p@outlook.com',    '+92-345-1110010', '+92-311-2220010', 'Inactive');

SET IDENTITY_INSERT Tenant OFF;

-- ============================================================
-- Rental Agreements
-- Allowed Status: 'Active', 'Expired', 'Terminated', 'Renewed'
-- ============================================================

SET IDENTITY_INSERT RentalAgreement ON;

INSERT INTO RentalAgreement (AgreementID, TenantID, UnitID, StartDate, EndDate, MonthlyRent, SecurityDeposit, AgreementStatus, Version) VALUES
-- Active agreements (current tenants)
(1,  1, 1, '2025-07-01', '2026-12-31', 35000.00, 70000.00,  'Active',      1),
(2,  2, 2, '2025-09-01', '2026-08-31', 30000.00, 60000.00,  'Active',      1),
(3,  3, 8, '2025-10-01', '2026-09-30', 80000.00, 160000.00, 'Active',      1),
(4,  4, 3, '2026-01-01', '2026-12-31', 40000.00, 80000.00,  'Active',      1),
(5,  5, 5, '2026-02-01', '2027-01-31', 55000.00, 110000.00, 'Active',      1),
(6,  6, 7, '2026-03-01', '2027-02-28', 60000.00, 120000.00, 'Active',      1),
(7,  7, 9, '2026-04-01', '2027-03-31', 90000.00, 180000.00, 'Active',      1),
(8,  8, 2, '2024-06-01', '2025-05-31', 28000.00, 56000.00,  'Expired',     1), -- previous tenant on Unit 2
-- Expired agreement
(9,  9, 4, '2024-01-01', '2025-01-01', 36000.00, 72000.00,  'Expired',     1),
-- Terminated agreement
(10, 10, 6, '2024-03-01', '2025-02-28', 48000.00, 96000.00, 'Terminated',  1),
-- Renewed agreement (Tenant 1 had an older agreement, now renewed)
(11, 1, 1, '2024-07-01', '2025-06-30', 33000.00, 66000.00,  'Renewed',     1);

SET IDENTITY_INSERT RentalAgreement OFF;

-- ============================================================
-- Rent Payments
-- Allowed Status: 'Paid', 'Pending', 'Overdue'
-- Allowed PaymentMethod: 'Cash', 'Bank Transfer', 'Cheque', 'Online', 'Pending'
-- ============================================================

SET IDENTITY_INSERT RentPayment ON;

-- Agreement 1 (Ahmed Raza Mirza, Unit A-101, 35000/mo, Jul 2025-Dec 2026)
INSERT INTO RentPayment (PaymentID, AgreementID, DueDate, PaymentDate, AmountPaid, PaymentMethod, Status, ReceiptPath, ReceiptGenerated) VALUES
(1,  1, '2025-07-01', '2025-07-03',  35000.00, 'Bank Transfer', 'Paid',    NULL, 0),
(2,  1, '2025-08-01', '2025-08-05',  35000.00, 'Bank Transfer', 'Paid',    NULL, 0),
(3,  1, '2025-09-01', '2025-09-02',  35000.00, 'Cash',          'Paid',    NULL, 0),
(4,  1, '2025-10-01', '2025-10-04',  35000.00, 'Online',        'Paid',    NULL, 0),
(5,  1, '2025-11-01', '2025-11-06',  35000.00, 'Bank Transfer', 'Paid',    NULL, 0),
(6,  1, '2025-12-01', '2025-12-03',  35000.00, 'Cash',          'Paid',    NULL, 0),
(7,  1, '2026-01-01', '2026-01-07',  35000.00, 'Online',        'Paid',    NULL, 0),
(8,  1, '2026-02-01', '2026-02-04',  35000.00, 'Bank Transfer', 'Paid',    NULL, 0),
(9,  1, '2026-03-01', '2026-03-05',  35000.00, 'Cheque',        'Paid',    NULL, 0),
(10, 1, '2026-04-01', '2026-04-02',  35000.00, 'Online',        'Paid',    NULL, 0),
(11, 1, '2026-05-01', '2026-05-08',  35000.00, 'Cash',          'Paid',    NULL, 0),
(12, 1, '2026-06-01', '2026-06-03',  35000.00, 'Bank Transfer', 'Paid',    NULL, 0),
(13, 1, '2026-07-01', NULL,           35000.00, 'Cash',          'Pending', NULL, 0),
(14, 1, '2026-08-01', NULL,           35000.00, 'Cash',          'Pending', NULL, 0),

-- Agreement 2 (Sara Qureshi, Unit A-102, 30000/mo, Sep 2025-Aug 2026)
(15, 2, '2025-09-01', '2025-09-10', 30000.00, 'Cash',          'Paid',    NULL, 0),
(16, 2, '2025-10-01', '2025-10-09', 30000.00, 'Cash',          'Paid',    NULL, 0),
(17, 2, '2025-11-01', '2025-11-12', 30000.00, 'Online',        'Paid',    NULL, 0),
(18, 2, '2025-12-01', '2025-12-08', 30000.00, 'Cash',          'Paid',    NULL, 0),
(19, 2, '2026-01-01', '2026-01-11', 30000.00, 'Bank Transfer', 'Paid',    NULL, 0),
(20, 2, '2026-02-01', '2026-02-09', 30000.00, 'Online',        'Paid',    NULL, 0),
(21, 2, '2026-03-01', '2026-03-07', 30000.00, 'Cash',          'Paid',    NULL, 0),
(22, 2, '2026-04-01', NULL,          30000.00, 'Cash',          'Overdue', NULL, 0),
(23, 2, '2026-05-01', NULL,          30000.00, 'Cash',          'Overdue', NULL, 0),
(24, 2, '2026-06-01', NULL,          30000.00, 'Cash',          'Overdue', NULL, 0),
(25, 2, '2026-07-01', NULL,          30000.00, 'Cash',          'Pending', NULL, 0),

-- Agreement 3 (Mohammad Usman, Unit C-001, 80000/mo, Oct 2025-Sep 2026)
(26, 3, '2025-10-01', '2025-10-05', 80000.00, 'Cheque',        'Paid',    NULL, 0),
(27, 3, '2025-11-01', '2025-11-04', 80000.00, 'Cheque',        'Paid',    NULL, 0),
(28, 3, '2025-12-01', '2025-12-03', 80000.00, 'Cheque',        'Paid',    NULL, 0),
(29, 3, '2026-01-01', '2026-01-05', 80000.00, 'Cheque',        'Paid',    NULL, 0),
(30, 3, '2026-02-01', '2026-02-06', 80000.00, 'Bank Transfer', 'Paid',    NULL, 0),
(31, 3, '2026-03-01', '2026-03-04', 80000.00, 'Cheque',        'Paid',    NULL, 0),
(32, 3, '2026-04-01', '2026-04-07', 80000.00, 'Cheque',        'Paid',    NULL, 0),
(33, 3, '2026-05-01', '2026-05-06', 80000.00, 'Bank Transfer', 'Paid',    NULL, 0),
(34, 3, '2026-06-01', '2026-06-05', 80000.00, 'Cheque',        'Paid',    NULL, 0),
(35, 3, '2026-07-01', NULL,          80000.00, 'Cash',          'Pending', NULL, 0),

-- Agreement 4 (Nadia Tariq, Unit A-201, 40000/mo, Jan 2026-Dec 2026)
(36, 4, '2026-01-01', '2026-01-08', 40000.00, 'Bank Transfer', 'Paid',    NULL, 0),
(37, 4, '2026-02-01', '2026-02-10', 40000.00, 'Online',        'Paid',    NULL, 0),
(38, 4, '2026-03-01', '2026-03-06', 40000.00, 'Bank Transfer', 'Paid',    NULL, 0),
(39, 4, '2026-04-01', '2026-04-09', 40000.00, 'Online',        'Paid',    NULL, 0),
(40, 4, '2026-05-01', '2026-05-07', 40000.00, 'Bank Transfer', 'Paid',    NULL, 0),
(41, 4, '2026-06-01', NULL,          40000.00, 'Cash',          'Overdue', NULL, 0),
(42, 4, '2026-07-01', NULL,          40000.00, 'Cash',          'Pending', NULL, 0),

-- Agreement 5 (Hamza Siddiqui, Unit B-101, 55000/mo, Feb 2026-Jan 2027)
(43, 5, '2026-02-01', '2026-02-03', 55000.00, 'Cash',          'Paid',    NULL, 0),
(44, 5, '2026-03-01', '2026-03-04', 55000.00, 'Cash',          'Paid',    NULL, 0),
(45, 5, '2026-04-01', '2026-04-02', 55000.00, 'Cash',          'Paid',    NULL, 0),
(46, 5, '2026-05-01', '2026-05-04', 55000.00, 'Cash',          'Paid',    NULL, 0),
(47, 5, '2026-06-01', '2026-06-03', 55000.00, 'Cash',          'Paid',    NULL, 0),
(48, 5, '2026-07-01', NULL,          55000.00, 'Cash',          'Pending', NULL, 0),

-- Agreement 6 (Zara Malik, Unit B-201, 60000/mo, Mar 2026-Feb 2027)
(49, 6, '2026-03-01', '2026-03-08', 60000.00, 'Online',        'Paid',    NULL, 0),
(50, 6, '2026-04-01', '2026-04-10', 60000.00, 'Online',        'Paid',    NULL, 0),
(51, 6, '2026-05-01', '2026-05-09', 60000.00, 'Bank Transfer', 'Paid',    NULL, 0),
(52, 6, '2026-06-01', '2026-06-07', 60000.00, 'Online',        'Paid',    NULL, 0),
(53, 6, '2026-07-01', NULL,          60000.00, 'Cash',          'Pending', NULL, 0),

-- Agreement 7 (Faisal Iqbal, Unit C-002, 90000/mo, Apr 2026-Mar 2027)
(54, 7, '2026-04-01', '2026-04-06', 90000.00, 'Cheque',        'Paid',    NULL, 0),
(55, 7, '2026-05-01', '2026-05-05', 90000.00, 'Cheque',        'Paid',    NULL, 0),
(56, 7, '2026-06-01', '2026-06-04', 90000.00, 'Bank Transfer', 'Paid',    NULL, 0),
(57, 7, '2026-07-01', NULL,          90000.00, 'Cash',          'Pending', NULL, 0),

-- Agreement 9 (Tariq Mehmood - Expired, Unit A-202)
(58, 9, '2024-01-01', '2024-02-05', 36000.00, 'Cash',          'Paid',    NULL, 0),
(59, 9, '2024-02-01', '2024-03-08', 36000.00, 'Cash',          'Paid',    NULL, 0),
(60, 9, '2024-03-01', '2024-04-06', 36000.00, 'Bank Transfer', 'Paid',    NULL, 0),
(61, 9, '2024-04-01', NULL,          36000.00, 'Cash',          'Overdue', NULL, 0),
(62, 9, '2024-05-01', NULL,          36000.00, 'Cash',          'Overdue', NULL, 0),
(63, 9, '2024-06-01', NULL,          36000.00, 'Cash',          'Overdue', NULL, 0),

-- Agreement 10 (Rukhsana Parveen - Terminated, Unit B-102)
(64, 10, '2024-03-01', '2024-03-05', 48000.00, 'Online',       'Paid',    NULL, 0),
(65, 10, '2024-04-01', '2024-04-04', 48000.00, 'Online',       'Paid',    NULL, 0),
(66, 10, '2024-05-01', NULL,          48000.00, 'Cash',         'Overdue', NULL, 0);

SET IDENTITY_INSERT RentPayment OFF;

-- ============================================================
-- Maintenance Requests
-- Allowed Status: 'Open', 'In Progress', 'Resolved'
-- Allowed Category: 'Plumbing', 'Electrical', 'Structural', 'General', 'Other'
-- ============================================================

SET IDENTITY_INSERT MaintenanceRequest ON;

INSERT INTO MaintenanceRequest (RequestID, UnitID, StaffID, Category, Description, DateLogged, Status, DateResolved) VALUES
(1,  1, 1, 'Plumbing',   'Kitchen sink drain blocked, water backing up.',                    '2026-05-10 09:00:00', 'Resolved',    '2026-05-11 14:00:00'),
(2,  2, 2, 'Electrical', 'Power tripping on main circuit. Whole unit affected.',             '2026-05-15 11:30:00', 'Resolved',    '2026-05-16 10:00:00'),
(3,  3, 3, 'Structural', 'Hairline cracks visible on the bedroom wall near the window.',     '2026-06-01 08:00:00', 'Resolved',    '2026-06-05 17:00:00'),
(4,  5, 4, 'General',    'Main gate lock is broken. Security concern.',                      '2026-06-10 14:00:00', 'Resolved',    '2026-06-10 18:00:00'),
(5,  7, 2, 'Electrical', 'Ceiling fan in master bedroom making grinding noise.',             '2026-06-20 10:00:00', 'Resolved',    '2026-06-22 12:00:00'),
(6,  8, 1, 'Plumbing',   'Shop washroom tap leaking continuously, water wasted.',            '2026-07-01 09:30:00', 'In Progress', NULL),
(7,  1, 2, 'Electrical', 'Two power sockets in the lounge are not working.',                 '2026-07-03 13:00:00', 'In Progress', NULL),
(8,  9, 4, 'General',    'Shop shutter mechanism stuck. Cannot open fully.',                 '2026-07-05 08:00:00', 'In Progress', NULL),
(9,  3, 1, 'Plumbing',   'Hot water geyser not heating properly. Cold water only.',          '2026-07-07 10:00:00', 'Open',        NULL),
(10, 5, NULL, 'Structural','Ceiling plaster falling in the second bedroom.',                 '2026-07-08 15:00:00', 'Open',        NULL),
(11, 7, NULL, 'Other',    'Rooftop access door lock is jammed. Tenant cannot access.',       '2026-07-09 11:00:00', 'Open',        NULL),
(12, 2, NULL, 'General',  'Intercom unit at main entrance not working. Visitors cannot buzz.','2026-07-10 09:00:00', 'Open',        NULL);

SET IDENTITY_INSERT MaintenanceRequest OFF;
