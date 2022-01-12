CREATE PROCEDURE dbo.GetUser
	@Name NVARCHAR(255),
	@Password NVARCHAR(128)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT Id, Name, FirstName, LastName, CreateDate
	FROM dbo.Users
	WHERE Name = @Name
	AND PWDCOMPARE(@Password, PasswordHash) = 1;

	IF @@ROWCOUNT = 0
		RETURN 1;

	RETURN 0;
END;