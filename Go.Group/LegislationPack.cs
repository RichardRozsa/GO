using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Go.Tools;

namespace Go {
	class LegislationPackGenerateXml {
		public LegislationPackGenerateXml(string eraseV4Dir, string legislationPack, bool validateMode) {
			SortedList	libraryRiskViews			= new SortedList();
			SortedList	alertTypes					= new SortedList();
			SortedList	mainDbRiskViews				= new SortedList();
			SortedList	miscDbPropertyDictionary	= new SortedList();
			SortedList	financeAlertBase			= new SortedList();

			DataSet ds = new DataSet("LegislationPack");

			ParseLibraryRiskViewsXml(eraseV4Dir, ref libraryRiskViews, validateMode);

			// Not delivered: Standard Library Risk Views\Alert Definitions Parameter Values.xls
			
			new ParseAccountDbScriptSql(eraseV4Dir, validateMode, ref ds);
			new ParseAimeeDbBaseSql(eraseV4Dir, validateMode, ref ds);
			new ParseImportDbScriptSql(eraseV4Dir, validateMode, ref ds);
			new ParseMainDbScriptSql(eraseV4Dir, validateMode, ref ds);
			new ParseMiscDbScriptSql(eraseV4Dir, validateMode, ref ds);

			new ParseFinanceAlertBaseSql(eraseV4Dir, validateMode, ref ds);
			new ParseFinanceAlertParametersSql(eraseV4Dir, validateMode, ref ds);

			// Standard Library Risk Views\Best Practices\Data Requirements [BP].doc
			// Standard Library Risk Views\Best Practices\Productsheet [BP] Best Practices.doc

			// Standard Library Risk Views\Best Practices\Deployment\AggregationDB script.sql
			// Standard Library Risk Views\Best Practices\Deployment\AIMEEDB Country Specifics.sql
			// Standard Library Risk Views\Best Practices\Deployment\Finance Alert Parameters.sql
			// Standard Library Risk Views\Best Practices\Deployment\MainDB customnodes script.sql
			// Standard Library Risk Views\Best Practices\Deployment\MiscDB Thresholds script.sql

			// Standard Library Risk Views\Best Practices\Deployment\IMPORT SERVICE (NEW IMPORTER)\transaction\flow.xml

		}

		class LibraryRiskViewGroup {
			public LibraryRiskViewGroup() {
				name		= null;
				description = null;
			}
			public LibraryRiskViewGroup(LibraryRiskViewGroup lrvg) {
				name		= lrvg.name;
				description = lrvg.description;
			}
			public string name			= null;
			public string description	= null;
		};

		class LibraryRiskView {
			public LibraryRiskView() {
				groups = new ArrayList();
			}
			public LibraryRiskView(LibraryRiskView lrv) {
				groups = new ArrayList();
				for (int i = 0; i < lrv.groups.Count; i++)
					groups.Add(new LibraryRiskViewGroup((LibraryRiskViewGroup)lrv.groups[i]));
				riskView	= lrv.riskView;
				description = lrv.description;
				options		= lrv.options;
			}
			public ArrayList groups		= null;
			public string riskView		= null;
			public string description	= null;
			public string options		= null;
		};

