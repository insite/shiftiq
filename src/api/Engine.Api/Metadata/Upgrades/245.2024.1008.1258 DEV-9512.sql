drop procedure if exists assets.IncrementSequence;

drop table if exists accounts.TEnterprise;
drop table if exists accounts.TOrganization;
drop table if exists accounts.TRole;
drop table if exists accounts.TRoleClaim;
drop table if exists accounts.TUser;
drop table if exists accounts.TUserClaim;
drop table if exists accounts.TUserRole;

drop table if exists assets.TInput;
drop table if exists assets.TLabel;
drop table if exists assets.TObject;
drop table if exists assets.TSequence;
drop table if exists assets.TText;

drop table if exists contacts.TGroup;
drop table if exists contacts.TGroupClaim;
drop table if exists contacts.TMembership;
drop table if exists contacts.TPerson;
drop table if exists contacts.TPersonClaim;
drop table if exists contacts.TPostalCode;

drop table if exists integrations.TApiCall;
drop table if exists integrations.TAuthentication;
drop table if exists integrations.TImpersonation;

drop table if exists settings.TApplication;
drop table if exists settings.TEnvironment;

go

create schema [content];
go

create schema [contact];
go

create schema [database];
go

alter schema [content] transfer assets.TTranslation;
alter schema contact transfer contacts.TCity;
alter schema contact transfer contacts.TCountry;
alter schema contact transfer contacts.TState;
alter schema [database] transfer [databases].TUpgrade;
go

drop table if exists courses.BInteractionSession, integrations.TWebhook, integrations.TWebhookLog, records.TStatement;
go

drop schema accounts; 
drop schema assets;
drop schema contacts;
drop schema courses;
drop schema [databases];
drop schema integrations;
drop schema locations;
drop schema records;
drop schema settings;
go

exec sp_rename 'contact.TState', 'TProvince';
go
exec sp_rename 'contact.TProvince.StateCode', 'ProvinceCode';
exec sp_rename 'contact.TProvince.StateName', 'ProvinceName';
exec sp_rename 'contact.TProvince.StateNameTranslation', 'ProvinceNameTranslation';
go
exec sp_rename 'contact.TCity.StateCode', 'ProvinceCode';
go

truncate table [database].TUpgrade;
go