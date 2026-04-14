# NPS - Non-Printable Symbols & Homoglyph Toolkit

NPS is a professional-grade security research toolkit designed for the simulation, analysis, and detection of adversarial text attacks. The application demonstrates techniques used in text obfuscation, such as zero-width character injection and homoglyph substitution, while providing mechanisms for detection and recovery.

This tool is intended for researchers, security engineers, and developers to analyze modern text obfuscation methods and implement defensive measures.

## Key Features

### Attack Simulation Engine
*   **Zero-Width Character Injection**: Embed invisible Unicode characters (U+200B, U+200C, U+200D, U+FEFF) into text at configurable frequencies.
*   **Homoglyph Substitution**: Substitute Latin characters with visually identical Cyrillic counterparts (and vice-versa).
*   **Hybrid Attacks**: Combine multiple obfuscation techniques for complex adversarial text generation.
*   **Visual Inspector**: Provide real-time visualization of injected characters and substituted homoglyphs through color-coded highlighting.

### Detection and Analysis Engine
*   **Multi-Factor Threat Scoring**: Calculate threat levels on a 0-100 scale based on anomaly density and script mixing.
*   **Shannon Entropy Analysis**: Identify potential obfuscation by measuring text randomness.
*   **Script Distribution Analysis**: Analyze character composition across different alphabets (Latin, Cyrillic, etc.).
*   **Detailed Findings**: Provide a structured breakdown of detected anomalies and script contamination.

### Recovery and Normalization
*   **Automated Normalization**: Revert homoglyph substitutions and remove non-printable characters to restore original text content.
*   **Clean Export**: Facilities for copying or saving recovered text for further analysis.

## Technical Specifications

*   **UI Framework**: Avalonia UI 11.0+
*   **Runtime**: .NET 8.0
*   **Language**: C# 12
*   **Architecture**: Service-oriented with Dependency Injection

## Installation and Deployment

### Prerequisites
*   .NET 8.0 SDK
*   Windows 10/11 (64-bit)

### Build Instructions
To run the application from source:
```bash
dotnet run --project NPS/NPS.csproj
```

### Standalone Distribution
To create a single, self-contained executable for Windows that includes the .NET runtime:
```bash
dotnet publish NPS/NPS.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishTrimmed=false
```
The output will be located at: `NPS/bin/Release/net8.0/win-x64/publish/NPS.exe`

## Usage Guide

### Attack Simulation
1.  Navigate to the Attack window.
2.  Input the source text via paste or file upload.
3.  Configure the attack methodology and intensity.
4.  Execute the attack to generate adversarial output.
5.  Use the Visual Inspector to review the modifications.

### Detection and Recovery
1.  Navigate to the Detect window.
2.  Input the suspicious text.
3.  Perform the analysis to receive a threat score and detailed findings.
4.  Review the normalization results to see the recovered text.
5.  Export the clean text as needed.

## Security and Ethics
This toolkit is provided for educational and research purposes only. Users are responsible for ensuring their use of the software complies with all applicable laws and ethical guidelines.

## License
This project is licensed under the MIT License.
