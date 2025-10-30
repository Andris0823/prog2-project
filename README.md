# prog2-project
This is a C# project that searches for images and will create an image viewer from the said images.

## How it works
The program recursively goes through a given folder and searches for any image file (png, jpg, jpeg, webp and even gif), then creates an HTML file for each picture and for the folder itself.
The "main" index.html will be placed in the root folder (the folder you specify when running the program).

## Requirements
- .NET 8.0 SDK or later

## Building the Project
```bash
dotnet build
```

## Usage

### Generate HTML files for images
```bash
dotnet run <folder path>
```

### Remove generated HTML files
```bash
dotnet run <folder path> -c
```

### Display help
```bash
dotnet run -- -h
```

## Testing
I uploaded a rar file that contains a folder that almost fully covers all the possible outcomes for the program, so you can test it out yourself!
(unzip before using it :D)

## Notes
The comments in the code are in Hungarian but their main purpose is to give a little information about the main methods and functions.
