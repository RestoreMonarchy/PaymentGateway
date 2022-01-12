CREATE PROCEDURE dbo.CreateUser
	@Name NVARCHAR(255),
	@FirstName NVARCHAR(255),
	@LastName NVARCHAR(255),
	@Password NVARCHAR(128)
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO dbo.Users (Name, PasswordHash, FirstName, LastName)
	OUTPUT
		INSERTED.Id,
		INSERTED.Name,
		INSERTED.FirstName,
		INSERTED.LastName,
		INSERTED.CreateDate
	VALUES (@Name, PWDENCRYPT(@Password), @FirstName, @LastName);

	RETURN 0;
END;
-- EXEC dbo.CreateUser @Name = 'MCrow', @FirstName = 'Michael', @LastName = 'Crow', @Password = '';