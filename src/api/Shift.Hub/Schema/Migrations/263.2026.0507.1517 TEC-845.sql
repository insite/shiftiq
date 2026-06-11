-- Create new tables using the snake_case naming conventions that will be required when the db moves to PostgreSQL.

CREATE TABLE upgrade (
    script_name VARCHAR(128)   NOT NULL CONSTRAINT pk_upgrade PRIMARY KEY,
    executed_at DATETIMEOFFSET NOT NULL,
    script_data VARCHAR(MAX)
)
go

CREATE TABLE city (
    city_name     VARCHAR(60)   NOT NULL,
    province_code VARCHAR(2)    NOT NULL,
    country_code  VARCHAR(2)    NOT NULL,
    latitude      DECIMAL(9, 6) NOT NULL,
    longitude     DECIMAL(9, 6) NOT NULL,
    CONSTRAINT pk_city PRIMARY KEY (city_name, province_code, country_code)
)
go

CREATE TABLE country (
    country_code       VARCHAR(2)   NOT NULL CONSTRAINT pk_country PRIMARY KEY,
    country_name       VARCHAR(50)  NOT NULL,
    capital_city_name  VARCHAR(60),
    continent_code     VARCHAR(2),
    currency_code      VARCHAR(3),
    currency_name      VARCHAR(20),
    languages          VARCHAR(60),
    top_level_domain   VARCHAR(3),
    country_id UNIQUEIDENTIFIER DEFAULT NEWID() NOT NULL
)
go

-- Load the new tables. Leave the old tables intact.

CREATE TABLE province (
    province_name              VARCHAR(80) NOT NULL,
    province_name_translations VARCHAR(200),
    province_code              VARCHAR(2),
    country_code               VARCHAR(2)  NOT NULL,

    CONSTRAINT pk_province PRIMARY KEY (province_name, country_code)
)
go

CREATE TABLE translation (
    en                     NVARCHAR(MAX),
    ar                     NVARCHAR(MAX),
    de                     NVARCHAR(MAX),
    eo                     NVARCHAR(MAX),
    es                     NVARCHAR(MAX),
    fr                     NVARCHAR(MAX),
    he                     NVARCHAR(MAX),
    it                     NVARCHAR(MAX),
    ja                     NVARCHAR(MAX),
    ko                     NVARCHAR(MAX),
    la                     NVARCHAR(MAX),
    nl                     NVARCHAR(MAX),
    no                     NVARCHAR(MAX),
    pa                     NVARCHAR(MAX),
    pl                     NVARCHAR(MAX),
    pt                     NVARCHAR(MAX),
    ru                     NVARCHAR(MAX),
    sv                     NVARCHAR(MAX),
    uk                     NVARCHAR(MAX),
    zh                     NVARCHAR(MAX),
    created_at      DATETIMEOFFSET CONSTRAINT df_created_at DEFAULT GETUTCDATE()  NOT NULL,
    modified_at     DATETIMEOFFSET CONSTRAINT df_modified_at DEFAULT GETUTCDATE() NOT NULL,
    expired_at      DATETIMEOFFSET,
    translation_id UNIQUEIDENTIFIER NOT NULL CONSTRAINT pk_translation PRIMARY KEY NONCLUSTERED
)
go

INSERT INTO country (
    country_code, country_name, capital_city_name, continent_code,
    currency_code, currency_name, languages, top_level_domain, country_id
)
SELECT
    CountryCode, CountryName, CapitalCityName, ContinentCode,
    CurrencyCode, CurrencyName, Languages, TopLevelDomain, CountryIdentifier
FROM contact.TCountry
go

INSERT INTO province (
    province_name, province_name_translations, province_code, country_code
)
SELECT
    ProvinceName, ProvinceNameTranslation, ProvinceCode, CountryCode
FROM contact.TProvince
go

INSERT INTO city (
    city_name, province_code, country_code, latitude, longitude
)
SELECT
    CityName, ProvinceCode, CountryCode, Latitude, Longitude
FROM contact.TCity
go

INSERT INTO translation (
    en, ar, de, eo, es, fr, he, it, ja, ko,
    la, nl, no, pa, pl, pt, ru, sv, uk, zh,
    created_at, modified_at, expired_at, translation_id
)
SELECT
    en, ar, de, eo, es, fr, he, it, ja, ko,
    la, nl, no, pa, pl, pt, ru, sv, uk, zh,
    TimestampCreated, TimestampModified, TimestampExpired, TranslationIdentifier
FROM content.TTranslation
go

INSERT INTO dbo.[upgrade] (script_name, executed_at, script_data)
VALUES  ('257.2025.1126.2341 TEC-329.sql', GETUTCDATE(), '-- Prepare for migration to PostgreSQL'),
        ('263.2026.0507.1517 TEC-845.sql', GETUTCDATE(), '-- Prepare for migration to PostgreSQL');
go