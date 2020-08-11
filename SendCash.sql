CREATE DATABASE SendCash
USE SendCash

CREATE TABLE Bank(
	BankId INT PRIMARY KEY IDENTITY,
	BankName VARCHAR(100) NOT NULL,
	BankPhone VARCHAR(20) NOT NULL,
	BankAddress VARCHAR(255) NOT NULL
)

CREATE TABLE Account(
	AccountId BIGINT PRIMARY KEY IDENTITY,
	AccountNumber VARCHAR(20) NOT NULL,
	AccountName VARCHAR(255) NOT NULL,
	BankId INT REFERENCES Bank(BankId) NOT NULL,
	AccountBalance BIGINT NOT NULL
)

CREATE TABLE TransactionHeader(
	TransactionId BIGINT PRIMARY KEY IDENTITY NOT NULL,
	SenderId BIGINT REFERENCES Account(AccountId) NOT NULL,
	TransactionDt DATETIME DEFAULT GETDATE()
)

--TVP
CREATE TYPE TransactionHeaderType AS TABLE(
	SenderId BIGINT,
	TransactionDt DATETIME
)

CREATE TABLE TransactionDetail(
	TransactionDetailId BIGINT PRIMARY KEY IDENTITY,
	TransactionId BIGINT REFERENCES TransactionHeader(TransactionId),
	ReceiverName VARCHAR(255) NOT NULL,
	ReceiverAccountNumber VARCHAR(20) NOT NULL,
	ReceiverBankName VARCHAR(100) NOT NULL,
	TransactionAmount BIGINT NOT NULL,
	isApproved BIT NOT NULL DEFAULT 1,	--default true, approved transaction can be sent, if success, isComplete = 1
	isComplete BIT NOT NULL DEFAULT 0,
	CompleteDt DATETIME,
	UpdatedJson VARCHAR(500)
)

--TVP
CREATE TYPE TransactionDetailType AS TABLE(
	TransactionId BIGINT,
	ReceiverName VARCHAR(255),
	ReceiverAccountNumber VARCHAR(20),
	ReceiverBankName VARCHAR(100),
	TransactionAmount BIGINT,
	isApproved BIT,	--isApproved -> valid -> can be proceed
	isComplete BIT,
	CompleteDt DATETIME,
	UpdatedJson VARCHAR(500) --if operator updates, supervisor can see what changes
)

INSERT INTO Bank VALUES('BCA', '1500888', 'Menara BCA, Jakarta')
INSERT INTO Bank VALUES('Mandiri', '14000', 'Jl. Jenderal Gatot Subroto Kav. 36-38 Jakarta')
INSERT INTO Bank VALUES('BNI', '1500046', 'Jl. Jenderal Sudirman Kav. 1, Jakarta Pusat')
INSERT INTO Bank VALUES('BRI', '1500017', 'Gedung BRI, Jl. Jenderal Sudirman Kav.44-46, Jakarta')
SELECT * FROM Bank

INSERT INTO Account VALUES('6048879712','Elise Grace',1,1735732)
INSERT INTO Account VALUES('1247780999','Jerome Dika',1,5870913)
INSERT INTO Account VALUES('111111111111111','Bryan Wilson',4,15023813)
INSERT INTO Account VALUES('222222222222222','Eric Ock',4,56447942)
INSERT INTO Account VALUES('333333333333333','Lala Vanilla',4,34811900)
INSERT INTO Account VALUES('0238887777','Christian Rodrick',3,1735732)
INSERT INTO Account VALUES('1234245999','Ettan Noel',3,1735732)
INSERT INTO Account VALUES('1111111111111','John Samuel',2,1735732)
INSERT INTO Account VALUES('2222222222222','Jeffry Lim',2,1735732)
INSERT INTO Account VALUES('3333333333333','Maria Lee',2,1735732)
SELECT * FROM Account

INSERT INTO TransactionHeader(SenderId) VALUES(4)
INSERT INTO TransactionHeader(SenderId) VALUES(2)
SELECT * FROM TransactionHeader

INSERT INTO TransactionDetail(TransactionId, ReceiverName, ReceiverAccountNumber, ReceiverBankName, TransactionAmount) VALUES(1, 'Maria Lee', '3333333333333', 'Mandiri',2000000)
INSERT INTO TransactionDetail(TransactionId, ReceiverName, ReceiverAccountNumber, ReceiverBankName, TransactionAmount) VALUES(1, 'Ettan Noel', '1234245999', 'BNI',3750000)
INSERT INTO TransactionDetail(TransactionId, ReceiverName, ReceiverAccountNumber, ReceiverBankName, TransactionAmount) VALUES(2, 'Etan Noel', '1234245999', 'BNI',820000)
INSERT INTO TransactionDetail(TransactionId, ReceiverName, ReceiverAccountNumber, ReceiverBankName, TransactionAmount) VALUES(2, 'Elise Grace', '6048879712', 'BCA',1000000)
INSERT INTO TransactionDetail(TransactionId, ReceiverName, ReceiverAccountNumber, ReceiverBankName, TransactionAmount) VALUES(2, 'Jerome Dika', '1247780999', 'BCA',4100000)
SELECT * FROM TransactionDetail


GO
CREATE PROC ValidateTransaction (@thType TransactionHeaderType READONLY, @tdType TransactionDetailType READONLY)
AS
BEGIN
	--insert header
	INSERT INTO TransactionHeader
	SELECT * FROM @thType
	--insert detail
	INSERT INTO TransactionDetail
	SELECT * FROM @tdType

	--validate account exists
	UPDATE TransactionDetail
	SET isApproved =
	CASE 
		WHEN acc.AccountId IS NOT NULL AND bn.BankID IS NOT NULL
		THEN 1
		ELSE 0
	END
	FROM TransactionDetail dt
		LEFT JOIN Account acc on dt.ReceiverName = acc.AccountName AND dt.ReceiverAccountNumber = acc.AccountNumber
		LEFT JOIN Bank bn on dt.ReceiverBankName = bn.BankName AND acc.BankID = bn.BankId
	WHERE isComplete = 0

	--validate sender's balance
	UPDATE TransactionDetail
	SET isApproved =
	CASE 
		WHEN acc.AccountBalance >= temp.TotalOutgoing
		THEN 1
		ELSE 0
	END
	FROM TransactionHeader th
		LEFT JOIN Account acc ON th.SenderId = acc.AccountId
		JOIN (SELECT TransactionId, TotalOutgoing = SUM(TransactionAmount)
		FROM TransactionDetail
		GROUP BY TransactionId) temp ON temp.TransactionId = th.TransactionId
		JOIN TransactionDetail td ON td.TransactionId = th.TransactionId
	WHERE isComplete = 0
END

GO
CREATE PROC ViewAllTransactions
AS
BEGIN
	SELECT th.TransactionId, TransactionDt, TransactionDetailId, a.AccountName as Sender, a.AccountNumber, b.BankName as [SenderBank], td.ReceiverName, td.ReceiverAccountNumber as [ReceiverAccount], td.ReceiverBankName as [TargetBank], td.TransactionAmount, td.isApproved, td.isComplete
	FROM Bank b
	JOIN Account a
	ON b.BankId = a.BankId
	JOIN TransactionHeader th
	ON a.AccountId = th.SenderId
	JOIN TransactionDetail td
	ON td.TransactionId = th.TransactionId
END

EXEC ViewAllTransactions
