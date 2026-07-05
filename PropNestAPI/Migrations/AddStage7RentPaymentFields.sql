-- Stage 7: add late fee and reminder tracking fields to RentPayment
ALTER TABLE RentPayment
ADD LateFeeAmount DECIMAL(18, 2) NOT NULL CONSTRAINT DF_RentPayment_LateFeeAmount DEFAULT(0);

ALTER TABLE RentPayment
ADD LateFeeApplied BIT NOT NULL CONSTRAINT DF_RentPayment_LateFeeApplied DEFAULT(0);

ALTER TABLE RentPayment
ADD ReminderSentAt DATETIME NULL;

ALTER TABLE RentPayment
ADD ReminderSent BIT NOT NULL CONSTRAINT DF_RentPayment_ReminderSent DEFAULT(0);
