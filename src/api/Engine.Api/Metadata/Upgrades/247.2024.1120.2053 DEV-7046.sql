CREATE SCHEMA [identity];
GO

CREATE TABLE [identity].TEnterprise 
(
 EnterpriseHost VARCHAR(50) NOT NULL,
 EnterpriseEmail VARCHAR(254) NOT NULL,

 EnterpriseDomains VARCHAR(400) NOT NULL,
 EnterpriseTesters VARCHAR(400) NULL,

 EnterpriseIdentifier UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
 EnterpriseName VARCHAR(50) NOT NULL,
 EnterpriseNumber INT NOT NULL,
 EnterpriseSlug VARCHAR(3) NOT NULL,
);

DECLARE @e01 UNIQUEIDENTIFIER = '2b476ca4-6719-43b2-a4c9-5f476725d132';
DECLARE @e02 UNIQUEIDENTIFIER = 'd88d0913-1c49-48d6-9c0b-2e0aef769d28';
DECLARE @e03 UNIQUEIDENTIFIER = '01eefbf0-9250-4396-842f-698d0bdc7b1a';
DECLARE @e04 UNIQUEIDENTIFIER = '19443ea2-b880-4652-bc46-0521b442f2f4';
DECLARE @e05 UNIQUEIDENTIFIER = 'c0a5f349-44a9-42e2-a08b-d04b093c099d';
DECLARE @e06 UNIQUEIDENTIFIER = '32bb015f-8720-4d04-b480-1421ced0a4ec';
DECLARE @e07 UNIQUEIDENTIFIER = '5d4efaf9-0b12-4d4c-9250-cde9809d5531';
DECLARE @e99 UNIQUEIDENTIFIER = '6c48f170-9afd-4140-a78f-81fadde0e2ea';

DECLARE @domains VARCHAR(300) = 'cmds.app,insite.com,insitedemos.com,insitemail.com,insitemessages.com,insitesystems.com,itabc.ca,itaportal.ca,keyeracmds.com,membertech.com,mg.shiftiq.com,shiftiq.com,skilledtradesbc.ca,skillspassport.com';
DECLARE @email VARCHAR(254) = 'support@shiftiq.com';

INSERT INTO [identity].TEnterprise
(
    EnterpriseHost,
    EnterpriseIdentifier,
    EnterpriseName,
    EnterpriseNumber,
    EnterpriseSlug,
    EnterpriseDomains,
    EnterpriseEmail
)
VALUES
('e01.shiftiq.com', @e01, 'Demo', 1, 'E01', @domains, @email),
('e02.insite.com', @e02, 'General', 2, 'E02', @domains, @email),
('e03.insite.com', @e03, 'CMDS', 3, 'E03', @domains, @email),
('e04.shiftiq.com', @e04, 'Skilled Trades BC', 4, 'E04', @domains, @email),
('e05.shiftiq.com', @e05, 'Demo', 5, 'E05', @domains, @email),
('e06.insite.com', @e06, 'Inspire Global Assessments', 6, 'E06', @domains, @email),
('e07.shiftiq.com', @e07, 'SkillsCheck', 7, 'E07', @domains, @email),
('e99.shiftiq.com', @e99, 'Test', 99, 'E99', @domains, @email);

GO

UPDATE [identity].TEnterprise SET EnterpriseTesters = 'girl.feldy@gmail.com';
UPDATE [identity].TEnterprise SET EnterpriseTesters = 'sandra@bcpvpa.bc.ca,david@bcpvpa.bc.ca' WHERE EnterpriseNumber = 2;
GO