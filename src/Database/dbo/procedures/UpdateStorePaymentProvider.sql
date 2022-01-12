CREATE PROCEDURE dbo.UpdateStorePaymentProvider
	@StoreId INT,
	@PaymentProvider VARCHAR(255),
	@JsonParameters NVARCHAR(MAX),
	@IsEnabled BIT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	UPDATE dbo.StorePaymentProviders 
	SET 
		JsonParameters = @JsonParameters,
		IsEnabled = @IsEnabled,
		UpdateDate = IIF(JsonParameters <> @JsonParameters OR IsEnabled <> @IsEnabled, SYSDATETIME(), UpdateDate)
	WHERE StoreId = @StoreId AND PaymentProvider = @PaymentProvider;

	IF @@ROWCOUNT = 0
		INSERT INTO dbo.StorePaymentProviders (StoreId, PaymentProvider, JsonParameters, IsEnabled) 
		VALUES (@StoreId, @PaymentProvider, @JsonParameters, @IsEnabled);

	RETURN 0;
END