//
// GO directory aliasing utility.  
//
// Modification LOG:
//
// v3.5 05/23/95 - Richard Rozsa
//      No longer upper case strings. Preserve typed or alias file case.
//
// v3.4 01/25/94 - Richard Rozsa
//      Fixed bug in GetQuotedString() where (char *) variable was used
//      instead of (int) to calculate length of strncpy().
//      This fix allows dos variables to be referrenced by aliases.
//
// v3.3 09/28/92 - Richard Rozsa
//      Added /m parameter: make any directory that's not found.
//      Fixed bug that prevented a go to an <alias> value.
//
// v3.2 09/11/91 - Richard Rozsa
//      Localized some global variables.
//      Reworked GO and HELP functions.
//      General cleanup.
//
// v3.1 09/09/91 - Richard Rozsa
//      Added support for novell style "..." directory references.
//
// v3.0 09/09/91 - Richard Rozsa
//      Converted source code from PASCAL to C.
//      Now allows go tables to have the read-only file attribute set.
//
// v2.0 08/29/91 - Richard Rozsa
//      Added support for DOS variables in expansion list (%var%).
//
// v1.3 08/29/91 - Richard Rozsa
//      Cleaned up and commented code.
//      Added support for leading white space before labels and aliases.
//      Added include file support (filename prefixed by '!').
//      (aliases currently not supported in include files.)
//
// v1.2 05/24/91 - Richard Rozsa
//      Added support for 'home' directory.
//      Added command line switch processing (to get help screen).
//
// v1.1 ??/??/91 - Georges Rahbani
//      Added support for aliased variables (placed in angle brackets)
//
// v1.0 04/19/91 - Lloyd Tabb
//
// v0.1 1989,1990 - Emanuel Mashian
//      Original Coding
//
// Wish List:
//
// 09/28/92: Add <alias> option that can read a novell drive regardless
//           of it's mapping.
//

// --------------------------------------------------------------
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <dir.h>
#include <dos.h>
// --------------------------------------------------------------
#define OK 1

#define MAXLINELENGTH 1024
#define TAB 0x09

#define TOKENSEP " \t,"
// --------------------------------------------------------------
typedef struct nodeType
    {
	char   *alias;
    char   *value;
	struct nodeType *leftChild;
	struct nodeType *rightChild;
	};
// --------------------------------------------------------------
char    *line;
char    *alias;
char    *buffer;
char    *token;

struct  nodeType *root = NULL;

int     quietMode = !OK;
// --------------------------------------------------------------
void Help( void )
    {
    printf( "GO Directory Navagation Utility  Version 3.5  Copyright (c) 1989-1994\n" );
    printf( "   by Richard Rozsa, Emanuel Mashian, Lloyd Tabb, and Georges Rahbani\n" );
    printf( "\n" );
    printf( "Usage: GO [options] <label/dir list>    (to change drives and directories)\n" );
    printf( "       options:\n" );
    printf( "            -h  this help screen      -q  quiet mode\n" );
    printf( "            -t  only use table        -d  don't use table\n" );
    printf( "            -m  make directory when not found\n" );
    printf( "\n" );
    printf( "The go table supports the following syntax:\n" );
    printf( "\n" );
    printf( "* Directory names and labels can be explicitly included.\n" );
    printf( "* Aliases can be defined by enclosing the alias in <brackets>.\n" );
    printf( "* DOS variables can be referenced by using batch syntax %var%.\n" );
    printf( "* Include files are defined by using the bang symbol: !filename\n" );
    printf( "* The HOME label will be searched for if no parameters are passed.\n" );
    printf( "\n" );
    printf( "examples:\n" );
    printf( "  tools        l:\\utils, c:\\bin\n" );
    printf( "  <netdrive>   v:\\database\n" );
    printf( "  work         <netdrive>\\newproj\\stuff  c:\\newproj\n" );
    printf( "  dosstuff     %cdos%c\\dos\n", '%', '%' );
    printf( "  !c:\\bin\\work.tbl\n" );
    }
