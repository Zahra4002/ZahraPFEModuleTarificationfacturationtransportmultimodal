using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistance.Migrations
{
    /// <inheritdoc />
    public partial class firstMigr1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    TaxId = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    Billing_Street = table.Column<string>(type: "text", nullable: false),
                    Billing_City = table.Column<string>(type: "text", nullable: false),
                    BullingAddress_PostalCode = table.Column<string>(type: "text", nullable: false),
                    BullingAddress_Country = table.Column<string>(type: "text", nullable: false),
                    BullingAddress_State = table.Column<string>(type: "text", nullable: true),
                    BullingAddress_Latitude = table.Column<double>(type: "double precision", nullable: true),
                    BullingAddress_Longitude = table.Column<double>(type: "double precision", nullable: true),
                    Shipping_Street = table.Column<string>(type: "text", nullable: false),
                    Shipping_City = table.Column<string>(type: "text", nullable: false),
                    ShippingAddress_PostalCode = table.Column<string>(type: "text", nullable: false),
                    ShippingAddress_Country = table.Column<string>(type: "text", nullable: false),
                    ShippingAddress_State = table.Column<string>(type: "text", nullable: true),
                    ShippingAddress_Latitude = table.Column<double>(type: "double precision", nullable: true),
                    ShippingAddress_Longitude = table.Column<double>(type: "double precision", nullable: true),
                    DefaultCurrencyCode = table.Column<string>(type: "text", nullable: false),
                    CreditLimit = table.Column<decimal>(type: "numeric", nullable: false),
                    PaymentTermDays = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedById = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Symbol = table.Column<string>(type: "text", nullable: false),
                    DecimalPlaces = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedById = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MerchandiseTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    HazardousLevel = table.Column<int>(type: "integer", nullable: false),
                    PriceMultiplier = table.Column<decimal>(type: "numeric", nullable: true),
                    RequiresSpecialHandling = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedById = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchandiseTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    TaxId = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: false),
                    DefaultCurrencyCode = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedById = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Surcharges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CalculationType = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedById = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Surcharges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TariffGrids",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    TransportMode = table.Column<int>(type: "integer", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CurrencyCode = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedById = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TariffGrids", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CompanyName = table.Column<string>(type: "text", nullable: false),
                    Contact = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedById = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FailedLoginAttempts = table.Column<int>(type: "integer", nullable: false),
                    LockoutEnd = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedById = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WebhookConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    Secret = table.Column<string>(type: "text", nullable: false),
                    SubscribedEvents = table.Column<string>(type: "text", nullable: false),
                    CustomHeaders = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    MaxRetries = table.Column<int>(type: "integer", nullable: false),
                    RetryDelaySeconds = table.Column<int>(type: "integer", nullable: false),
                    TotalCalls = table.Column<int>(type: "integer", nullable: false),
                    SuccessfulCalls = table.Column<int>(type: "integer", nullable: false),
                    FailedCalls = table.Column<int>(type: "integer", nullable: false),
                    LastCallAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastCallSuccess = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedById = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebhookConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Zones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Country = table.Column<string>(type: "text", nullable: false),
                    Region = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedById = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeRates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FromCurrencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    ToCurrencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rate = table.Column<decimal>(type: "numeric", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Source = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedById = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExchangeRates_Currencies_FromCurrencyId",
                        column: x => x.FromCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExchangeRates_Currencies_ToCurrencyId",
                        column: x => x.ToCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Quotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuoteNumber = table.Column<string>(type: "text", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: true),
                    Quote_Origin_Street = table.Column<string>(type: "text", nullable: false),
                    Quote_Origin_City = table.Column<string>(type: "text", nullable: false),
                    OriginAddress_PostalCode = table.Column<string>(type: "text", nullable: false),
                    OriginAddress_Country = table.Column<string>(type: "text", nullable: false),
                    OriginAddress_State = table.Column<string>(type: "text", nullable: true),
                    OriginAddress_Latitude = table.Column<double>(type: "double precision", nullable: true),
                    OriginAddress_Longitude = table.Column<double>(type: "double precision", nullable: true),
                    Quote_Dest_Street = table.Column<string>(type: "text", nullable: false),
                    Quote_Dest_City = table.Column<string>(type: "text", nullable: false),
                    DestinationAddress_PostalCode = table.Column<string>(type: "text", nullable: false),
                    DestinationAddress_Country = table.Column<string>(type: "text", nullable: false),
                    DestinationAddress_State = table.Column<string>(type: "text", nullable: true),
                    DestinationAddress_Latitude = table.Column<double>(type: "double precision", nullable: true),
                    DestinationAddress_Longitude = table.Column<double>(type: "double precision", nullable: true),
                    WeightKg = table.Column<decimal>(type: "numeric", nullable: true),
                    VolumeM3 = table.Column<decimal>(type: "numeric", nullable: true),
                    MerchandiseTypeId = table.Column<Guid>(type: "uuid", nullable: true),
                    TotalHT = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalTTC = table.Column<decimal>(type: "numeric", nullable: false),
                    CurrencyCode = table.Column<string>(type: "text", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsAccepted = table.Column<bool>(type: "boolean", nullable: false),
                    AcceptedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PriceBreakdownJson = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedById = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quotes_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Quotes_MerchandiseTypes_MerchandiseTypeId",
                        column: x => x.MerchandiseTypeId,
                        principalTable: "MerchandiseTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContractNumber = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: true),
                    SupplierId = table.Column<Guid>(type: "uuid", nullable: true),
                    ValidFrom = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Terms = table.Column<string>(type: "text", nullable: true),
                    TermsAccepted = table.Column<bool>(type: "boolean", nullable: false),
                    TermsAcceptedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    GlobalDiscountPercent = table.Column<decimal>(type: "numeric", nullable: false),
                    MinimumVolume = table.Column<decimal>(type: "numeric", nullable: true),
                    MinimumVolumeUnit = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    AutoRenew = table.Column<bool>(type: "boolean", nullable: false),
                    RenewalNoticeDays = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedById = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contracts_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Contracts_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TaxRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Country = table.Column<string>(type: "text", nullable: false),
                    Region = table.Column<string>(type: "text", nullable: true),
                    StandardRate = table.Column<decimal>(type: "numeric", nullable: false),
                    ReducedRate = table.Column<decimal>(type: "numeric", nullable: true),
                    SuperReducedRate = table.Column<decimal>(type: "numeric", nullable: true),
                    ZeroRate = table.Column<decimal>(type: "numeric", nullable: true),
                    AllowExemption = table.Column<bool>(type: "boolean", nullable: false),
                    ExemptionConditions = table.Column<string>(type: "text", nullable: true),
                    SurchargeId = table.Column<Guid>(type: "uuid", nullable: true),
                    ValidFrom = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedById = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaxRules_Surcharges_SurchargeId",
                        column: x => x.SurchargeId,
                        principalTable: "Surcharges",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ActivityLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserEmail = table.Column<string>(type: "text", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: false),
                    EntityType = table.Column<string>(type: "text", nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    OldValuesJson = table.Column<string>(type: "text", nullable: true),
                    NewValuesJson = table.Column<string>(type: "text", nullable: true),
                    IpAddress = table.Column<string>(type: "text", nullable: true),
                    UserAgent = table.Column<string>(type: "text", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedById = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ApiKeys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    KeyHash = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Permissions = table.Column<string>(type: "text", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    RateLimitPerMinute = table.Column<int>(type: "integer", nullable: true),
                    RateLimitPerDay = table.Column<int>(type: "integer", nullable: true),
                    TotalCalls = table.Column<long>(type: "bigint", nullable: false),
                    LastUsedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedById = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiKeys_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WebhookLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WebhookConfigId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventType = table.Column<string>(type: "text", nullable: false),
                    PayloadJson = table.Column<string>(type: "text", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    HttpStatusCode = table.Column<int>(type: "integer", nullable: false),
                    ResponseBody = table.Column<string>(type: "text", nullable: true),
                    IsSuccess = table.Column<bool>(type: "boolean", nullable: false),
                    AttemptNumber = table.Column<int>(type: "integer", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    DurationMs = table.Column<long>(type: "bigint", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedById = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebhookLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebhookLogs_WebhookConfigs_WebhookConfigId",
                        column: x => x.WebhookConfigId,
                        principalTable: "WebhookConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SurchargeRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SurchargeId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ConditionsJson = table.Column<string>(type: "text", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ValidTo = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ZoneFromId = table.Column<Guid>(type: "uuid", nullable: true),
                    ZoneToId = table.Column<Guid>(type: "uuid", nullable: true),
                    ApplicableTransportModes = table.Column<string>(type: "text", nullable: true),
                    OverrideValue = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedById = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurchargeRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SurchargeRules_Surcharges_SurchargeId",
                        column: x => x.SurchargeId,
                        principalTable: "Surcharges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SurchargeRules_Zones_ZoneFromId",
                        column: x => x.ZoneFromId,
                        principalTable: "Zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SurchargeRules_Zones_ZoneToId",
                        column: x => x.ZoneToId,
                        principalTable: "Zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TariffLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TariffGridId = table.Column<Guid>(type: "uuid", nullable: true),
                    ZoneFromId = table.Column<Guid>(type: "uuid", nullable: true),
                    ZoneToId = table.Column<Guid>(type: "uuid", nullable: true),
                    MerchandiseTypeId = table.Column<Guid>(type: "uuid", nullable: true),
                    MinWeight = table.Column<decimal>(type: "numeric", nullable: true),
                    MaxWeight = table.Column<decimal>(type: "numeric", nullable: true),
                    MinVolume = table.Column<decimal>(type: "numeric", nullable: true),
                    MaxVolume = table.Column<decimal>(type: "numeric", nullable: true),
                    PricePerKg = table.Column<decimal>(type: "numeric", nullable: true),
                    PricePerM3 = table.Column<decimal>(type: "numeric", nullable: true),
                    PricePerContainer20ft = table.Column<decimal>(type: "numeric", nullable: true),
                    PricePerContainer40ft = table.Column<decimal>(type: "numeric", nullable: true),
                    BasePrice = table.Column<decimal>(type: "numeric", nullable: true),
                    TransitDays = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedById = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TariffLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TariffLines_TariffGrids_TariffGridId",
                        column: x => x.TariffGridId,
                        principalTable: "TariffGrids",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TariffLines_Zones_ZoneFromId",
                        column: x => x.ZoneFromId,
                        principalTable: "Zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TariffLines_Zones_ZoneToId",
                        column: x => x.ZoneToId,
                        principalTable: "Zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Shipments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ShipmentNumber = table.Column<string>(type: "text", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: true),
                    QuoteId = table.Column<Guid>(type: "uuid", nullable: true),
                    Shipment_Origin_Street = table.Column<string>(type: "text", nullable: false),
                    Shipment_Origin_City = table.Column<string>(type: "text", nullable: false),
                    OriginAddress_PostalCode = table.Column<string>(type: "text", nullable: false),
                    OriginAddress_Country = table.Column<string>(type: "text", nullable: false),
                    OriginAddress_State = table.Column<string>(type: "text", nullable: true),
                    OriginAddress_Latitude = table.Column<double>(type: "double precision", nullable: true),
                    OriginAddress_Longitude = table.Column<double>(type: "double precision", nullable: true),
                    Shipment_Dest_Street = table.Column<string>(type: "text", nullable: false),
                    Shipment_Dest_City = table.Column<string>(type: "text", nullable: false),
                    DestinationAddress_PostalCode = table.Column<string>(type: "text", nullable: false),
                    DestinationAddress_Country = table.Column<string>(type: "text", nullable: false),
                    DestinationAddress_State = table.Column<string>(type: "text", nullable: true),
                    DestinationAddress_Latitude = table.Column<double>(type: "double precision", nullable: true),
                    DestinationAddress_Longitude = table.Column<double>(type: "double precision", nullable: true),
                    MerchandiseTypeId = table.Column<Guid>(type: "uuid", nullable: true),
                    WeightKg = table.Column<decimal>(type: "numeric", nullable: true),
                    VolumeM3 = table.Column<decimal>(type: "numeric", nullable: true),
                    ContainerType = table.Column<int>(type: "integer", nullable: true),
                    ContainerCount = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TrackingNumber = table.Column<string>(type: "text", nullable: true),
                    TotalCostHT = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalSurcharges = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalTaxes = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalCostTTC = table.Column<decimal>(type: "numeric", nullable: false),
                    CurrencyCode = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedById = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shipments_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Shipments_MerchandiseTypes_MerchandiseTypeId",
                        column: x => x.MerchandiseTypeId,
                        principalTable: "MerchandiseTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Shipments_Quotes_QuoteId",
                        column: x => x.QuoteId,
                        principalTable: "Quotes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ContractPricings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContractId = table.Column<Guid>(type: "uuid", nullable: false),
                    ZoneFromId = table.Column<Guid>(type: "uuid", nullable: true),
                    ZoneToId = table.Column<Guid>(type: "uuid", nullable: true),
                    TransportMode = table.Column<int>(type: "integer", nullable: true),
                    UseFixedPrice = table.Column<bool>(type: "boolean", nullable: false),
                    FixedPrice = table.Column<decimal>(type: "numeric", nullable: true),
                    DiscountPercent = table.Column<decimal>(type: "numeric", nullable: false),
                    VolumeThreshold = table.Column<decimal>(type: "numeric", nullable: true),
                    VolumeDiscountPercent = table.Column<decimal>(type: "numeric", nullable: true),
                    CurrencyCode = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedById = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractPricings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractPricings_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContractPricings_Zones_ZoneFromId",
                        column: x => x.ZoneFromId,
                        principalTable: "Zones",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ContractPricings_Zones_ZoneToId",
                        column: x => x.ZoneToId,
                        principalTable: "Zones",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "text", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: true),
                    InvoiceDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsSupplierInvoice = table.Column<bool>(type: "boolean", nullable: false),
                    ClientAddress = table.Column<string>(type: "text", nullable: true),
                    SupplierId = table.Column<Guid>(type: "uuid", nullable: true),
                    ShipmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    SupplierName = table.Column<string>(type: "text", nullable: true),
                    ShipmentNumber = table.Column<string>(type: "text", nullable: true),
                    IssueDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TotalHT = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalVAT = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalTTC = table.Column<decimal>(type: "numeric", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "numeric", nullable: false),
                    CurrencyId = table.Column<Guid>(type: "uuid", nullable: true),
                    ExchangeRate = table.Column<decimal>(type: "numeric", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CurrencyCode = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedById = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoices_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Invoices_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Invoices_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Invoices_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TransportSegments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ShipmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Sequence = table.Column<int>(type: "integer", nullable: false),
                    TransportMode = table.Column<int>(type: "integer", nullable: false),
                    SupplierId = table.Column<Guid>(type: "uuid", nullable: true),
                    ZoneFromId = table.Column<Guid>(type: "uuid", nullable: true),
                    ZoneToId = table.Column<Guid>(type: "uuid", nullable: true),
                    DistanceKm = table.Column<decimal>(type: "numeric", nullable: true),
                    EstimatedTransitDays = table.Column<int>(type: "integer", nullable: true),
                    DepartureDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ArrivalDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    BaseCost = table.Column<decimal>(type: "numeric", nullable: false),
                    SurchargesTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalCost = table.Column<decimal>(type: "numeric", nullable: false),
                    CurrencyCode = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedById = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransportSegments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransportSegments_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TransportSegments_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TransportSegments_Zones_ZoneFromId",
                        column: x => x.ZoneFromId,
                        principalTable: "Zones",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TransportSegments_Zones_ZoneToId",
                        column: x => x.ZoneToId,
                        principalTable: "Zones",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CreditNote",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreditNoteNumber = table.Column<string>(type: "text", nullable: false),
                    InvoiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedById = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditNote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditNote_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InvoiceId = table.Column<Guid>(type: "uuid", nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    PaymentMethod = table.Column<int>(type: "integer", nullable: false),
                    Reference = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedById = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InvoiceLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InvoiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPriceHT = table.Column<decimal>(type: "numeric", nullable: false),
                    VATRate = table.Column<decimal>(type: "numeric", nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "numeric", nullable: false),
                    TransportSegmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreditNoteId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedById = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceLines_CreditNote_CreditNoteId",
                        column: x => x.CreditNoteId,
                        principalTable: "CreditNote",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InvoiceLines_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedById", "CreatedDate", "DecimalPlaces", "DeletedDate", "IsActive", "IsDefault", "IsDeleted", "ModifiedBy", "ModifiedById", "ModifiedDate", "Name", "Symbol" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "EUR", null, null, null, 2, null, true, true, false, null, null, null, "Euro", "€" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "USD", null, null, null, 2, null, true, false, false, null, null, null, "Dollar américain", "$" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "TND", null, null, null, 3, null, true, false, false, null, null, null, "Dinar tunisien", "د.ت" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "GBP", null, null, null, 2, null, true, false, false, null, null, null, "Livre sterling", "£" }
                });

            migrationBuilder.InsertData(
                table: "MerchandiseTypes",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedById", "CreatedDate", "DeletedDate", "Description", "HazardousLevel", "IsActive", "IsDeleted", "ModifiedBy", "ModifiedById", "ModifiedDate", "Name", "PriceMultiplier", "RequiresSpecialHandling" },
                values: new object[,]
                {
                    { new Guid("88888888-8888-8888-8888-888888888888"), "GEN001", null, null, null, null, "Standard general cargo with no special requirements", 0, true, false, null, null, null, "General Cargo", 1.0m, false },
                    { new Guid("99999999-9999-9999-9999-999999999999"), "PER002", null, null, null, null, "Temperature-sensitive goods requiring refrigeration or special handling", 0, true, false, null, null, null, "Perishable Goods", 1.5m, true },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "HAZ003", null, null, null, null, "Dangerous goods requiring special permits and handling procedures", 3, true, false, null, null, null, "Hazardous Materials", 2.0m, true }
                });

            migrationBuilder.InsertData(
                table: "Suppliers",
                columns: new[] { "Id", "Address", "Code", "CreatedBy", "CreatedById", "CreatedDate", "DefaultCurrencyCode", "DeletedDate", "Email", "IsActive", "IsDeleted", "ModifiedBy", "ModifiedById", "ModifiedDate", "Name", "Phone", "TaxId" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), "N/A", "DUMMY", null, null, null, "USD", null, null, true, false, null, null, null, "Unknown Supplier", null, null });

            migrationBuilder.InsertData(
                table: "TaxRules",
                columns: new[] { "Id", "AllowExemption", "Code", "Country", "CreatedBy", "CreatedById", "CreatedDate", "DeletedDate", "ExemptionConditions", "IsActive", "IsDeleted", "ModifiedBy", "ModifiedById", "ModifiedDate", "Name", "ReducedRate", "Region", "StandardRate", "SuperReducedRate", "SurchargeId", "ValidFrom", "ValidTo", "ZeroRate" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-1111-1111-1111-aaaaaaaaaaaa"), false, "VAT-FR-20", "FR", "Seed", null, new DateTime(2026, 3, 18, 12, 41, 29, 568, DateTimeKind.Utc).AddTicks(3782), null, null, true, false, null, null, null, "TVA France 20%", 5.5m, null, 20.0m, 2.1m, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0m },
                    { new Guid("aaaaaaaa-2222-2222-2222-aaaaaaaaaaaa"), false, "VAT-FR-55", "FR", "Seed", null, new DateTime(2026, 3, 18, 12, 41, 29, 568, DateTimeKind.Utc).AddTicks(3804), null, null, true, false, null, null, null, "TVA France 5.5% (réduit)", 5.5m, null, 20.0m, 2.1m, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0m },
                    { new Guid("aaaaaaaa-3333-3333-3333-aaaaaaaaaaaa"), false, "VAT-FR-21", "FR", "Seed", null, new DateTime(2026, 3, 18, 12, 41, 29, 568, DateTimeKind.Utc).AddTicks(3811), null, null, true, false, null, null, null, "TVA France 2.1% (super réduit)", 5.5m, null, 20.0m, 2.1m, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0m },
                    { new Guid("aaaaaaaa-4444-4444-4444-aaaaaaaaaaaa"), true, "VAT-TN-19", "TN", "Seed", null, new DateTime(2026, 3, 18, 12, 41, 29, 568, DateTimeKind.Utc).AddTicks(3818), null, "{\"export\": true, \"sectors\": [\"agriculture\", \"education\"]}", true, false, null, null, null, "TVA Tunisie 19%", 7.0m, null, 19.0m, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0m },
                    { new Guid("aaaaaaaa-5555-5555-5555-aaaaaaaaaaaa"), true, "VAT-TN-07", "TN", "Seed", null, new DateTime(2026, 3, 18, 12, 41, 29, 568, DateTimeKind.Utc).AddTicks(3922), null, "{\"sectors\": [\"hotels\", \"restaurants\"]}", true, false, null, null, null, "TVA Tunisie 7% (réduit)", 7.0m, null, 19.0m, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0m },
                    { new Guid("aaaaaaaa-6666-6666-6666-aaaaaaaaaaaa"), false, "VAT-FR-196", "FR", "Seed", null, new DateTime(2026, 3, 18, 12, 41, 29, 568, DateTimeKind.Utc).AddTicks(3931), null, null, false, false, null, null, null, "Ancien taux TVA France 19.6%", 5.5m, null, 19.6m, 2.1m, null, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2013, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m },
                    { new Guid("aaaaaaaa-7777-7777-7777-aaaaaaaaaaaa"), true, "VAT-EXPORT", "FR", "Seed", null, new DateTime(2026, 3, 18, 12, 41, 29, 568, DateTimeKind.Utc).AddTicks(3948), null, "{\"proofOfExport\": true, \"documents\": [\"EX1\", \"invoice\"]}", true, false, null, null, null, "Exportation hors UE", null, null, 20.0m, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 0m }
                });

            migrationBuilder.InsertData(
                table: "Zones",
                columns: new[] { "Id", "Code", "Country", "CreatedBy", "CreatedById", "CreatedDate", "DeletedDate", "Description", "IsActive", "IsDeleted", "ModifiedBy", "ModifiedById", "ModifiedDate", "Name", "Region" },
                values: new object[,]
                {
                    { new Guid("10111111-1111-1111-1111-111111111111"), "FRA", "France", null, null, null, null, null, true, false, null, null, null, "France", null },
                    { new Guid("12111111-1111-1111-1111-111111111111"), "ITA", "Italie", null, null, null, null, null, true, false, null, null, null, "Italie", null },
                    { new Guid("13111111-1111-1111-1111-111111111111"), "LBY", "Libye", null, null, null, null, null, true, false, null, null, null, "Libye", null },
                    { new Guid("14111111-1111-1111-1111-111111111111"), "MAR", "Maroc", null, null, null, null, null, true, false, null, null, null, "Maroc", null },
                    { new Guid("51111111-1111-1111-1111-111111111111"), "DZA", "Algérie", null, null, null, null, null, true, false, null, null, null, "Algérie", null },
                    { new Guid("61111111-1111-1111-1111-111111111111"), "DEU", "Allemagne", null, null, null, null, null, true, false, null, null, null, "Allemagne", null },
                    { new Guid("71111111-1111-1111-1111-111111111111"), "CHN", "Chine", null, null, null, null, null, true, false, null, null, null, "Chine", null },
                    { new Guid("81111111-1111-1111-1111-111111111111"), "ESP", "Espagne", null, null, null, null, null, true, false, null, null, null, "Espagne", null },
                    { new Guid("91111111-1111-1111-1111-111111111111"), "USA", "États-Unis", null, null, null, null, null, true, false, null, null, null, "États-Unis", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_UserId",
                table: "ActivityLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_OwnerId",
                table: "ApiKeys",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractPricings_ContractId",
                table: "ContractPricings",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractPricings_ZoneFromId",
                table: "ContractPricings",
                column: "ZoneFromId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractPricings_ZoneToId",
                table: "ContractPricings",
                column: "ZoneToId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ClientId",
                table: "Contracts",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_SupplierId",
                table: "Contracts",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditNote_InvoiceId",
                table: "CreditNote",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_FromCurrencyId",
                table: "ExchangeRates",
                column: "FromCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_ToCurrencyId",
                table: "ExchangeRates",
                column: "ToCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLines_CreditNoteId",
                table: "InvoiceLines",
                column: "CreditNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLines_InvoiceId",
                table: "InvoiceLines",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_ClientId",
                table: "Invoices",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_CurrencyId",
                table: "Invoices",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_ShipmentId",
                table: "Invoices",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_SupplierId",
                table: "Invoices",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_InvoiceId",
                table: "Payments",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_ClientId",
                table: "Quotes",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_MerchandiseTypeId",
                table: "Quotes",
                column: "MerchandiseTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_ClientId",
                table: "Shipments",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_MerchandiseTypeId",
                table: "Shipments",
                column: "MerchandiseTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_QuoteId",
                table: "Shipments",
                column: "QuoteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SurchargeRules_SurchargeId",
                table: "SurchargeRules",
                column: "SurchargeId");

            migrationBuilder.CreateIndex(
                name: "IX_SurchargeRules_ZoneFromId",
                table: "SurchargeRules",
                column: "ZoneFromId");

            migrationBuilder.CreateIndex(
                name: "IX_SurchargeRules_ZoneToId",
                table: "SurchargeRules",
                column: "ZoneToId");

            migrationBuilder.CreateIndex(
                name: "IX_Surcharges_Code",
                table: "Surcharges",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TariffLines_TariffGridId",
                table: "TariffLines",
                column: "TariffGridId");

            migrationBuilder.CreateIndex(
                name: "IX_TariffLines_ZoneFromId",
                table: "TariffLines",
                column: "ZoneFromId");

            migrationBuilder.CreateIndex(
                name: "IX_TariffLines_ZoneToId",
                table: "TariffLines",
                column: "ZoneToId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxRules_SurchargeId",
                table: "TaxRules",
                column: "SurchargeId");

            migrationBuilder.CreateIndex(
                name: "IX_TransportSegments_ShipmentId",
                table: "TransportSegments",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TransportSegments_SupplierId",
                table: "TransportSegments",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_TransportSegments_ZoneFromId",
                table: "TransportSegments",
                column: "ZoneFromId");

            migrationBuilder.CreateIndex(
                name: "IX_TransportSegments_ZoneToId",
                table: "TransportSegments",
                column: "ZoneToId");

            migrationBuilder.CreateIndex(
                name: "IX_WebhookLogs_WebhookConfigId",
                table: "WebhookLogs",
                column: "WebhookConfigId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityLogs");

            migrationBuilder.DropTable(
                name: "ApiKeys");

            migrationBuilder.DropTable(
                name: "ContractPricings");

            migrationBuilder.DropTable(
                name: "ExchangeRates");

            migrationBuilder.DropTable(
                name: "InvoiceLines");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "SurchargeRules");

            migrationBuilder.DropTable(
                name: "TariffLines");

            migrationBuilder.DropTable(
                name: "TaxRules");

            migrationBuilder.DropTable(
                name: "Tests");

            migrationBuilder.DropTable(
                name: "TransportSegments");

            migrationBuilder.DropTable(
                name: "WebhookLogs");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropTable(
                name: "CreditNote");

            migrationBuilder.DropTable(
                name: "TariffGrids");

            migrationBuilder.DropTable(
                name: "Surcharges");

            migrationBuilder.DropTable(
                name: "Zones");

            migrationBuilder.DropTable(
                name: "WebhookConfigs");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropTable(
                name: "Shipments");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropTable(
                name: "Quotes");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "MerchandiseTypes");
        }
    }
}
