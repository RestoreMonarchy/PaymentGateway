CREATE TABLE dbo.StorePaymentProviders
(
	StoreId INT NOT NULL CONSTRAINT FK_StorePaymentProviders_StoreId FOREIGN KEY REFERENCES dbo.Stores(Id),
	PaymentProvider VARCHAR(255) NOT NULL,
	JsonParameters NVARCHAR(MAX) NULL,
	IsEnabled BIT NOT NULL,
	UpdateDate DATETIME2(0) NOT NULL CONSTRAINT DF_StorePaymentProviders_UpdateDate DEFAULT SYSDATETIME(),
	CreateDate DATETIME2(0) NOT NULL CONSTRAINT DF_StorePaymentProviders_CreateDate DEFAULT SYSDATETIME(),
	CONSTRAINT PK_StorePaymentProviders PRIMARY KEY (StoreId, PaymentProvider)
)