// --------------------------------------------------------------
int GetSwitch( char *anyStr, int *useTable, int *useDos, int *makeDir )
    {

    // ---Help switch

    if ( ( anyStr[1] == 0 ) && ( anyStr[0] == '?' ) )
        {
        Help();
        return 1;
        }

    if ( ( anyStr[2] == 0 ) && ( ( anyStr[0] == '-' ) || ( anyStr[0] == '/' ) ))
        {
        switch ( anyStr[1] )
            {
            case '?':
                Help();
                return 1;
            case 'h':
                Help();
                return 1;
            case 't':
    
                // ---Use table only
                
                *useTable = OK;
                *useDos   = !OK;
                return 2;
            case 'd':
    
                // ---Use dos only
                
                *useTable = !OK;
                *useDos   = OK;
                return 3;
            case 'q':
    
                // ---Quiet mode
                
                quietMode = OK;
                return 4;
            case 'm':

                // ---Make directory if not present

                *makeDir = OK;
                return 5;
            default :

                // ---Invalid switch

                printf( "ERROR: Invalid switch.\n" );
                return 1;
            }
        }

    return 0;
    }
// --------------------------------------------------------------
int StoreOnTree( char *aliasStr, char *valueStr, struct nodeType **node )
    {
    if ( *node == NULL )
        {
        if ( ( *node = malloc( sizeof( struct nodeType ) ) ) == NULL )
            {
            printf( "ERROR: Not enough memory to allocate a new node\n" );
            return ( !OK );
            }
        if ( ( (*node)->alias = malloc( strlen( aliasStr ) + 1 ) ) == NULL )
            {
	        printf( "ERROR: Not enough memory for string\n" );
	        return ( !OK );
            }
        if ( ( (*node)->value  = malloc( strlen( valueStr ) + 1 ) ) == NULL )
            {
	        printf( "ERROR: Not enough memory for string\n" );
	        return ( !OK );
            }
        strcpy( (*node)->alias, aliasStr );
        strcpy( (*node)->value, valueStr );
        (*node)->leftChild  = NULL;
        (*node)->rightChild = NULL;
        }
    else
        {
        if ( strcmp( (*node)->alias, aliasStr ) == 0 )
            {
            }
        else
            {
            if ( strcmp( (*node)->alias, aliasStr ) < 0 )
	            return (StoreOnTree(aliasStr,valueStr,&((*node)->rightChild )));
            else
	            return (StoreOnTree(aliasStr,valueStr,&((*node)->leftChild  )));
            }
        }
    return ( OK );
    }
// --------------------------------------------------------------
char *GetAliasValue( char *str, struct nodeType **node )
    {
    
    if ( *node == NULL )
        return ( NULL );

    if ( strcmp( (*node)->alias, str ) == 0 )
        return ( (*node)->value );
    else
        {
        if ( strcmp( (*node)->alias, str ) < 0 )
	        return ( GetAliasValue( str, &( (*node)->rightChild ) ) );
        else
	        return ( GetAliasValue( str, &( (*node)->leftChild  ) ) );
        }
    }
// --------------------------------------------------------------
char *GetQuotedString( char *str, int  *leftPos,   int  *rightPos,
                                  char *leftQuote, char *rightQuote )
    {
    static char returnStr[ MAXLINELENGTH ];
    char *var;
       
    if ( ( *leftPos = strcspn( str, leftQuote ) ) != strlen( str ) )
        {
        var = str + *leftPos + 1;
        if ( ( *rightPos = strcspn( var, rightQuote ) ) != strlen( var ) )
            {
            strncpy( returnStr, var, *rightPos );
            returnStr[ *rightPos ] = 0;

            *rightPos += ( *leftPos + 1 );
            return ( returnStr );
            }
        }

    *leftPos  = 0;
    *rightPos = 0;
    return ( NULL );

    }