		private void	ParseLibraryRiskViewsXml(string eraseV4Dir, ref SortedList libraryRiskViews, bool validateMode) {
			string filename = eraseV4Dir + @"\Solutions\NetEconomy.ComplianceManager\LibraryRiskViews.xml";
			if (validateMode) {
				DialogResult response = MessageBox.Show("Verify parsing of " + filename + "?", "ParseLegislationPack", MessageBoxButtons.YesNoCancel);
				if (response == DialogResult.Cancel)
					throw new Exception("Stopped at user request");
				if (response == DialogResult.No)
					return;
			}

			TextFileContents tfc = new TextFileContents(filename);
			if (!tfc.Exists)
				throw new Exception("Missing file " + filename);

			Regex regexGroupStart = new Regex(@"^[ \t]*<group[ \t]+name[ \t]*=[ \t]*""(?<Name>.*)"".*description[ \t]*=[ \t]*""(?<Description>.*)"".*image[ \t]*=", 
				RegexOptions.Compiled);
			Regex regexGroupEnd = new Regex(@"^[ \t]*</group>", 
				RegexOptions.Compiled);
			Regex regexInstantiate = new Regex(@"^[ \t]*<instantiate[ \t]+name[ \t]*=[ \t]*""v3Exec""[ \t]+description[ \t]*=[ \t]*""(?<Description>.*)""[ \t]+params[ \t]*=[ \t]*""[Dd]atabase=.*&amp;ID=(?<RiskView>RV.*)""[ \t]+options[ \t]*=[ \t]*""(?<Options>.*)""[ \t]+image[ \t]*=", 
				RegexOptions.Compiled);

			LibraryRiskView			libRV = new LibraryRiskView();
			LibraryRiskViewGroup	group = new LibraryRiskViewGroup();

			ArrayList groups = new ArrayList();

			string line = tfc.GetFirstLine();
			while (line != null) {
				if (regexGroupStart.IsMatch(line)) {
					group.name			= regexGroupStart.Match(line).Result("${Name}");
					group.description	= regexGroupStart.Match(line).Result("${Description}");
					groups.Add(new LibraryRiskViewGroup(group));
				}
				else if (regexGroupEnd.IsMatch(line)) {
					if (groups.Count > 0)
						groups.RemoveAt(groups.Count - 1);
				}
				else if (regexInstantiate.IsMatch(line)) {
					libRV.groups.Clear();
					for (int i = 0; i < groups.Count; i++)
						libRV.groups.Add(new LibraryRiskViewGroup((LibraryRiskViewGroup)groups[i]));
					libRV.riskView		= regexInstantiate.Match(line).Result("${RiskView}");
					libRV.description	= regexInstantiate.Match(line).Result("${Description}");
					libRV.options		= regexInstantiate.Match(line).Result("${Options}");

					libraryRiskViews.Add(libRV.riskView, new LibraryRiskView(libRV));
				}

				line = tfc.GetNextLine();
			}
		}

		class ParseDatabaseScripts {
			public ParseDatabaseScripts(string eraseV4Dir, bool validateMode) {
				_validateMode	= validateMode;

				_filename = eraseV4Dir + Path;
				if (validateMode) {
					DialogResult response = MessageBox.Show("Verify parsing of " + _filename + "?", "ParseLegislationPack", MessageBoxButtons.YesNoCancel);
					if (response == DialogResult.Cancel)
						throw new Exception("Stopped at user request");
					if (response == DialogResult.No)
						return;
				}

				_tfc = new TextFileContents(_filename);
				if (!_tfc.Exists)
					throw new Exception("Missing file " + _filename);

				if (validateMode)
					_test = new StringBuilder();
			}
			protected void Cleanup() {
				if (_validateMode) {
					Shell.Merge(_filename, FileTools.WriteTempFile("test.sql", _test.ToString()), true, false);
				}
			}
			virtual protected string Path { get { return null; } }
			protected void LoadData(ref DataSet ds, ITableStructure tableType) {
				DataTable dt = ds.Tables.Add(tableType.FullTableName);
				_tfc.GetSqlInsertStatements(ref dt);

				if (_validateMode) {
					for (int r = 0; r < dt.Rows.Count; r++) {
						_test.Append(tableType.FormatStatement(dt.Rows[r]));
					}
				}
			}

			protected string			_filename		= null;
			protected bool				_validateMode	= false;
			protected TextFileContents	_tfc			= null;
			protected StringBuilder		_test			= null;
		};

		// -----------------------------------------------------------------
		// Script parsing classes
		// -----------------------------------------------------------------

		class ParseAccountDbScriptSql : ParseDatabaseScripts {
			public ParseAccountDbScriptSql(string eraseV4Dir, bool validateMode, ref DataSet ds) : base(eraseV4Dir, validateMode) {
				LoadData(ref ds, new AccountDB_Objects());
				LoadData(ref ds, new AccountDB_Dictionary());
				LoadData(ref ds, new AccountDB_BigPopup());
				LoadData(ref ds, new AccountDB_Layouts());
				LoadData(ref ds, new AccountDB_RoleTypeDescription());
				LoadData(ref ds, new AccountDB_CISEventType());
				LoadData(ref ds, new AccountDB_CISProperty());
				Cleanup();
			}
			protected override string Path { get { return @"\Standard Library Risk Views\Deployment\AccountDB script.sql"; } }
		};

