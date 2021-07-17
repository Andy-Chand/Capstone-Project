/*
ScriptName: sp_GenusByAnimalCommonName
Coder: Allan Vickers
Date: 2021-03-01

vers     Date                    Coder       Issue
1.0      2021-03-01              Allan       Initial

EXEC sp_GenusByAnimalCommonName "Boreal Chickadee"
*/

USE [DB_Radagast]
GO

IF OBJECT_ID('sp_GenusByAnimalCommonName', 'P') IS NOT NULL

DROP PROCEDURE [dbo].[sp_GenusByAnimalCommonName]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_GenusByAnimalCommonName]
 
@AnimalName NVARCHAR(MAX)


AS  


BEGIN TRANSACTION
BEGIN TRY

  
SET NOCOUNT ON  
SET ANSI_WARNINGS OFF  
--SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED  

--OLD CODE
--SELECT RA.RareAnimalID, RA.Latitude, RA.Longitude, RA.SiteNumber, RA.DateYear, RA.DateMonth, RA.DateDay, RA.SpeciesCode, RA.CommonName, RP.RespID
--FROM tbl_RareAnimal RA
--LEFT JOIN tbl_RespParticles RP ON RP.RespID = RA.RespParticlesFK
--WHERE RA.Species = @AnimalSpecies


SELECT DISTINCT RA.SpeciesCode, RA.Genus, RA.Species, RA.CommonName
FROM tbl_RareAnimal RA

WHERE RA.CommonName = @AnimalName



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