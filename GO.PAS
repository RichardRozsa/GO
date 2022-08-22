{$M $1000, 0, 0}        { 4K stack, no heap }

(*
 * GO directory aliasing utility.  
 *
 * Modification LOG:
 *
 * v1.3 Richard Rozsa
 *      Added support for leading white space before labels and aliases.
 *      Cleaned up and commented code.
 *      Added include file support (filename prefixed by '!').
 *
 * v1.2 Richard Rozsa
 *      Added support for 'home' directory.
 *      Added command line switch processing (to get help screen).
 *
 * v1.1 Georges Rabahni
 *      Added support for aliased variables (placed in angle brackets)
 *
 * v0.1 Emanuel Mashian
 *      Original Coding
 *
 *)

(* ----------------------------------------------------------- *)
program GO;
(* ----------------------------------------------------------- *)
uses
   DOS;
(* ----------------------------------------------------------- *)
type
   commandtype = record
      str  : string;
      proc : pointer;
   end;
(* ----------------------------------------------------------- *)
var
   tableFile      : Text;
   tableFileName  : string;
   includeFile    : Text;
   len            : integer;
   count          : integer;
   done           : boolean;
   line           : string;
   alias          : string;
   match          : string;
   cDir           : string;
   command        : commandtype;
   helpLvl        : integer;
   isInclude      : boolean;

   sub            : boolean;
   subst          : string;
   subLn          : string;
   subWrd         : string;
(* ----------------------------------------------------------- *)
procedure Help;

begin
   Writeln( 'GO v1.3 Directory Navagation Utility' );
   Writeln( 'Copyright (c) 1990,1991  Nobody in particular.' );
   Writeln;
   Writeln( 'USAGE: GO <alias>  to change drive and directory' );
   Writeln( '       GO -h       this help screen' );
   Writeln;
   Writeln( 'Requires GO.TBL in same directory as GO.EXE' );
end;
(* ----------------------------------------------------------- *)
function GetPath : string;

var
   endPos   : integer;
   fileName : string;

begin

   { ---Return the executable name without the '.exe' }

   fileName  := ParamStr( 0 );
   endPos    := Length( fileName );
   while ( endPos >= 1 ) and ( fileName[endPos] <> '.' ) do
      endPos := endPos -1;

   GetPath   := Copy( ParamStr( 0 ), 1, endPos );
end;
(* ----------------------------------------------------------- *)
function FileExists( fileName : string ) : Boolean;

var
   f : file;

begin
   {$I-}
   Assign( f, FileName );
   Reset( f );
   Close( f );
   {$I+}
   FileExists := ( IOResult = 0 ) and ( FileName <> '' );
end;
(* ----------------------------------------------------------- *)
function WhiteSpace( isSpace : char ) : Boolean;

begin
   WhiteSpace := ( isSpace = ' ' ) or ( isSpace = chr( 9 ) );
end;
(* ----------------------------------------------------------- *)
function Upper( anyStr : string ) : string;

var
   n : integer;

begin
   for n := 1 to Length( anyStr ) do
      Upper[n] := upcase( anyStr[n] );
end;
(* ----------------------------------------------------------- *)
function Lower( anyStr : string ) : string;

var
   n : integer;

begin
   for n := 1 to Length( anyStr ) do
   begin
      if ( anyStr[n] >= 'A' ) and ( anyStr[n] <= 'Z' ) then
         Inc( anyStr[n], 32 );
   end;

   Lower := anyStr;
end;
(* ----------------------------------------------------------- *)
function LTrim( anyStr : string ) : string;

begin
   while ( Length( anyStr ) > 0 ) and ( WhiteSpace( anyStr[1] ) ) do
      anyStr := Copy( anyStr, 2, Length( anyStr ) -1 );

   LTrim := Copy( anyStr, 1, Length( anyStr ) );
end;
(* ----------------------------------------------------------- *)
function GetSwitch( anyStr : string ) : integer;

begin

   { ---Default - no switch }

   GetSwitch := 0;

   { ---Help switch }

   if anyStr = '?'  then
      GetSwitch := 1;
   if anyStr = '-?' then
      GetSwitch := 1;
   if anyStr = '/?' then
      GetSwitch := 1;
   if anyStr = '-H' then
      GetSwitch := 1;
   if anyStr = '/H' then
      GetSwitch := 1;

end;
(* ----------------------------------------------------------- *)
procedure ReadALine;

begin

   if ( isInclude ) then
   begin
      Readln( includeFile, line );
   end
   else
   begin
      Readln( tableFile, line );
   end;

   line := LTrim( line );
   line := Upper( line );
end;
(* ----------------------------------------------------------- *)
procedure OpenIncludeFile;

