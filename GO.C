//
// GO directory aliasing utility.  
//
// Modification LOG:
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
// v1.1 ??/??/91 - Georges Rabahni
//      Added support for aliased variables (placed in angle brackets)
//
// v1.0 04/19/91 - Lloyd Tabb
//
// v0.1 ??/??/91 - Emanuel Mashian
//      Original Coding
//

// --------------------------------------------------------------
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <dir.h>
#include <dos.h>
// --------------------------------------------------------------
#define OK 1

#define CMDLINELENGTH 127
#define MAXLINELENGTH 500
#define TAB 0x09
// --------------------------------------------------------------
typedef struct nodeType
    {
	char   *alias;
    char   *value;
	struct nodeType *leftChild;
	struct nodeType *rightChild;
	};
// --------------------------------------------------------------
FILE    *tableFile;
char    tableFileName[ CMDLINELENGTH ];
FILE    *includeFile;
int     labelFound;
char    *line;
char    *alias;
char    *buffer;
int     isInclude;
char    *token;
char    tokenSep[] = " \t,";

struct  nodeType *root = NULL;

int     helpLvl;
int     useTable  = OK;
int     useDos    = OK;
int     quietMode = !OK;
// --------------------------------------------------------------
void Help( void )
    {
    printf( "GO v3.0 Directory Navagation Utility\n" );
    printf( "Copyright (c) 1990,1991  Nobody in particular.\n" );
    printf( "\n" );
    printf( "usage: GO [options] <alias list>  to change drive and directory\n" );
    printf( "       options:\n" );
    printf( "            -h            this help screen\n" );
    printf( "            -t            only use table\n" );
    printf( "            -d            don't use table\n" );
    printf( "            -q            quiet mode\n" );
    printf( "\n" );
    printf( "Requires GO.TBL in same directory as GO.EXE\n" );
    printf( "\n" );
    printf( "The go table supports the following syntax:\n" );
    printf( "\n" );
    printf( "* directory names and labels can be explicitly included.\n" );
    printf( "* aliases can be defined by enclosing the alias in <brackets>.\n" );
    printf( "* dos variables can be referenced by using batch syntax %var%.\n" );
    printf( "\n" );
    printf( "examples:\n" );
    printf( "  tools        l:\\utils, c:\\bin\n" );
    printf( "  <netdrive>   v:\\database\n" );
    printf( "  work         <netdrive>\\newproj\\stuff  c:\\newproj\n" );
    printf( "  dosstuff     %cdos%c\\dos\n", '%', '%' );   
    }