// --------------------------------------------------------------
int ExpandAVariable( char **passStr )
    {
    char *localStr;
    int  openIndex;
    int  closeIndex;
    char *dosVar;
    char *dosVarStr;

    localStr = *passStr;

    // ---Check for a dos variable (expand if found)...

    dosVar = GetQuotedString( localStr, &openIndex, &closeIndex, "%", "%" );
    if ( closeIndex > 0 )
        {
        dosVarStr = getenv( dosVar );
        if ( dosVarStr != NULL )
            {
            strncpy( buffer, localStr, openIndex );
            buffer[ openIndex ] = 0;
            strcat(  buffer, dosVarStr );
            strcat(  buffer, ( localStr + closeIndex + 1 ) );

            // ---Return expanded string.

            *passStr = buffer;
            return ( OK );
            }
        }

    *passStr = localStr;
    return ( !OK );
    }
// --------------------------------------------------------------
int ExpandAnAlias( char **passStr )
    {
    char *localStr;
    int  openIndex;
    int  closeIndex;
    char *aliasStr;
    char *valuestr;

    localStr = *passStr;

    // ---Check for an alias (expand if found)...

    aliasStr = GetQuotedString( localStr, &openIndex, &closeIndex, "<", ">" );
    if ( closeIndex > 0 )
        {
        if ( ( valuestr = GetAliasValue( aliasStr, &root ) ) != NULL )
            {
            strncpy( buffer, localStr, openIndex );
            buffer[ openIndex ] = 0;
            strcat(  buffer, valuestr );
            strcat(  buffer, ( localStr + closeIndex + 1 ) );

            // ---Return expanded string.

            *passStr = buffer;
            return ( OK );
            }
        }

    *passStr = localStr;
    return ( !OK );
    }
// --------------------------------------------------------------
int ExpandDots( char **passStr )
    {
    char *localStr;
    int  i;
    int  openIndex;
    char *dots;

    localStr = *passStr;

    // ---Check for novell style dots "..." (expand if found)...

    if ( ( dots = strstr( localStr, "..." ) ) != NULL )
        {
        openIndex = ( localStr - dots );
        strncpy( buffer, localStr, ( openIndex + 2 ) );
        buffer[ openIndex + 2 ] = 0;
        for ( i = 1; localStr[ openIndex + 1 + i ] == '.' ; i++ )
            {
            strcat( buffer, "\\.." );
            }
        localStr += ( openIndex + 1 + i );
        strcat( buffer, localStr );

        // ---Return expanded string.

        *passStr = buffer;
        return ( OK );
        }

    *passStr = localStr;
    return ( !OK );
    }
// --------------------------------------------------------------
void GetAToken( char *str )
    {
    
    token = strtok( str, TOKENSEP );

    // ---Expand any DOS variables.

    while ( ExpandAVariable( &token ) );

    // ---Expand aliases.

    while ( ExpandAnAlias( &token ) );

    // ---Expand dots.

    while ( ExpandDots( &token ) );

    }
// --------------------------------------------------------------
int StoreAlias( void )
    {
    int  openIndex;
    int  closeIndex;
    char *aliasStr;

    // ---Check for an alias (store if found)...

    aliasStr = GetQuotedString( line, &openIndex, &closeIndex, "<", ">" );
    if ( ( closeIndex > 0 ) && ( strlen( aliasStr ) > 0 ) )
        {
        GetAToken( line + closeIndex + 1 );
        if ( strlen( token ) > 0 )
            return ( StoreOnTree( aliasStr, token, &root ) );
        }
    return ( !OK );
    }
// --------------------------------------------------------------
int OpenIncludeFile( FILE **includeFile, int *isInclude )
    {

    // ---Make sure there isn't an include file already open

    if ( *isInclude )
        {
        printf( "ERROR: An include file can't open another include file\n" );
        return ( !OK );
        }

    token++;
    if ( strlen( token ) == 0 )
        GetAToken( NULL );
    
    // ---Attempt to open include file

    if ( ( *includeFile = fopen( token, "r" ) ) == NULL )
        {
        printf( "ERROR: Could not open table [%s]\n", token );
        return ( !OK );
        }

    *isInclude = OK;

    return ( OK );
    }
