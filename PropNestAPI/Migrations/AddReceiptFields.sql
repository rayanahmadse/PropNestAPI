-- Add ReceiptPath and ReceiptGenerated to RentPayment
ALTER TABLE RentPayment
ADD ReceiptPath NVARCHAR(512) NULL;

ALTER TABLE RentPayment
ADD ReceiptGenerated BIT DEFAULT 0;