// --------------------------------------------------------------
int GetSwitch( char *anyStr )
    {

    // ---Help switch

    if ( strcmp( anyStr, "?"  ) == 0 )
        return 1;
    if ( strcmp( anyStr, "-?" ) == 0 )
        return 1;
    if ( strcmp( anyStr, "/?" ) == 0 )
        return 1;
    if ( strcmp( anyStr, "-h" ) == 0 )
        return 1;
    if ( strcmp( anyStr, "/h" ) == 0 )
        return 1;
    if ( strcmp( anyStr, "-t" ) == 0 )
        return 2;
    if ( strcmp( anyStr, "/t" ) == 0 )
        return 2;
    if ( strcmp( anyStr, "-d" ) == 0 )
        return 3;
    if ( strcmp( anyStr, "/d" ) == 0 )
        return 3;
    if ( strcmp( anyStr, "-q" ) == 0 )
        return 4;
    if ( strcmp( anyStr, "/q" ) == 0 )
        return 4;

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
    char returnStr[ MAXLINELENGTH ];
    char *var;
       
    if ( ( *leftPos = strcspn( str, leftQuote ) ) != strlen( str ) )
        {
        var = str + *leftPos + 1;
        if ( ( *rightPos = strcspn( var, rightQuote ) ) != strlen( var ) )
            {
            strncpy( returnStr, var, *rightQuote );
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
        dosVarStr = getenv( strupr( dosVar ) );
        if ( dosVarStr != NULL )
            {
            strncpy( buffer, localStr, openIndex );
            buffer[ openIndex ] = 0;
            strcat(  buffer, dosVarStr );
            strcat(  buffer, ( localStr + closeIndex + 1 ) );
            strlwr(  buffer );

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
            strlwr(  buffer );

            // ---Return expanded string.

            *passStr = buffer;
            return ( OK );
            }
        }

    *passStr = localStr;
    return ( !OK );
    }
// --------------------------------------------------------------
void GetAToken( char *str )
    {
    
    token = strtok( str, tokenSep );

    // ---Expand any DOS variables.

    while ( ExpandAVariable( &token ) );

    // ---Expand aliases.

    while ( ExpandAnAlias( &token ) );

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
int OpenIncludeFile( void )
    {

    // ---Make sure there isn't an include file already open

    if ( isInclude )
        {
        printf( "ERROR: An include file can't open another include file\n" );
        return ( !OK );
        }

    token++;
    if ( strlen( token ) == 0 )
        GetAToken( NULL );
    
    // ---Attempt to open include file

    if ( ( includeFile = fopen( token, "r" ) ) == NULL )
        {
        printf( "ERROR: Could not open table [%s]\n", token );
        return ( !OK );
        }

    isInclude = OK;

    return ( OK );
    }
// --------------------------------------------------------------
int ReadALine( void )
    {
    
    line[0] = 0;

    if ( isInclude )
        {
        if ( fgets( line, MAXLINELENGTH, includeFile ) == NULL )
            {
            isInclude = !OK;
            fclose( includeFile );
            }
        }

    if ( !isInclude )
        fgets( line, MAXLINELENGTH, tableFile );
    
    line[ strlen( line ) - 1 ] = 0;
    strlwr( line );

    // ---LTrim the line.

    for ( ; ( line[0] == ' ' ) || ( line[0] == '\t' ); line++ );

    if ( line[0] == '<' )
        {
        StoreAlias();
        token[ 0 ] = 0;
        }

    GetAToken( line );

    // ---Open up include file if directed to

    if ( token[0] == '!' )
        {
        OpenIncludeFile();
        token[ 0 ] = 0;
        }
    
    return strlen( token );
    }
// --------------------------------------------------------------
int AttemptChangeDir( void )
    {
    int  drive;
    int  retVal;

    // ---Change drive if specified

    strcpy( buffer, token );

    if ( token[1] == ':' )
        {
        drive = token[0] - 'a';
        setdisk( drive );
        
        if ( getdisk() != drive )
            token[0] = 0;

        // ---Supply current path if only drive specified

        if ( strlen( token ) == 2 )
            {
            getcwd( buffer, MAXLINELENGTH - 1 );
            strlwr( buffer );
            if ( ( buffer[0] - 'a' ) != drive )
                buffer[0] = 0;
            }
        }

    if ( chdir( buffer ) == 0 )
        {

        // ---Dir change successful...

        if ( !quietMode )
            printf( "%s\n", buffer );
        }
    else
        {
        if ( !labelFound )
            {
            printf( "Invalid Path: %s\n", buffer );
            return ( !OK );   
            }
        }

    return ( OK );
    }
// --------------------------------------------------------------
void ParseForDirs( void )
    {
    for ( ; token[0] != 0; )
        {
        AttemptChangeDir();
        GetAToken( NULL );
        }
    }
// --------------------------------------------------------------
int ScanForLabels( void )
    {

    isInclude  = !OK;
    labelFound = !OK;

    while ( ( !feof( tableFile ) ) && ( !labelFound ) && ( useTable ) )
        {

        // ---Read a line, either from the GO table or any include tables

        if ( ReadALine() )
            {

            // ---If found, stop looking

            if ( strcmp( token, alias ) == 0 )
                {
                labelFound = OK;
                GetAToken( NULL );
                ParseForDirs();
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
        ParseForDirs();
        }

    return ( OK );
    }
// --------------------------------------------------------------
int Go( int argCnt, char **aliases )
    {
    int i;
    int tableOpened;
    int dirPassed;

    // ---Check for valid dos version

    if ( _osmajor < 3 )
        {
        printf( "ERROR: Need DOS version 3.0 or higher\n" );
        return ( !OK );
        }

    // ---Allocate space for vars

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

    // ---Store the parameter in alias ('home' if no parameter)

    tableOpened = !OK;
    dirPassed   = !OK;
    for ( i = 1, tableOpened = !OK; i <= argCnt; i++ )
        {

        if ( i < argCnt )
            strcpy( alias, strlwr( aliases[ i ] ) );
        else
            {
            if ( !dirPassed )
                strcpy( alias, "home" );
            else
                break;
            }

        // ---Test for command line switches
        helpLvl = GetSwitch( alias );

        switch ( helpLvl )
            {
            case 1:
                // ---Help
                Help();
                exit ( 1 );
            case 2:
                // ---Use table only
                useTable = OK;
                useDos   = !OK;
                break;
            case 3:
                // ---Use dos only
                useTable = !OK;
                useDos   = OK;
                break;
            case 4:
                // ---Quiet mode
                quietMode = OK;
                break;
            case 0:
                // ---Open GO.TBL

                dirPassed = OK;
                if ( useTable )
                    {
                    if ( ( !tableOpened ) &&
                         ( ( tableFile = fopen( aliases[0], "r" ) ) == NULL ) )
                        {
                        printf( "ERROR: Could not open table [%s]\n",
                                tableFileName );
                        return ( !OK );
                        }
                    else
                        {
                        if ( fseek( tableFile, 0L, SEEK_SET ) != 0 )
                            {
                            printf( "ERROR: Could not reposition table [%s]\n",
                                tableFileName );
                            return ( !OK );
                            }
                        }
                    tableOpened = OK;
                    }

                // ---Scan the table, looking for matches along the way

                ScanForLabels();
            }
        }

    fclose( tableFile );

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

    }
#endif
// --------------------------------------------------------------