// --------------------------------------------------------------
int ReadALine( FILE *tableFile, FILE **includeFile, int *isInclude )
    {
    
    line[0] = 0;

    if ( *isInclude )
        {
        if ( fgets( line, MAXLINELENGTH, *includeFile ) == NULL )
            {
            *isInclude = !OK;
            fclose( *includeFile );
            }
        }

    if ( !(*isInclude) )
        fgets( line, MAXLINELENGTH, tableFile );
    
    line[ strlen( line ) - 1 ] = 0;

    // ---LTrim the line.

    for ( ; ( line[0] == ' ' ) || ( line[0] == '\t' ); line++ );

    if ( line[0] == '<' )
        {
        StoreAlias();
        token[ 0 ] = 0;
        return 0;
        }

    GetAToken( line );

    // ---Open up include file if directed to

    if ( token[0] == '!' )
        {
        OpenIncludeFile( includeFile, isInclude );
        token[ 0 ] = 0;
        }
    
    return strlen( token );
    }
// --------------------------------------------------------------
int AttemptChangeDir( int labelFound, int makeDir )
    {
    int  drive;
    int  retVal;
    int  minLen;
    char *pS;
    char *pE;

    // ---Change drive if specified

    strcpy( buffer, token );

    if ( token[1] == ':' )
        {
        drive = token[0] - 'a';
        setdisk( drive );
        
        if ( getdisk() != drive )
            token[0] = 0;

        // ---Done if current path specifies only drive

        if ( token[ 2 ] == 0 )
            {
            getcwd( buffer, MAXLINELENGTH - 1 );
            if ( ( buffer[0] - 'a' ) != drive )
                buffer[0] = 0;
            }

        minLen = 3;
        }
    else
        minLen = 1;

    drive = strlen( buffer );
    if ( ( drive > minLen ) && ( buffer[ drive - 1 ] == '\\' ) )
        buffer[ drive - 1 ] = 0;

    if ( chdir( buffer ) == 0 )
        {

        // ---Dir change successful...

        if ( !quietMode )
            printf( "%s\n", buffer );
        }
    else
        {

        // ---Dir change UNsuccessful...

        if ( makeDir )
            {
            pS = buffer;
            if ( pS[1] == ':' )
                pS += 2;
            if ( pS[0] == '\\' )
                {
                if ( chdir( "\\" ) == -1 )
                    {
                    printf( "Couldn't move to root directory\n" );
                    return ( !OK );
                    }
                pS++;
                }

            for ( ; ( pE = strchr( pS, '\\' ) ) != NULL; )
                {
                pE[0] = 0;
                if ( chdir( pS ) == -1 )
                    {
                    if ( mkdir( pS ) == -1 )
                        {
                        printf( "Couldn't create directory: %s\n", buffer );
                        return ( !OK );
                        }
                    if ( chdir( pS ) == -1 )
                        {
                        printf( "Couldn't move to just created directory: %s\n", buffer );
                        return ( !OK );
                        }
                    }
                pE[0] = '\\';
                pS = pE;
                pS++;
                }
            if ( mkdir( pS ) == -1 )
                {
                printf( "Couldn't create directory: %s\n", buffer );
                return ( !OK );
                }
            if ( chdir( pS ) == -1 )
                {
                printf( "Couldn't move to just created directory: %s\n", buffer );
                return ( !OK );
                }

            // ---Make/change dir successful...

            if ( !quietMode )
                printf( "%s\n", buffer );
            }
        
        else if ( !labelFound )
            {
            printf( "Invalid Path: %s\n", buffer );
            return ( !OK );   
            }
        }

    return ( OK );
    }
// --------------------------------------------------------------
void ParseForDirs( int labelFound, int makeDir )
    {
    for ( ; token[0] != 0; )
        {
        AttemptChangeDir( labelFound, makeDir );
        GetAToken( NULL );
        }
    }
