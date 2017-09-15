# findandreplace
Archive of fnr.exe - Find And Replace Tool, because CodePlex is shutting down

Original repo: https://findandreplace.codeplex.com/

# Description

An open source tool to find and replace text in multiple files.

Features

* Single file download - fnr.exe (181kb)
* Replace text in multiple files using windows application or through command line
* Find Only to see where matches are found
* Case-sensitive searching
* Searching for files in one directory or recursing sub-directories
* Regular expressions
* Find and replace multi-line text
* Generate command line button to create command line text to put in batch file
* Command line help
* Unit tests of Find/Replace engine

# Command Line Documentation

For command line parsing the following popular library was used:
http://commandline.codeplex.com/

## Usage

fnr.exe --cl --find "Text To Find" --replace "Text To Replace" --caseSensitive --dir "Directory Path" --fileMask "*.txt" --includeSubDirectories

## Flags

--cl	Required to run on command line.
--find	Required. Text to find.
--replace	Replacement text. If not specified only find operation is performed.
--useRegEx	Use regular expressions in 'find' parameter.
--useEscapeChars	Use escape chars like '\n' in 'find' and 'replace' parameters.
--caseSensitive	Case Sensitive.
--dir	Required. Directory path.
--includeSubDirectories	Include files in SubDirectories.
--fileMask	Required. File mask or multiple file masks separated by comma.
--excludeFileMask	File masks to exclude. If multiple - separate by comma
--help	Dispaly help screen.
--showEncoding	Displays detected encoding for each file with matches.
--silent	Supress the command window output.
--logFile	Path to log file where to save command output.
--skipBinaryFileDetection	Ignore detection of binary files.
--alwaysUseEncoding	Skip encoding detection and always use specified encoding.
--defaultEncodingIfNotDetected	If encoding is not detected in some very rare cases, use this one.
--includeFilesWithoutMatches	Include files without matches in results.
--setErrorLevelIfAnyFileErrors	Return ErrorLevel 2 if any files have read/write errors.

## Error Levels

ErrorLevels are:
0 - No errors
1 - Major error like directory is invalid
2 - Read/Write error for any processed file

By default only 0 and 1 are returned by the command. To enable ErrorLevel 2, use new flag --setErrorLevelIfAnyFileErrors

## Regular Expressions

.NET RegEx engine is used for regular expressions. It is described here:
http://msdn.microsoft.com/en-us/library/hs600312(v=vs.110).aspx

"In the .NET Framework, regular expression patterns are defined by a special syntax or language, which is compatible with Perl 5 regular expressions and adds some additional features such as right-to-left matching."

Here is a link to Regular Expression Language - Quick Reference in msdn:
http://msdn.microsoft.com/en-us/library/az24scfc(v=vs.110).aspx

## Encoding detection

fnr.exe uses two approaches two detect file encoding:
First KlerkSoft Bom Detector which is streamlined version of code from here:
http://www.architectshack.com/TextFileEncodingDetector.ashx
If Bom Detection fails it uses Microsoft's MLang library using approach described here:
http://www.codeproject.com/Articles/17201/Detect-Encoding-for-In-and-Outgoing-Text

In some very rare cases the detection may fail or may report a false positive. The reason for it failing is that detecting correct file encoding 100% of the time is impossible. MLang uses heuristics to go through file content and give its best guess on encoding. Usually its guess is correct 99%+ of the time. The only sure way to load a file with correct encoding is to know it upfront and pass it in.

If you have a case where encoding detection fails for a specific file, you will get error message "Could not detect file encoding." for that file. 

To get around this error there are several approaches:
You can open the problematic file in Notepad and save it using Unicode encoding. This will add a BOM header to the file so it will be easily detected by KlerkSoft Bom Detector
Use --defaultEncodingIfNotDetected or --alwaysUseEncoding to bypass automatic detection. See details for these flags in the table above.

For flags --defaultEncodingIfNotDetected and --alwaysUseEncoding you can use any encoding listed in the following web page. Just scroll down to example results and look for values in column 'identifier and name':

http://msdn.microsoft.com/en-us/library/system.text.encoding.getencodings(v=vs.110).aspx

## Special characters

Escape new line and quote characters using \n and \". If there is actual text "\n" in find or replace text, you can use [NewLine] for new lines instead of "\n" (starting v 1.7)

## Using accented characters in BAT Files

By default when you run DOS commands, the values in arguments are limited to code page 437. This code page is the character set of the original IBM PC (personal computer), or MS-DOS. It is pretty old and doesn't support a lot of accented chars like "ó" and many others.

To pass chars that are not supported by code page 437 in find/replace text or other arguments, please use information in the following links:

How to write batch file that has accented chars:
ur:http://stackoverflow.com/questions/1427796/batch-file-encoding/1427817

View UTF-16 or other encodings in DOS prompt
http://stackoverflow.com/questions/10764920/utf-16-on-cmd-exe

One more tip is to make sure you save your BAT file in UTF-8 or other format that displays the chars correctly. Don't use ASCII.
