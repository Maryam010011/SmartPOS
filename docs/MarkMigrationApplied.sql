-- ============================================================
-- Run this in SSMS ONCE to tell EF the migration is already
-- applied (tables already exist from previous migrations).
-- ============================================================

USE SmartPOS;
GO

-- Only insert if not already there
IF NOT EXISTS (
    SELECT 1 FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260607113622_FullSchema'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260607113622_FullSchema', N'10.0.0');
    PRINT 'Migration marked as applied.';
END
ELSE
BEGIN
    PRINT 'Migration was already marked as applied.';
END
GO
