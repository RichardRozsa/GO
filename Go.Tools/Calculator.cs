using System;

namespace Go.Tools {
	public class Calculator {
		// -------------------------------------------------------------------
		public string FormatNumber(double number) {
			// TODO: printf("%#20.8g  0x%x", number, (int)number);
			return number.ToString();
		}
		public double Evaluate(string formula) {	// ---Result of _formula.
			if (formula.Length == 0) {
				Syntax();
			}

			_formula = formula;

			// ---Remove the '+' if it's the first character.
			if (_formula.StartsWith("+")) {
				_formula = _formula.Substring(1);
			}

			_pos = -1;
			NextCh();
		    
			var result = AddSubtract();
			if (_ch == EofLine) {
				_errPos = 0;
				return result;
			}
		    
			_errPos = _pos;

			throw new Exception(String.Format(@"Error in expression:
{0}
{1}^
",
				result, "".PadRight(_errPos, ' ')));
		}
		// -------------------------------------------------------------------
		// Val() translates the string parameter into the double parameter.
		// Valid number formats are decimal and C type hex numbers.
		// -------------------------------------------------------------------
		void Val(string s, ref double value) {
			int     i;

			s = s.ToUpper();
		    
			// ---Check for hex or decimal number (0x????).

			_errPos = 0;
			if (s.Length < 2 || s[1] != 'X')
				value = Convert.ToDouble(s);
			else {
				value = 0.0;
				for (i = 2; i < s.Length; i++) {

					// ---Verify that the current character is a valid hex digit.

					if ((HexDigits.IndexOf(s[i]) >= 0) && (s[i] != 'X')) {

						// ---Add in the value of numbers or hex letters.

						if (NumDigits.IndexOf(s[i]) < 0)
							value = (value * 16) + s[i] - 'A' + 10;
						else
							value = (value * 16) + s[i] - '0';
					}
				}
			}
		}
		// -------------------------------------------------------------------
		// Function NextCh returns the next character in the _formula.
		// The variable _pos contains the position and _ch the character.
		// -------------------------------------------------------------------
		void NextCh() {
			do  {
				_pos++;
				if (_pos < _formula.Length)
					_ch = _formula[_pos];
				else
					_ch = EofLine;
			} while (_ch == ' ');
		}
		// -------------------------------------------------------------------
		double Factor(int i) {
			if (i > 0)
				return (i * Factor(i - 1));
			return 1;
		}
		// -------------------------------------------------------------------
		double Final() {
			double  val = 0;
			string  tmp;

			// ---Append '0' to decimal number starting with '.'.

			if (_ch == '.') {
				_formula = _formula.Substring(0, _pos) + "0" + _formula.Substring(_pos);
				_pos--;
				NextCh();
			}

			// ---Process numbers.

			if (NumDigits.IndexOf(_ch) >= 0) {

				// ---Find the beginning and end of the number.

				int start = _pos;

				// ---Skip past all hex digits.

				do  {
					NextCh();
				} while (HexDigits.IndexOf(_ch) >= 0);
		        
				// ---Skip past all decimal digits after a decimal point.

				if (_ch == '.') {
					do  {
						NextCh();
					} while (NumDigits.IndexOf(_ch) >= 0);
				}

				// ---Skip past the 'E' and all decimal digits for scientific notation.

				if (_ch == 'E' || _ch == 'e') {
					NextCh();
					do  {
						NextCh();
					} while (NumDigits.IndexOf(_ch) >= 0);
				}

				// ---Store the value of the number to 'f'.

				tmp = _formula.Substring(start, _pos - start);
				Val(tmp, ref val);
			}

			// ---Process parenthesis.

			else if (_ch == '(') {
				NextCh();
				val = AddSubtract();
				if (_ch == ')')
					NextCh();
				else
					_errPos = _pos;
			}

			// ---Process functions.

			else {
				bool    found = false;
				int     len;
				// for (StandardFunctions eSf = emAbs; eSf <= emFactor && !found; eSf++) {
				for (StandardFunctions eSf = StandardFunctions.EmAbs; eSf <= StandardFunctions.EmFactor && !found; eSf++) {
					len = standardFunctionNames[(int)eSf].Length;
					tmp = _formula.Substring(_pos, len);
					if (tmp.ToUpper() == standardFunctionNames[(int)eSf].ToUpper()) {
						_pos += len - 1;
						NextCh();
						val = Final();
						switch (eSf) {
							case StandardFunctions.EmAbs:		val = Math.Abs(val);	break;
							case StandardFunctions.EmSqrt:		val = Math.Sqrt(val);	break;
							case StandardFunctions.EmSqr:		val = (val * val);		break;
							case StandardFunctions.EmSin:		val = Math.Sin(val);	break;
							case StandardFunctions.EmCos:		val = Math.Cos(val);	break;
							case StandardFunctions.EmArctan:	val = Math.Atan(val);	break;
							case StandardFunctions.EmLn:		val = Math.Log(val);	break;
							case StandardFunctions.EmLog:		val = Math.Log10(val);	break;
							case StandardFunctions.EmExp:		val = Math.Exp(val);	break;
							case StandardFunctions.EmFactor:	val = Factor((int)val);	break;
						}
						found = true;
					}
				}
				if (!found)
					_errPos = _pos;
			}

			return val;
		}
		// -------------------------------------------------------------------
		double Signed() {
			if (_ch == '-') {
				NextCh();
				return -Final();
			}

			return Final();
		}
		// -------------------------------------------------------------------
		double Exponent() {
			double  val;

			val = Signed();
			while (_ch == '^') {
				NextCh();
				val = Math.Exp(Math.Log(val) * Signed());
			}
		    
			return val;
		}
		// -------------------------------------------------------------------
		double MultiplyDivide() {
			double  val;
			char    opr;

			val = Exponent();
			while (_ch == '*' || _ch == '/') {
				opr = _ch;
				NextCh();
				switch (opr) {
					case '*': val *= Exponent(); break;
					case '/': val /= Exponent(); break;
				}
			}

			return val;
		}
		// -------------------------------------------------------------------
		double AddSubtract() {
			double  val;
			char    opr;

			val = MultiplyDivide();
			while (_ch == '+' || _ch == '-') {
				opr = _ch;
				NextCh();
				switch (opr) {
					case '+': val += MultiplyDivide(); break;
					case '-': val -= MultiplyDivide(); break;
				}
			}

			return val;
		}
		// -------------------------------------------------------------------
		public string Syntax() {
			return @"

Functions:
   ABS()
   SQRT()
   SQR()
   SIN()
   COS()
   ARCTAN
   LN()
   LOG()
   EXP()
   FACTOR()
";
		}
		// -------------------------------------------------------------------
		private const char		EofLine     = '\n';
		private const string	NumDigits   = "0123456789";
		private const string	HexDigits  = "0123456789abcdefABCDEFxX";
		private enum StandardFunctions {
			EmAbs, EmSqrt, EmSqr, EmSin, EmCos, EmArctan, EmLn, EmLog, EmExp, EmFactor
		};
		private readonly string[] standardFunctionNames = new[] {
			"ABS", "SQRT", "SQR", "SIN", "COS", "ARCTAN", "LN", "LOG", "EXP", "FACTOR"
		};

		private string	_formula;	// ---Formula to evaluate.
		private int		_pos;		// ---Current position in _formula.
		private char    _ch;		// ---Current character being scanned.
		private int     _errPos;	// ---Location of error in _formula.
	};
}