begin
   { ---Make sure there isn't an include file already open }

   if isInclude then
   begin
      Writeln( 'ERROR: An include file can''t open another include file' );
      Halt( 1 );
   end;

   isInclude := True;

   match := LTrim( line );
   match := copy( match, 2, Length( match ) );
   match := LTrim( match );

   { ---Attempt to open include file }
   if FileExists( match ) then 
   begin
      Assign( includeFile, match );
      Reset(  includeFile );
   end
   else 
   begin
      isInclude := False;

      { ---///
      Writeln( 'ERROR: No table found' );
      Halt( 1 );
      }
   end;
end;
(* ----------------------------------------------------------- *)
procedure ScanGoTable;

begin

   len       := Length( alias );
   isInclude := False;
   done      := False;

   while ( ( not eof( tableFile ) ) and ( not done ) ) do
   begin

      { ---Read a line, either from the GO table or any include table }

      ReadALine;

      match := Copy( line, 1, len );

      { ---If found, stop looking }

      if ( match = alias ) and ( line[len + 1] = ' ' ) then
      begin
         done := True;
         line := Copy( line, ( len + 1 ), Length( line ) );
      end;

      { ---Open up include file if directed to }

      if match[1] = '!' then
         OpenIncludeFile;
      
      { ---EOF and close file processing for include file }

      if isInclude then
      begin
         if eof( includeFile ) then
         begin
            isInclude := False;
            Close( includeFile );
         end;
      end;
   end;
end;
(* ----------------------------------------------------------- *)
procedure AttemptChangeDir;

begin
   if Length( cDir ) > 0 then
   begin
      {$I-}
      ChDir( cDir );

      { ---If ChDir wasn't successful and token wasn't in GO.TBL... }

      if ( not done ) and ( IOResult <> 0 ) then
      begin
         Writeln( 'Invalid Path: ', cDir );
         Halt( 5 );   
      end
      else
      begin
         { ---If successful and token WAS in GO.TBL... }

         if done and ( IOResult = 0 ) then
         begin
            cDir := Lower( cDir );
            Writeln( cDir );
         end;
      end;

      { ---If drive error, go to C: }

      if ( IOResult = 15 ) then
         Chdir( 'C:' );

      {$I+}
   end;
end;
(* ----------------------------------------------------------- *)
procedure LookUpAlias;

begin

   { ---Open the GO.TBL }

   Assign( tableFile, tableFileName );
   Reset(  tableFile );

   { ---Scan table, look for aliases }

   sub := False;
   while ( not eof( tableFile ) ) and ( not sub ) do 
   begin

      readln( tableFile, subLn );

      subLn := LTrim( subLn );

      if ( subLn[1] = '<' ) then
      begin

         subLn := Upper( subLn );

         { ---Copy the alias being inspected into 'subWrd' }

         subWrd := '';
         if Length( subLn ) >= Length( subst ) then
         begin
            for count := 1 to Length( subst ) do
            begin
               subWrd := subWrd + subLn[1];
               subLn  := Copy( subLn, 2, Length( subLn ) -1 );
            end;
         end;

         { ---If we found the right alias, expand our string }

         if ( subWrd = subst ) then
         begin

            subLn := LTrim( subLn );
         
            { ---Store the expand text to 'subWrd' }

            subWrd := '';
            for count := 1 to Length( subLn ) do
            begin
               if ( WhiteSpace( subLn[count] ) ) then 
               begin
                  count  := Length( subLn );
               end
               else
               begin
                  subWrd := subWrd + subLn[count];
               end;
            end;

            { ---Store the expand text to 'cDir' }

            cDir := cDir + subWrd;
            sub  := True;
         end;
      end;
   end;

   Close( tableFile );
end;
(* ----------------------------------------------------------- *)
procedure ParseForDirs;

begin

   { ---'line' now has the change directory list.  Start parsing and attempt
        changedir with each token }

   Repeat

      line := LTrim( line );

      { ---Parse out directories }

      cDir := '';
      while ( Length( line ) > 0 ) and ( not WhiteSpace( line[1] ) ) do
      begin

         { ---If there's an alias, parse it and look it up in the GO.TBL }

         if ( line[1] = '<' ) then
         begin
            subst := '';
            sub   := False;
            repeat
               subst := subst + line[1];
               if ( line[1] = '>' ) then
                  sub := True;
               line := Copy( line, 2, Length( line ) -1 );
            until ( sub ) or ( Length( line ) = 0 );

            { ---If a valid alias was parsed, look it up }

            if sub = True then
               LookUpAlias;
         end
         else
         begin
         
            { ---Store the rest of the dir name to 'cDir' }

            cDir := cDir + line[1];
            line := Copy( line, 2, Length( line ) -1 );
         end;
      end;

      { ---If we have any text in cDir, attempt to changedir to it }

      AttemptChangeDir;

   Until Length( line ) = 0;
end;
(* ----------------------------------------------------------- *)
procedure Execute( var command : commandtype );

begin
end;

begin

   { ---Check for valid dos version }

   if Lo( DosVersion ) < 3 then 
   begin
      Writeln( 'ERROR: Need DOS version 3.0 or higher' );
      Halt( 1 );
   end;

   { ---Store the parameter in alias ('home' if no parameter) }

   if ParamCount < 1 then
   begin
      alias := '';
      alias := alias + 'HOME';
   end
   else
   begin
      alias := ParamStr( 1 );
      alias := Upper( alias );

      { ---Test for command line switches }
      helpLvl := GetSwitch( alias );

      if helpLvl = 1 then
      begin        
         Help;
         Halt( 1 );
      end;
   end;

   { ---Open GO.TBL }

   tableFileName := GetPath + 'TBL';
   if FileExists( tableFileName ) then 
   begin
      Assign( tableFile, tableFileName );
      Reset(  tableFile );
   end
   else 
   begin
      Writeln( 'ERROR: No table found' );
      Halt( 1 );
   end;

   { ---Scan the table, looking for matches along the way }

   ScanGoTable;

   Close( tableFile );

   { ---If we did not find the alias in the table, assume the guy passed a
        path name }

   if not done then
      line := alias;

   { ---'line' now has the change directory list.  Start parsing and attempt
        changedir with each token }

   ParseForDirs;

end.
(* ----------------------------------------------------------- *)
