using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Go {
	interface ITableStructure {
		string DatabaseName	{ get; }
		string OwnerName	{ get; }
		string TableName	{ get; }
		string FullTableName{ get; }
		string FormatStatement(DataRow row);
	};

	class TableInstance {
		protected string StrFld(object field) {
			if		(field == DBNull.Value)
				return "NULL";
			else
				return (string)field;
			/*
			else if	(((string)field).ToUpper() == "NULL")
				return (string)field;
			else if (((string)field).ToUpper() == "GETDATE()")
				return (string)field;
			else
				return "'" + (string)field + "'";
			*/
		}

		protected string NumFld(object field) {
			if		(field == DBNull.Value)
				return "NULL";
			/*
			else if	(((string)field).ToUpper() == "NULL")
				return (string)field;
			*/
			else
				return (string)field;
		}
	};

	// -----------------------------------------------------------------
	// AccountDB
	// -----------------------------------------------------------------

	class AccountDB_Objects : TableInstance, ITableStructure {
		public string DatabaseName	{ get { return "AccountDB"; } }
		public string OwnerName		{ get { return "dbo"; } }
		public string TableName		{ get { return "Objects"; } }
		public string FullTableName	{ get { return DatabaseName + "." + OwnerName + "." + TableName; } }
		public string FormatStatement(DataRow row) {
			return String.Format(@"insert [dbo].[Objects]([Object],[Parent],[Description],[Image]) values ({0},{1},{2},{3})
GO
",
				StrFld(row["Object"]), 
				StrFld(row["Parent"]), 
				StrFld(row["Description"]), 
				StrFld(row["Image"]));
		}
	};

	class AccountDB_Dictionary : TableInstance, ITableStructure {
		public string DatabaseName	{ get { return "AccountDB"; } }
		public string OwnerName		{ get { return "dbo"; } }
		public string TableName		{ get { return "Dictionary"; } }
		public string FullTableName	{ get { return DatabaseName + "." + OwnerName + "." + TableName; } }
		public string FormatStatement(DataRow row) {
			return String.Format(@"insert [dbo].[Dictionary]([Property],[EditType],[FieldSize],[Description],[UsePopup],[Image],[AddToTree],[Privilege],[Category],[Importance]) values ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9})
GO
",
				StrFld(row["Property"]),
				StrFld(row["EditType"]),
				StrFld(row["FieldSize"]),
				StrFld(row["Description"]),
				StrFld(row["UsePopup"]),
				StrFld(row["Image"]),
				StrFld(row["AddToTree"]),
				StrFld(row["Privilege"]),
				StrFld(row["Category"]),
				StrFld(row["Importance"]));
		}
	};

	class AccountDB_BigPopup : TableInstance, ITableStructure {
		public string DatabaseName	{ get { return "AccountDB"; } }
		public string OwnerName		{ get { return "dbo"; } }
		public string TableName		{ get { return "BigPopup"; } }
		public string FullTableName	{ get { return DatabaseName + "." + OwnerName + "." + TableName; } }
		public string FormatStatement(DataRow row) {
			return String.Format(@"insert into BigPopup (Property, Value) values ({0},{1})
",
				StrFld(row["Property"]),
				StrFld(row["Value"]));
		}
	};

	class AccountDB_Layouts : TableInstance, ITableStructure {
		public string DatabaseName	{ get { return "AccountDB"; } }
		public string OwnerName		{ get { return "dbo"; } }
		public string TableName		{ get { return "Layouts"; } }
		public string FullTableName	{ get { return DatabaseName + "." + OwnerName + "." + TableName; } }
		public string FormatStatement(DataRow row) {
			return String.Format(@"insert [dbo].[Layouts]([Object],[Property],[SeqNumber]) values ({0},{1},{2})
GO
",
				StrFld(row["Object"]),
				StrFld(row["Property"]),
				StrFld(row["SeqNumber"]));
		}
	};

	class AccountDB_RoleTypeDescription : TableInstance, ITableStructure {
		public string DatabaseName	{ get { return "AccountDB"; } }
		public string OwnerName		{ get { return "dbo"; } }
		public string TableName		{ get { return "RoleTypeDescription"; } }
		public string FullTableName	{ get { return DatabaseName + "." + OwnerName + "." + TableName; } }
		public string FormatStatement(DataRow row) {
			return String.Format(@"insert [dbo].[RoletypeDescription]([ObjectFrom],[ObjectTo],[RoleType],[DescriptionFrom],[DescriptionTo],[ImageFrom],[ImageTo],Standard) values ({0},{1},{2},{3},{4},{5},{6},{7})
GO
",
				StrFld(row["ObjectFrom"]),
				StrFld(row["ObjectTo"]),
				StrFld(row["RoleType"]),
				StrFld(row["DescriptionFrom"]),
				StrFld(row["DescriptionTo"]),
				StrFld(row["ImageFrom"]),
				StrFld(row["ImageTo"]),
				NumFld(row["Standard"]));
		}
	};

	class AccountDB_CISEventType : TableInstance, ITableStructure {
		public string DatabaseName	{ get { return "AccountDB"; } }
		public string OwnerName		{ get { return "dbo"; } }
		public string TableName		{ get { return "CISEventType"; } }
		public string FullTableName	{ get { return DatabaseName + "." + OwnerName + "." + TableName; } }
		public string FormatStatement(DataRow row) {
			return String.Format(@"IF NOT EXISTS (SELECT * FROM CISEventType WHERE Name={0})
INSERT INTO CISEventType(Name, Description)
VALUES({0}, {1})
",
				StrFld(row["Name"]),
				StrFld(row["Description"]));
		}
	};

	class AccountDB_CISProperty : TableInstance, ITableStructure {
		public string DatabaseName	{ get { return "AccountDB"; } }
		public string OwnerName		{ get { return "dbo"; } }
		public string TableName		{ get { return "CISProperty"; } }
		public string FullTableName	{ get { return DatabaseName + "." + OwnerName + "." + TableName; } }
		public string FormatStatement(DataRow row) {
			return String.Format(@"IF NOT EXISTS (SELECT * FROM CISProperty WHERE Property={0})
INSERT INTO CISProperty(Property)
VALUES({0})
",
				StrFld(row["Property"]));
		}
	};

	// -----------------------------------------------------------------
	// AimeeDB
	// -----------------------------------------------------------------

	class AimeeDB_AlertAction : TableInstance, ITableStructure {
		public string DatabaseName	{ get { return "AimeeDB"; } }
		public string OwnerName		{ get { return "dbo"; } }
		public string TableName		{ get { return "AlertAction"; } }
		public string FullTableName	{ get { return DatabaseName + "." + OwnerName + "." + TableName; } }
		public string FormatStatement(DataRow row) {
			return String.Format(@"INSERT INTO AlertAction (AlertActionID,Description,Standard) VALUES({0},{1},{2})
",
				StrFld(row["AlertActionID"]),
				StrFld(row["Description"]),
				NumFld(row["Standard"]));
		}
	};

	class AimeeDB_AlertType : TableInstance, ITableStructure {
		public string DatabaseName	{ get { return "AimeeDB"; } }
		public string OwnerName		{ get { return "dbo"; } }
		public string TableName		{ get { return "AlertType"; } }
		public string FullTableName	{ get { return DatabaseName + "." + OwnerName + "." + TableName; } }
		public string FormatStatement(DataRow row) {
			if (row["DefaultCrimeType"] == DBNull.Value && row["DefaultCrimeSubType"] == DBNull.Value) {
				return String.Format(@"INSERT INTO AlertType (AlertType,Description,DefaultWorkflowID,DefaultReportType,Standard) VALUES({0},{1},{2},{3},{4})
",
					StrFld(row["AlertType"]),
					StrFld(row["Description"]),
					StrFld(row["DefaultWorkflowID"]),
					StrFld(row["DefaultReportType"]),
					NumFld(row["Standard"]));
			}
			else {
				return String.Format(@"INSERT INTO AlertType (AlertType,Description,DefaultWorkflowID,DefaultReportType,Standard,DefaultCrimeType,DefaultCrimeSubType) VALUES({0},{1},{2},{3},{4},{5},{6})
",
					StrFld(row["AlertType"]),
					StrFld(row["Description"]),
					StrFld(row["DefaultWorkflowID"]),
					StrFld(row["DefaultReportType"]),
					NumFld(row["Standard"]),
					StrFld(row["DefaultCrimeType"]),
					StrFld(row["DefaultCrimeSubType"]));
			}
		}
	};

	class AimeeDB_CrimeType : TableInstance, ITableStructure {
		public string DatabaseName	{ get { return "AimeeDB"; } }
		public string OwnerName		{ get { return "dbo"; } }
		public string TableName		{ get { return "CrimeType"; } }
		public string FullTableName	{ get { return DatabaseName + "." + OwnerName + "." + TableName; } }
		public string FormatStatement(DataRow row) {
			return String.Format(@"IF NOT EXISTS (SELECT 1 FROM CrimeType WHERE [Name]={0} AND Custom = {1})
BEGIN
	INSERT INTO CrimeType (Name, Custom) VALUES({0},{1})
	SET @MA_CrimeTypeId = SCOPE_IDENTITY()
END
ELSE
BEGIN
	SELECT @MA_CrimeTypeId = [ID]
	  FROM CrimeType
	 WHERE [Name] = {0}
END

",
				StrFld(row["Name"]),
				StrFld(row["Custom"]));
		}
	};

	class AimeeDB_CrimeSubType : TableInstance, ITableStructure {
		public string DatabaseName	{ get { return "AimeeDB"; } }
		public string OwnerName		{ get { return "dbo"; } }
		public string TableName		{ get { return "CrimeSubType"; } }
		public string FullTableName	{ get { return DatabaseName + "." + OwnerName + "." + TableName; } }
		public string FormatStatement(DataRow row) {
			return String.Format(@"IF NOT EXISTS (SELECT 1 FROM CrimeSubType WHERE [CrimeTypeID] = @MA_CrimeTypeId AND [Name] = {0})
BEGIN
	INSERT INTO CrimeSubType (CrimeTypeId, Name, Custom) VALUES(@MA_CrimeTypeId,{0},{1})
	SET @MA_CrimeSubTypeId = SCOPE_IDENTITY()
END
ELSE
BEGIN
	SELECT @MA_CrimeSubTypeId = [ID]
	  FROM CrimeSubType
	 WHERE [CrimeTypeID] = @MA_CrimeTypeId
	       AND [Name] = {0}
END
-- no Go statement because we need the declared variables to add the alert types

",
				StrFld(row["Name"]),
				StrFld(row["Custom"]));
		}
	};

	class AimeeDB_AlertTypeAction : TableInstance, ITableStructure {
		public string DatabaseName	{ get { return "AimeeDB"; } }
		public string OwnerName		{ get { return "dbo"; } }
		public string TableName		{ get { return "AlertTypeAction"; } }
		public string FullTableName	{ get { return DatabaseName + "." + OwnerName + "." + TableName; } }
		public string FormatStatement(DataRow row) {
			return String.Format(@"INSERT INTO AlertTypeAction (AlertType,AlertActionID) VALUES({0},{1})
",
				StrFld(row["AlertType"]),
				StrFld(row["AlertActionID"]));
		}
	};

	// -----------------------------------------------------------------
	// ImportDB
	// -----------------------------------------------------------------

	class ImportDB_Normalization : TableInstance, ITableStructure {
		public string DatabaseName	{ get { return "ImportDB"; } }
		public string OwnerName		{ get { return "dbo"; } }
		public string TableName		{ get { return "Normalization"; } }
		public string FullTableName	{ get { return DatabaseName + "." + OwnerName + "." + TableName; } }
		public string FormatStatement(DataRow row) {
			return String.Format(@"insert [dbo].[Normalization]([SeqNumber],[Description],[Type],[PropertyMapping],[SystemProperty],[Store]) values ('1','A_Number','K','Y','Y','y')
GO
",
				StrFld(row["SeqNumber"]),
				StrFld(row["Description"]),
				StrFld(row["Type"]),
				StrFld(row["PropertyMapping"]),
				StrFld(row["SystemProperty"]),
				StrFld(row["Store"]));
		}
	};

	class ImportDB_Processors : TableInstance, ITableStructure {
		public string DatabaseName	{ get { return "ImportDB"; } }
		public string OwnerName		{ get { return "dbo"; } }
		public string TableName		{ get { return "Processors"; } }
		public string FullTableName	{ get { return DatabaseName + "." + OwnerName + "." + TableName; } }
		public string FormatStatement(DataRow row) {
			return String.Format(@"insert [dbo].[Processors]([ProcessorID],[Description],[Status],[Command]) values ({0},{1},{2},{3})
GO
",
				StrFld(row["ProcessorID"]),
				StrFld(row["Description"]),
				StrFld(row["Status"]),
				StrFld(row["Command"]));
		}
	};

	class ImportDB_Streams : TableInstance, ITableStructure {
		public string DatabaseName	{ get { return "ImportDB"; } }
		public string OwnerName		{ get { return "dbo"; } }
		public string TableName		{ get { return "Streams"; } }
		public string FullTableName	{ get { return DatabaseName + "." + OwnerName + "." + TableName; } }
		public string FormatStatement(DataRow row) {
			return String.Format(@"IF NOT EXISTS (SELECT * FROM Streams where [streamID] = {0})
BEGIN
insert [dbo].[Streams]([StreamID],[StreamName],[Object],[Verified],[ProcessorID],[SourceDirectory],[FileSpecification],[MoveDirectory],[MoveAfterImport],[ErrorDirectory],[TargetQueue],[ErrorQueue],[IntegrityCheckDelay],[ExternalIntegrityCheckType],[ExternalIntegrityCheckValues],[QueueSize],[HeaderSize],[RecordSize],[TrailerSize],[Script],[CustomProcessor],[PreCharging],[FocusIndicators],[AllowUpdates],[AcceptZeroByteFiles]) values ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23})
END
",
				StrFld(row["StreamID"]),
				StrFld(row["StreamName"]),
				StrFld(row["Object"]),
				StrFld(row["Verified"]),
				StrFld(row["ProcessorID"]),
				StrFld(row["SourceDirectory"]),
				StrFld(row["FileSpecification"]),
				StrFld(row["MoveDirectory"]),
				StrFld(row["MoveAfterImport"]),
				StrFld(row["ErrorDirectory"]),
				StrFld(row["TargetQueue"]),
				StrFld(row["ErrorQueue"]),
				StrFld(row["IntegrityCheckDelay"]),
				StrFld(row["ExternalIntegrityCheckType"]),
				StrFld(row["ExternalIntegrityCheckValues"]),
				StrFld(row["QueueSize"]),
				StrFld(row["HeaderSize"]),
				StrFld(row["RecordSize"]),
				StrFld(row["TrailerSize"]),
				StrFld(row["Script"]),
				StrFld(row["CustomProcessor"]),
				StrFld(row["PreCharging"]),
				StrFld(row["FocusIndicators"]),
				StrFld(row["AllowUpdates"]),
				StrFld(row["AcceptZeroByteFiles"]));
		}
	};

	class ImportDB_StreamLayouts : TableInstance, ITableStructure {
		public string DatabaseName	{ get { return "ImportDB"; } }
		public string OwnerName		{ get { return "dbo"; } }
		public string TableName		{ get { return "StreamLayouts"; } }
		public string FullTableName	{ get { return DatabaseName + "." + OwnerName + "." + TableName; } }
		public string FormatStatement(DataRow row) {
			return String.Format(@"insert [dbo].[StreamLayouts]([StreamID],[SeqNumber],[Offset],[Length],[Options]) values ('Account','1','1','50','Main=n')
GO
",
				StrFld(row["StreamID"]),
				StrFld(row["SeqNumber"]),
				StrFld(row["Offset"]),
				StrFld(row["Length"]),
				StrFld(row["Options"]));
		}
	};

	// -----------------------------------------------------------------
	// MainDB
	// -----------------------------------------------------------------

	class MainDB_SysProps : TableInstance, ITableStructure {
		public string DatabaseName	{ get { return "MainDB"; } }
		public string OwnerName		{ get { return "dbo"; } }
		public string TableName		{ get { return "SysProps"; } }
		public string FullTableName	{ get { return DatabaseName + "." + OwnerName + "." + TableName; } }
		public string FormatStatement(DataRow row) {
			return String.Format(@"/* {0} - {1} */
INSERT INTO sysprops ([service],[property],[value],[script],[queryid],[tag],[type],[created])
VALUES ({2},{3},{4},{5},{6},{7},{8},{9})

",
				NumFld(row["CommentRiskView"]),
				NumFld(row["CommentDescription"]),
				StrFld(row["Service"]), 
				StrFld(row["Property"]), 
				StrFld(row["Value"]),
				StrFld(row["Script"]),
				StrFld(row["QueryId"]),
				StrFld(row["Tag"]),
				StrFld(row["Type"]),
				StrFld(row["Created"]));
		}
	};

	// -----------------------------------------------------------------
	// MiscDB
	// -----------------------------------------------------------------

	class MiscDB_PropertyDictionary : TableInstance, ITableStructure {
		public string DatabaseName	{ get { return "MiscDB"; } }
		public string OwnerName		{ get { return "dbo"; } }
		public string TableName		{ get { return "PropertyDictionary"; } }
		public string FullTableName	{ get { return DatabaseName + "." + OwnerName + "." + TableName; } }
		public string FormatStatement(DataRow row) {
			return String.Format(@"INSERT INTO PropertyDictionary (Property,Description,Weight,Explanation,Image,Created,Type, Condition) VALUES({0},{1},{2},{3},{4},{5},{6},{7})
",
			StrFld(row["Property"]),
			StrFld(row["Description"]),
			NumFld(row["Weight"]),
			NumFld(row["Explanation"]),
			StrFld(row["Image"]),
			NumFld(row["Created"]),
			StrFld(row["Type"]),
			StrFld(row["Condition"]));
		}
	};

	// -----------------------------------------------------------------
	// Finance
	// -----------------------------------------------------------------

	class Finance_AlertType : TableInstance, ITableStructure {
		public string DatabaseName	{ get { return "Finance"; } }
		public string OwnerName		{ get { return "dbo"; } }
		public string TableName		{ get { return "AlertType"; } }
		public string FullTableName	{ get { return DatabaseName + "." + OwnerName + "." + TableName; } }
		public string FormatStatement(DataRow row) {
			return String.Format(@"INSERT INTO AlertType (id,Name,Description,RiskViewName,Active,ScanTop,Modified,Standard) VALUES({0},{1},{2},{3},{4},{5},{6},{7})
",
				StrFld(row["ID"]),
				StrFld(row["Name"]),
				StrFld(row["Description"]),
				StrFld(row["RiskViewName"]),
				NumFld(row["Active"]),
				StrFld(row["ScanTop"]),
				NumFld(row["Modified"]),
				NumFld(row["Standard"]));
		}
	};

	class Finance_AlertDestinationTable : TableInstance, ITableStructure {
		public string DatabaseName	{ get { return "Finance"; } }
		public string OwnerName		{ get { return "dbo"; } }
		public string TableName		{ get { return "AlertDestinationTable"; } }
		public string FullTableName	{ get { return DatabaseName + "." + OwnerName + "." + TableName; } }
		public string FormatStatement(DataRow row) {
			return String.Format(@"INSERT INTO AlertDestinationTable (id,AlertTypeID,Product,Name,SequenceNr,HandleExisting) VALUES({0},{1},{2},{3},{4},{5})
",
				StrFld(row["ID"]),
				StrFld(row["AlertTypeId"]),
				StrFld(row["Product"]),
				StrFld(row["Name"]),
				StrFld(row["SequenceNr"]),
				StrFld(row["HandleExisting"]));
		}
	};

	class Finance_AlertTypeFieldLink : TableInstance, ITableStructure {
		public string DatabaseName	{ get { return "Finance"; } }
		public string OwnerName		{ get { return "dbo"; } }
		public string TableName		{ get { return "AlertTypeFieldLink"; } }
		public string FullTableName	{ get { return DatabaseName + "." + OwnerName + "." + TableName; } }
		public string FormatStatement(DataRow row) {
			return String.Format(@"INSERT INTO AlertTypeFieldLink (AlertDestinationTableID,DestinationField,SourceExpression,IsKey,Fixed) VALUES({0},{1},{2},{3},{4})
",
				StrFld(row["AlertDestinationTableID"]),
				StrFld(row["DestinationField"]),
				StrFld(row["SourceExpression"]),
				NumFld(row["IsKey"]),
				NumFld(row["Fixed"]));
		}
	};

	class Finance_AlertRule : TableInstance, ITableStructure {
		public string DatabaseName	{ get { return "Finance"; } }
		public string OwnerName		{ get { return "dbo"; } }
		public string TableName		{ get { return "AlertRule"; } }
		public string FullTableName	{ get { return DatabaseName + "." + OwnerName + "." + TableName; } }
		public string FormatStatement(DataRow row) {
			return String.Format(@"INSERT INTO AlertRule (id,Name,Description,Product,[Database],Statement,Standard,RuleTypeID,TableName) 	values({0},	{1},		{2},	{3},{4},			{5}, {6},{7},{8})
",
				StrFld(row["ID"]),
				StrFld(row["Name"]),
				StrFld(row["Description"]),
				StrFld(row["Product"]),
				StrFld(row["Database"]),
				StrFld(row["Statement"]),
				NumFld(row["Standard"]),
				NumFld(row["RuleTypeId"]),
				StrFld(row["Tablename"]));
		}
	};

	class Finance_AlertTypeRule : TableInstance, ITableStructure {
		public string DatabaseName	{ get { return "Finance"; } }
		public string OwnerName		{ get { return "dbo"; } }
		public string TableName		{ get { return "AlertTypeRule"; } }
		public string FullTableName	{ get { return DatabaseName + "." + OwnerName + "." + TableName; } }
		public string FormatStatement(DataRow row) {
			return String.Format(@"INSERT INTO AlertTypeRule (AlertTypeID, AlertRuleID) VALUES ({0}, {1})
",
				StrFld(row["AlertTypeId"]),
				StrFld(row["AlertRuleId"]));
		}
	};

	class Finance_ArtefactInfo : TableInstance, ITableStructure {
		public string DatabaseName	{ get { return "Finance"; } }
		public string OwnerName		{ get { return "dbo"; } }
		public string TableName		{ get { return "ArtefactInfo"; } }
		public string FullTableName	{ get { return DatabaseName + "." + OwnerName + "." + TableName; } }
		public string FormatStatement(DataRow row) {
			return String.Format(@"INSERT INTO ArtefactInfo (Object,ID,FunctionalDescription,version) VALUES ({0},{1},{2},{3})
",					
				StrFld(row["Object"]),
				StrFld(row["ID"]),
				StrFld(row["FunctionalDescription"]),
				StrFld(row["Version"]));
		}
	};

	class Finance_AlertTypeRiskViewParameter : TableInstance, ITableStructure {
		public string DatabaseName	{ get { return "Finance"; } }
		public string OwnerName		{ get { return "dbo"; } }
		public string TableName		{ get { return "AlertTypeRiskViewParameter"; } }
		public string FullTableName	{ get { return DatabaseName + "." + OwnerName + "." + TableName; } }
		public string FormatStatement(DataRow row) {
			return String.Format(@"INSERT INTO AlertTypeRiskViewParameter (AlertTypeID,Name,Value) VALUES({0},{1},{2})
",
				StrFld(row["AlertTypeID"]),
				StrFld(row["Name"]),
				StrFld(row["Value"]));
		}
	};

	class Finance_AlertTypeRuleParameter : TableInstance, ITableStructure {
		public string DatabaseName	{ get { return "Finance"; } }
		public string OwnerName		{ get { return "dbo"; } }
		public string TableName		{ get { return "AlertTypeRuleParameter"; } }
		public string FullTableName	{ get { return DatabaseName + "." + OwnerName + "." + TableName; } }
		public string FormatStatement(DataRow row) {
			return String.Format(@"IF NOT EXISTS (SELECT * FROM AlertTypeRuleParameter where AlertTypeID = {0} and AlertRuleID = {1})
BEGIN
	INSERT INTO AlertTypeRuleParameter (AlertTypeID,AlertRuleID,Name,Value) VALUES({0},{1},{2},{3})
END

",
				StrFld(row["AlertTypeID"]),
				StrFld(row["AlertTypeID"]),
				StrFld(row["Name"]),
				StrFld(row["Value"]));
		}
	};
}
