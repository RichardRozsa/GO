{$M $1000, 0, 0}        { 4K stack, no heap }

(*
 * GO directory aliasing utility.  
 *
 * Modification LOG:
 *  
 * v1.1 Georges Rabahni
 *      Added support for aliased variables (placed in angle brackets)
 *
 * v0.1 Emanuel Mashian
 *      Original Coding
 *
 *)
program GO;

uses
   DOS;

type
   commandtype = record
      str : string;
      proc : pointer;
      end;

var
   tableFile : Text;
   tableFileName : string;
   len : integer;
   count : integer;
   done : boolean;
   line : string;
   alias : string;
   match : string;
   cdir : string;
   command : commandtype;

   sub : boolean;
   subst : string;
   subLn : string;
   subWrd: string;

function GetPath : string;
var
   endPos : integer;
   fileName : string;
begin
   fileName := ParamStr( 0 );
   endPos := length( fileName );
   while ( endPos >= 1 ) and ( fileName[endPos] <> '.' ) do
      endPos := endPos - 1;
   GetPath := Copy( ParamStr( 0 ), 1, endPos );
end;

function FileExists( fileName: string ) : Boolean;
var
   f: file;
begin
   {$I-}
   Assign(f, FileName);
   Reset(f);
   Close(f);
   {$I+}
   FileExists := (IOResult = 0) and
      (FileName <> '');
end;


procedure Execute( var command : commandtype );
begin
end;

begin
   if Lo( DosVersion ) < 3 then begin
      Writeln( 'Need DOS version 3.0 or higher' );
      Halt( 1 );
   end;

   if ParamCount < 1 then begin
      Writeln( 'GO v1.2 (C) Directory Navagation Utility' );
      Writeln( 'Copyright (C) 1991, Nobody in particular.' );
      Writeln;
      Writeln( 'USAGE: GO <alias>' );
      Halt( 1 );
   end;

   tableFileName := GetPath + 'TBL';
   if FileExists( tableFileName ) then begin
      Assign( tableFile, tableFileName );
      Reset( tableFile );
   end
   else begin
      Writeln( 'No table found' );
      Halt( 1 );
   end;

   alias := ParamStr( 1 );
   len := Length( alias );
   for count := 1 to len do
       alias[count] := upcase( alias[count] );
   done := False;
   while ( not eof( tableFile ) ) and ( not done ) do begin
      Readln( tableFile, line );
      match := Copy( line, 1, len );
      for count := 1 to Length( match ) do
          match[count] := upcase( match[count] );
      if    ( match = alias ) and
            ( line[len + 1] = ' ' ) then begin
         done := True;
         line := Copy( line, len + 2, Length( line ) );
      end;
   end;

   Close( tableFile );

   { we did not find the alias in the table, assume the guy passed a 
        path name }

   if not done then begin
        line := alias;
   end;

    Repeat
      while  (Length(line) > 0) and ((line[1] = ' ') or
              (line[1] = chr(9)) ) do
         line := Copy( line, 2, Length( line ) -1 );
      cdir := '';
      while ( (Length( line) > 0) and (line[1] <> ' ') ) do begin
         if (line [1] = '<') then
         begin
            subst := '';
            sub := false;
            repeat
               subst := subst + line [1];
               if (line [1] = '>') then
                  sub := true;
               line := Copy( line, 2, Length( line ) -1 );
            until (sub) or (length (line) = 0);

            if sub = true then
            begin
               for count := 1 to length (subst) do
                  subst [count] := upcase (subst [count]);
               Assign (tableFile, tableFileName);
               Reset (tableFile);
               sub := false;
               while (not eof (tableFile)) and (not sub) do 
               begin
                  readln (tableFile, subLn);
                  if (subLn [1] = '<') then
                  begin
                     for count := 1 to length (subLn) do
                        subLn [count] := upcase (subLn [count]);
                     subWrd := '';
                     if length (subLn) >= length (subst) then
                     begin
                        for count := 1 to length (subst) do
                        begin
                           subWrd := subWrd + subLn [1];
                           subLn := Copy( subLn, 2, Length(subLn) -1 );
                        end;
                     end;
                     if (subWrd = subst) then
                     begin
                        subWrd := '';
                        while ((length (subLn) > 0) and 
                              ((subLn [1] = ' ') or (subLn [1] = chr (9)))) do
                           subLn := Copy( subLn, 2, Length(subLn) -1 );
                           
                        for count := 1 to length (subLn) do
                        begin
                           if (subLn [count] = ' ') or 
                              (subLn [count] = chr (9)) then
                              count := length (subLn)
                           else
                              subWrd := subWrd + subLn [count];
                        end;
                        cdir := cdir + subWrd;
                        sub := true;
                     end;
                  end;
               end;
               Close (tableFile);
            end;
         end
         else
         begin
            cdir := cdir + line[1];
            line := Copy( line, 2, Length( line ) -1 );
         end;
      end;

      if length( cdir ) > 0 then begin
         {$I-}
         ChDir( cdir );
         if (not done) and (IOResult <> 0) then begin
             Writeln( 'Invalid Path: ',cdir );
             Halt(5);   
             end
         else if done and (IOResult = 0) then
             Writeln( cdir );
         if ( IOResult = 15 ) then
             Chdir( 'C:' );
         {$I+}
      end;
    Until Length( line ) = 0;
end.
