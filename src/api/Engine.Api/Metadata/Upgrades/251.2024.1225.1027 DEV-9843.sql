IF COL_LENGTH('[identity].TEnterprise','EnterpriseSettings') IS NULL
    ALTER TABLE [identity].TEnterprise ADD EnterpriseSettings VARCHAR(512) NULL;
GO

UPDATE [identity].TEnterprise SET EnterpriseSettings = '{"Database.Monitors.LargeCommandSize":"8192"}';
GO