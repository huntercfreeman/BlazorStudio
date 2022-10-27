# BlazorStudio.ClassLib/FileSystem/Classes
This directory contains the Interfaces and Classes for representing the user's file system as C# classes. For example [AbsoluteFilePath.cs](/BlazorStudio.ClassLib/FileSystem/Classes/AbsoluteFilePath.cs) is used for mainly two reasons:

- A string is ambiguous. Is the string an absolute path or a relative path? AbsoluteFilePath.cs removes this ambiguity.
- Instead of parsing an absolute file path string every time a section of that string is needed (example: a filename) one can parse the string once and maintain all that data on a class.