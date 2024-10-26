-- =============================================
-- Author:		Jonathan Sun
-- Create date: 10/11/2022
-- Description:	strip domain name from user name before execute remote procedure to get facility list for the user.
-- =============================================
CREATE PROCEDURE shared.sp_UserAccessLevel
	-- Add the parameters for the stored procedure here
	@userName varchar(255)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @originalName varchar(255) = @userName
	declare @pattern varchar(10) = '%\\%'
	declare @cleanName varchar(255)

	if (PATINDEX( @pattern, @originalName) <> 0)
		begin
			declare @reverseString varchar(10) = substring(REVERSE(@originalName),1, len(@originalName))

			--select @userName = REVERSE(SUBSTRING(REVERSE(@userName), 1, 
   --        CHARINDEX(@pattern, REVERSE(@userName), 1) - 1))
			print 'originalName = ' + @originalName 
			print 'reverseString = ' + @reverseString

			set @cleanName = REVERSE(@reverseString)
			print 'cleanName = ' + @cleanName
			
		end


    -- Insert statements for procedure here
	EXEC [VHAAUSDB2.vha.med.va.gov].MasterReports.[Apps].[uspVSSCMain_SelectAccessInformationFromNSSD]  @cleanName;
	return;
END
