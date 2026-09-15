# SimpleAutoClicker

A simple and lightweight auto clicker for Windows.

## Features

- Adjustable click interval in milliseconds
- Left-clicks at the current cursor position
- Global **F6** hotkey to start or stop clicking
- Simple **Start / Stop** button
- Clear **Running / Stopped** status
- No network access
- No telemetry
- No third-party libraries

## Usage

1. Launch `AutoClicker.exe`.
2. Set the click interval in milliseconds.
3. Press **Start** or **F6** to begin clicking.
4. Press **Stop** or **F6** again to stop.

> Minimum interval: 1 ms.

## Source code

The original source used to build the app is in the [`AutoClicker`](AutoClicker) folder.

The project is written in **C# Windows Forms** and targets **.NET Framework 4.0**.

### Build from source

Open `AutoClicker/AutoClicker.csproj` in Visual Studio with .NET Framework development tools installed, then build the project in Release mode.

The app uses only built-in .NET Framework libraries and Windows `user32.dll` APIs for the global F6 hotkey and simulated mouse input.

## Download

A prebuilt Windows executable will be added to the **Releases** section.

## Notes

Some antivirus engines may flag auto clickers because they register a global hotkey and simulate mouse input. The project is open source so the code can be inspected and built locally.

## License

Licensed under the [MIT License](LICENSE).