		class ParseAimeeDbBaseSql : ParseDatabaseScripts {
			public ParseAimeeDbBaseSql(string eraseV4Dir, bool validateMode, ref DataSet ds) : base(eraseV4Dir, validateMode) {
				LoadData(ref ds, new AimeeDB_AlertAction());
				LoadData(ref ds, new AimeeDB_AlertType());
				LoadData(ref ds, new AimeeDB_CrimeType());
				LoadData(ref ds, new AimeeDB_CrimeSubType());
				LoadData(ref ds, new AimeeDB_AlertTypeAction());
				Cleanup();
			}
			protected override string Path { get { return @"\Standard Library Risk Views\Deployment\AIMEEDB Base.sql"; } }
		};

		class ParseImportDbScriptSql : ParseDatabaseScripts {
			public ParseImportDbScriptSql(string eraseV4Dir, bool validateMode, ref DataSet ds) : base(eraseV4Dir, validateMode) {
				LoadData(ref ds, new ImportDB_Normalization());
				LoadData(ref ds, new ImportDB_Processors());
				LoadData(ref ds, new ImportDB_Streams());
				LoadData(ref ds, new ImportDB_StreamLayouts());
				Cleanup();
			}
			protected override string Path { get { return @"\Standard Library Risk Views\Deployment\AIMEEDB Base.sql"; } }
		};

		class ParseMainDbScriptSql : ParseDatabaseScripts {
			public ParseMainDbScriptSql(string eraseV4Dir, bool validateMode, ref DataSet ds) : base(eraseV4Dir, validateMode) {
				LoadData(ref ds, new MainDB_SysProps());
				Cleanup();
			}
			protected override string Path { get { return @"\Standard Library Risk Views\Deployment\AIMEEDB Base.sql"; } }
		};

		class ParseMiscDbScriptSql : ParseDatabaseScripts {
			public ParseMiscDbScriptSql(string eraseV4Dir, bool validateMode, ref DataSet ds) : base(eraseV4Dir, validateMode) {
				LoadData(ref ds, new MiscDB_PropertyDictionary());
				Cleanup();
			}
			protected override string Path { get { return @"\Standard Library Risk Views\Deployment\MiscDB script.sql"; } }
		};

		class ParseFinanceAlertBaseSql : ParseDatabaseScripts {
			public ParseFinanceAlertBaseSql(string eraseV4Dir, bool validateMode, ref DataSet ds) : base(eraseV4Dir, validateMode) {
				LoadData(ref ds, new Finance_AlertType());
				LoadData(ref ds, new Finance_AlertDestinationTable());
				LoadData(ref ds, new Finance_AlertTypeFieldLink());
				LoadData(ref ds, new Finance_AlertRule());
				LoadData(ref ds, new Finance_AlertTypeRule());
				LoadData(ref ds, new Finance_ArtefactInfo());
				LoadData(ref ds, new Finance_AlertType());
				Cleanup();
			}
			protected override string Path { get { return @"\Standard Library Risk Views\Deployment\Alert Definitions\Finance Alert Base.sql"; } }
		};

		class ParseFinanceAlertParametersSql : ParseDatabaseScripts {
			public ParseFinanceAlertParametersSql(string eraseV4Dir, bool validateMode, ref DataSet ds) : base(eraseV4Dir, validateMode) {
				LoadData(ref ds, new Finance_AlertTypeRiskViewParameter());
				LoadData(ref ds, new Finance_AlertTypeRuleParameter());
				Cleanup();
			}
			protected override string Path { get { return @"\Standard Library Risk Views\Deployment\Alert Definitions\Finance Alert Parameters.sql"; } }
		};

//		DataRow[] dr = myDataSet.Tables[0].Select("id < 2", "id");
	};
}