// --------------------------------------------------------------
int ScanForLabels( FILE *tableFile, int useTable, int useDos, int makeDir )
    {
    FILE *includeFile;
    int  labelFound = !OK;
    int  isInclude  = !OK;

    while ( ( !feof( tableFile ) ) && ( !labelFound ) && ( useTable ) )
        {

        // ---Read a line, either from the GO table or any include tables

        if ( ReadALine( tableFile, &includeFile, &isInclude ) )
            {

            // ---If found, stop looking

            if ( strcmp( token, alias ) == 0 )
                {
                labelFound = OK;
                GetAToken( NULL );
                ParseForDirs( labelFound, makeDir );
                }
            }
        }

    // ---If we did not find the alias in the table, assume the guy passed a
    //      path name

    if ( !labelFound )
        {
        if ( !useDos )
            {
            printf( "Label [%s] not found\n", alias );
            return ( !OK );
            }
        strcpy( line, alias );
        GetAToken( line );
        ParseForDirs( labelFound, makeDir );
        }

    return ( OK );
    }
// --------------------------------------------------------------
int GoToTopOfTable( FILE **tableFile, int tableOpened, char *fileName )
    {

    if ( !tableOpened )
        {
        if ( ( *tableFile = fopen( fileName, "r" ) ) == NULL )
            {
            printf( "ERROR: Could not open table [%s]\n", fileName );
            return ( !OK );
            }
        tableOpened = OK;
        }
    else
        {
        if ( fseek( *tableFile, 0L, SEEK_SET ) != 0 )
            {
            printf( "ERROR: Could not reposition table [%s]\n", fileName );
            return ( !OK );
            }
        }
    return ( OK );
    }
// --------------------------------------------------------------
int ProcessParameters( int argCnt, char **aliases )
    {
    FILE *tableFile;
    int  i;
    int  helpLvl;
    int  tableOpened = !OK;
    int  dirPassed   = !OK;
    int  useTable    = OK;
    int  useDos      = OK;
    int  makeDir     = !OK;

    for ( i = 1, tableOpened = !OK; i <= argCnt; i++ )
        {

        if ( i < argCnt )
            strcpy( alias, aliases[ i ] );
        else
            {
            if ( !dirPassed )
                
                // ---Store the parameter in alias ('home' if no parameter)

                strcpy( alias, "home" );
            else
                break;
            }

        // ---Test for and set command line switches
        
        helpLvl = GetSwitch( alias, &useTable, &useDos, &makeDir );
        
        if ( helpLvl == 1 )
            return ( !OK );

        if ( helpLvl == 0 )
            {
            dirPassed = OK;
            if ( useTable )
                {

                // ---Open GO.TBL
                                
                if ( !GoToTopOfTable( &tableFile, tableOpened, aliases[0] ) )
                    return ( !OK );
                }

            // ---Scan the table, looking for matches along the way

            ScanForLabels( tableFile, useTable, useDos, makeDir );
            }
        }

    fclose( tableFile );

    return ( OK );
    }
// --------------------------------------------------------------
int Go( int argCnt, char **aliases )
    {

    // ---Check for valid dos version

    if ( _osmajor < 3 )
        {
        printf( "ERROR: Need DOS version 3.0 or higher\n" );
        return ( !OK );
        }

    // ---Allocate space for variables

    if ( ( line   = malloc( MAXLINELENGTH ) ) == NULL )
        {
        printf( "ERROR: Not enough memory for LINE buffer\n" );
        return ( !OK );
        }
    if ( ( alias  = malloc( MAXLINELENGTH ) ) == NULL )
        {
        printf( "ERROR: Not enough memory for ALIAS buffer\n" );
        return ( !OK );
        }
    if ( ( buffer = malloc( MAXLINELENGTH ) ) == NULL )
        {
        printf( "ERROR: Not enough memory for BUFFER buffer\n" );
        return ( !OK );
        }

    // ---Process the parameters

    if ( !ProcessParameters( argCnt, aliases ) )
        return ( !OK );

    return ( OK );
    }
// --------------------------------------------------------------
#ifndef _GO_
void main( int argc, char **argv )
    {

    // ---Open GO.TBL

    argv[0] [ strlen( argv[0] ) - 3 ] = 'T';
    argv[0] [ strlen( argv[0] ) - 2 ] = 'B';
    argv[0] [ strlen( argv[0] ) - 1 ] = 'L';

    if ( !Go( argc, argv ) )
        exit ( 1 );

    exit ( 0 );
    }
#endif
// --------------------------------------------------------------
