/*********************************************************************************************************
* Script Name: sp_DeleteAirParticulateByID
* Coder: Andy
* Date: 2021-01-29
*
*Date 			Vers 	Coder 		Comments
*2021-01-29 	1.0 	Andy		Initial
*2021-02-05     1.1     Jill        Fixed 1 typo
EXEC sp_DeleteAirParticulateByID 2
***********************************************************************************************************/

IF OBJECT_ID('sp_DeleteAirParticulateByID', 'P') IS NOT NULL

DROP PROCEDURE [dbo].[sp_DeleteAirParticulateByID]


SET ANSI_NULLS ON


SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_DeleteAirParticulateByID]

@AirParticulateID INT

AS 

BEGIN TRANSACTION
BEGIN TRY

  
SET NOCOUNT ON  
SET ANSI_WARNINGS OFF  
--SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED  

DELETE FROM tbl_RespParticles
WHERE @AirParticulateID = tbl_RespParticles.RespID

END TRY

BEGIN CATCH

DECLARE @ErMessage NVARCHAR(MAX),
        @ErSeverity INT,
		@ErState INT

SELECT @ErMessage = ERROR_MESSAGE(), @ErSeverity = ERROR_SEVERITY(), @ErState = ERROR_STATE()

IF @@TRANCOUNT > 0
BEGIN
ROLLBACK TRANSACTION

END
RAISERROR(@ErMessage,@ErSeverity,@ErState)

END CATCH

IF @@TRANCOUNT > 0
BEGIN
COMMIT TRANSACTION
END
GO