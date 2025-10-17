UPDATE [identity].TEnterprise SET EnterpriseSettings = '{"Database.Monitors.LargeCommandSize":"300000"}';

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS AS C WHERE C.TABLE_NAME = 'TProvince' AND C.COLUMN_NAME = 'ProvinceNameTranslation')
  ALTER TABLE contact.TProvince ADD ProvinceNameTranslation VARCHAR(MAX) NULL;