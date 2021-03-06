
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[APIResponses](
	[APIResponseId] [bigint] IDENTITY(1,1) NOT NULL,
	[JSON] [varchar](max) NOT NULL,
	[DateStamp] [datetime] NOT NULL,
	[RequestType] [tinyint] NULL,
 CONSTRAINT [PK_APIResponses] PRIMARY KEY CLUSTERED 
(
	[APIResponseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UserInfo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ASPNetUserId] [nvarchar](128) NOT NULL,
	[UserKey] [varchar](64) NULL,
	[TempUser] [bit] NOT NULL,
	[Currency] [bigint] NOT NULL,
	[CorrectGuesses] [int] NOT NULL,
	[InCorrectGuesses] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[NewGuest]
	@UserKey varchar(64),
	@ASPNetUserId nvarchar(128)
AS
BEGIN
	DECLARE @SUCCESS BIT = 0
	DECLARE @MESSAGE VARCHAR(150) = 'Guest name already taken. Try registering for a real account or try a new name.'

	IF NOT EXISTS(SELECT [ID] FROM [USERINFO] WHERE [USERKEY] = @USERKEY)
	BEGIN
		INSERT INTO [USERINFO]
		([ASPNETUSERID], [USERKEY], [TEMPUSER], [CURRENCY], [CORRECTGUESSES], [INCORRECTGUESSES])
		VALUES
		(@ASPNETUSERID , @USERKEY , 1         , 5000      , 0               , 0                 )
		
		SET @SUCCESS = 1
		SET @MESSAGE = ''
	END

	SELECT
		@SUCCESS AS [SUCCESS],
		@MESSAGE AS [MESSAGE]
END

GO
