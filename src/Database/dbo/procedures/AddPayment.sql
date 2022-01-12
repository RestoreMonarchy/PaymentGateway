CREATE PROCEDURE dbo.AddPayment
	@StoreId INT,
	@Provider NVARCHAR(255), 
	@Custom NVARCHAR(255), 
	@Amount DECIMAL(9,2),
	@Currency CHAR(3), 
	@Receiver NVARCHAR(255),
	@Items NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;
		
	DECLARE @paymentId INT;

	BEGIN TRAN;
	INSERT INTO dbo.Payments (StoreId, Provider, Custom, Amount, Currency, Receiver) 
	VALUES (@StoreId, @Provider, @Custom, @Amount, @Currency, @Receiver);

	SET @paymentId = SCOPE_IDENTITY();

	INSERT INTO dbo.PaymentItems (PaymentId, Name, Quantity, Price)
	SELECT @paymentId, Name, Quantity, Price
	FROM OPENJSON(@Items)
	WITH (Name NVARCHAR(255), Quantity INT, Price DECIMAL(9,2));

	COMMIT;

	SELECT PublicId FROM dbo.Payments WHERE Id = @paymentId;
	
	RETURN 0;
END
