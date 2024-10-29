DROP PROCEDURE IF EXISTS sp_insert_outputs
GO

CREATE PROCEDURE sp_insert_outputs(@P_DATA dbo.TABLE_OUTPUT_DETAIL READONLY)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @inserted_ids TABLE(ID BIGINT);

	BEGIN TRANSACTION trn;

	BEGIN TRY
		INSERT INTO [dbo].[OUTPUT_DETAIL]
        (
			[COMPANYCODE]
			,[BILL_DESC]
			,[BILL_NO]
			,[SITEID]
			,[TYPE]
			,[unit_AMT]
			,[unit_AMT_NGN]
			,[Qty]
			,[AMT]
			,[AMT_NGN]
			,[AMT_USD]
			,[AMT_USD_PORTION]
			,[AMT_USD_PORTION_NGN]
			,[CURRENCY]
			,[BILL_DATE]
			,[T_ID]
			,[RECUSER]
			,[recdate]
			,[RFI_DATE]
			,[IFRS_RFI_DATE]
			,[IFRS_END]
		)
		OUTPUT INSERTED.ID INTO @inserted_ids(ID)
		SELECT t.COMPANYCODE
				,t.BILL_DESC
				,t.BILL_NO
				,t.SITEID
				,t.TYPE
				,t.unit_AMT
				,t.unit_AMT_NGN
				,t.Qty
				,t.AMT
				,t.AMT_NGN
				,t.AMT_USD
				,t.AMT_USD_PORTION
				,t.AMT_USD_PORTION_NGN
				,t.CURRENCY
				,t.BILL_DATE
				,t.T_ID
				,t.RECUSER
				,t.recdate
				,t.RFI_DATE
				,t.IFRS_RFI_DATE
				,t.IFRS_END
			FROM @P_DATA t		

		COMMIT TRANSACTION trn;
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION trn;
	END CATCH;
END
go