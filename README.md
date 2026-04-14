# NPS - Non-Printable Symbols & Homoglyph Toolkit

![License](https://img.shields.io/badge/license-MIT-blue)
![Platform](https://img.shields.io/badge/platform-Windows-blue)
![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![Language](https://img.shields.io/badge/language-C%23-green)

## 🎯 Overview

**NPS** is a professional-grade security research toolkit for simulating, analyzing, and detecting adversarial text attacks. It demonstrates state-of-the-art techniques used in real-world obfuscation attacks—such as zero-width character injection and homoglyph substitution—while providing robust detection and recovery mechanisms.

This tool bridges the gap between attack simulation and defensive security by allowing researchers, security engineers, and developers to:
- **Understand** how modern text obfuscation attacks work
- **Simulate** real-world attack vectors in a controlled environment
- **Detect** and **neutralize** suspicious patterns in text

---

## 🚀 Key Features

### 🔴 Attack Simulation Engine
- **Zero-Width Character Injection** - Embed invisible Unicode characters (U+200B, U+200C, U+200D, U+FEFF) at configurable frequencies
- **Homoglyph Substitution** - Seamlessly swap Latin characters with visually identical Cyrillic counterparts:
  - Example: `a` (Latin) ↔ `а` (Cyrillic) - indistinguishable to the human eye
  - Bidirectional mode for flexible transformation
- **Hybrid Attacks** - Combine injection and replacement for maximum obfuscation
- **Real-Time Visual Inspector** - Highlight injected characters (red dots) and substituted homoglyphs (green) with color-coded visualization

### 🟢 Detection & Analysis Engine
- **Multi-Factor Threat Scoring** (0-100 scale)
  - Zero-width character density analysis
  - Homoglyph detection using script-aware algorithms
  - Bidirectional override character detection
  - Entropy-based obfuscation pattern recognition
  - Alphabet mixing detection (Latin/Cyrillic cross-contamination)
- **Shannon Entropy Analysis** - Identifies abnormally low entropy suggesting compressed/obfuscated content
- **Script Distribution Breakdown** - Visualizes character composition across Latin, Cyrillic, and other alphabets
- **Detailed Findings Panel** - Human-readable anomaly breakdown with severity indicators

### 🔧 Professional UI/UX
- **Dual-Window Architecture** - Separate attack and detection workflows
- **Clean, Dark-Themed Interface** - Built with Avalonia for cross-platform consistency
- **File I/O Operations** - Load text from files, save/export results
- **Responsive Controls** - Fast, real-time analysis with no lag

---

## 🏗️ Architecture

### Service-Oriented Design

The project uses **dependency injection** and a **layered service architecture**:

```
┌─────────────────────────────────┐
│    MainWindow (Router)           │
│  - Service Injection Point       │
│  - Window Navigation             │
└──────────┬──────────┬────────────┘
           │          │
    ┌──────▼──┐    ┌──▼────────┐
    │  Attack  │    │  Detect   │
    │ Window   │    │  Window   │
    └──────┬──┘    └──┬────────┘
           │          │
    ┌──────▼──────────▼──────┐
    │   Service Layer        │
    ├───────────────────────┤
    │ • IInjectService      │
    │ • IReplaceService     │
    │ • IDetectService      │
    └───────────────────────┘
           │
    ┌──────▼──────────────────┐
    │  Business Logic         │
    │  • Unicode analysis     │
    │  • Entropy calculation  │
    │  • Pattern matching     │
    └───────────────────────┘
```

### Core Components

| Component | Responsibility |
|-----------|-----------------|
| **InjectService** | Zero-width character embedding with frequency control |
| **ReplaceService** | Homoglyph mapping & bidirectional substitution |
| **DetectService** | Entropy, alphabet distribution, anomaly scoring |
| **AttackWindow** | UI for crafting adversarial text |
| **DetectWindow** | UI for analysis & threat assessment |

---

## 🔬 Technical Highlights

### Mathematical Foundation
- **Shannon Entropy**: `H(X) = -Σ P(x_i) * log₂(P(x_i))` - Detects randomness and obfuscation patterns
- **Threat Scoring**: Weighted formula combining character density, entropy deviation, and alphabet mixing
- **Statistical Analysis**: Frequency distribution of Unicode blocks

### Unicode Handling
- Comprehensive Unicode category support:
  - **Zero-Width Characters**: U+200B, U+200C, U+200D, U+FEFF
  - **Bidirectional Overrides**: U+202A–E, U+2066–2069
  - **Script Detection**: Latin (A-Z, a-z), Cyrillic (А-я), Other

### Performance Optimizations
- Lightweight entropy recalculation
- Efficient character-by-character analysis
- Single-pass frequency computation

---

## 🛠️ Technology Stack

| Component | Technology |
|-----------|-----------|
| **UI Framework** | [Avalonia 11.0+](https://avaloniaui.net/) (Cross-platform XAML) |
| **Runtime** | .NET 8.0 |
| **Language** | C# 12 with nullable reference types |
| **Architecture** | Dependency Injection, Service Pattern |
| **Build System** | dotnet CLI |

---

## 📦 Installation & Setup

### Prerequisites
- **.NET 8.0 SDK** ([Download](https://dotnet.microsoft.com/en-us/download/dotnet/8.0))
- **Windows 10/11** (64-bit recommended)

### Quick Start

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/nps.git
   cd nps
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Run from source**
   ```bash
   dotnet run --project NPS/NPS.csproj
   ```

4. **Build for development**
   ```bash
   dotnet build -c Debug
   ```

---

## 🚀 Distribution

### Create Standalone Executable (Single .exe)

For easy distribution without requiring .NET installation:

```bash
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true --self-contained
```

**Output**: `NPS/bin/Release/net8.0/win-x64/publish/NPS.exe`
- **Size**: ~100-120 MB (includes .NET runtime)
- **Compatibility**: Windows 64-bit

### Framework-Dependent Build (Smaller)

Requires .NET 8 runtime on target machine:

```bash
dotnet publish -c Release -r win-x64
```

**Output**: Folder with `.exe` + supporting DLLs (~15 MB)

---

## 📖 Usage Guide

### Attack Simulation Workflow

1. **Launch NPS** → Click "Attack"
2. **Input Text** → Paste or load text from file
3. **Configure Attack**:
   - Select mode: Injection Only | Replacement Only | Combined
   - Set Injection Frequency (0-100%)
   - Enable "Reverse Mode" for Latin→Cyrillic
4. **Generate** → Click ➜ button
5. **Inspect** → Use Visual Inspector to see injected/replaced characters (colored dots)
6. **Export** → Save or copy to clipboard

### Detection Workflow

1. **Launch NPS** → Click "Detect"
2. **Paste Suspicious Text** → Load from file or paste directly
3. **Analyze** → Click "Simulate Analysis"
4. **Review Results**:
   - **Verdict Banner**: SUSPICIOUS/CLEAN with confidence score
   - **Metrics**: Zero-width, Homoglyphs, Bidi overrides counts
   - **Threat Bar**: Visual risk level (0-100)
   - **Findings Panel**: Detailed anomalies detected
5. **Take Action**: Clean exports, save reports, or investigate further

---

## 🎓 Use Cases

### Security Research
- Understand modern obfuscation techniques
- Test detection mechanisms
- Benchmark threat assessment algorithms

### Educational
- Demonstrate Unicode security vulnerabilities
- Teach entropy analysis and pattern recognition
- System security course material

### Defensive Security
- Audit text for hidden characters before processing
- Implement character normalization pipelines
- Build character sanitization rules

### Content Moderation
- Detect evasion attempts in user-generated content
- Identify masked phishing URLs/domains
- Flag potentially suspicious submissions

---

## 📊 Project Structure

```
NPS/
├── NPS.csproj                           # Project configuration
├── Program.cs                           # Entry point
├── App.axaml.cs                         # Application setup & DI
├── Views/
│   ├── MainWindow.axaml(.cs)           # Main menu & routing
│   ├── AttackWindow.axaml(.cs)         # Attack simulation UI
│   └── DetectWindow.axaml(.cs)         # Detection analysis UI
├── Services/
│   ├── Interfaces/
│   │   ├── IInjectService.cs           # Injection contract
│   │   ├── IReplaceService.cs          # Replacement contract
│   │   └── IDetectService.cs           # Detection contract
│   ├── AttackService/
│   │   ├── InjectService.cs            # Zero-width logic
│   │   └── ReplaceService.cs           # Homoglyph mapping
│   └── DetectService.cs                # Analysis engine
└── bin/Release/net8.0/win-x64/publish/ # Compiled executable
```

---

## 🔐 Security Considerations

### Legitimate Research Use
This toolkit is designed for:
- ✅ Security research and education
- ✅ Authorized penetration testing
- ✅ Defensive measures and mitigation
- ✅ Text normalization pipeline development

### Responsible Disclosure
- Do not use to create malicious content without authorization
- Report findings responsibly through proper channels
- Comply with local laws and regulations

---

## 📈 Performance Metrics

| Operation | Time | Notes |
|-----------|------|-------|
| Entropy calculation (1000 chars) | ~5ms | Single-pass algorithm |
| Homoglyph detection | ~10ms | Uses lookup tables |
| Full analysis (1000 chars) | ~50ms | All metrics computed |
| Rendering (1000 char visualization) | ~30ms | Real-time highlighting |

---

## 🎨 Features Showcase

### Visual Inspector
- **Red (●)** - Injected zero-width characters
- **Green (●)** - Substituted homoglyphs
- **Gray** - Normal text
- Instant feedback on hidden anomalies

### Threat Dashboard
- **Color-coded verdict**: Red (⚠ Suspicious ≥40/100), Green (✓ Clean <40/100)
- **Metric tiles**: Quick overview of detected anomalies
- **Threat bar**: Visual risk level progression
- **Findings list**: Concise, actionable insights

---

## 🚦 Development

### Build & Test
```bash
# Debug build
dotnet build

# Release build
dotnet build -c Release

# Run all tests (if added)
dotnet test
```

### Code Quality
- Follows C# coding standards
- Nullable reference types enabled
- Service-based architecture for testability
- Clean separation of concerns

---

## 📝 License

MIT License - See LICENSE file for details

---

## 👤 Author

**Project** demonstrating expertise in:
- Desktop application development (Avalonia/XAML)
- Service-oriented architecture & dependency injection
- Unicode and text analysis algorithms
- Security research tooling
- C# and .NET ecosystem

---

## 🤝 Contributing

Contributions, issues, and feature requests are welcome! Areas for expansion:
- Additional homoglyph sets (Latin, Greek, etc.)
- Advanced statistical analysis
- Batch processing capabilities
- Export reports (PDF, JSON)
- CLI interface for batch operations

---

## 📞 Support

For questions or issues:
1. Check existing documentation
2. Review the codebase (well-commented)
3. Open an issue with detailed description

---

## 🎯 Future Roadmap

- [ ] Cross-platform build (macOS, Linux)
- [ ] CLI tool for automation
- [ ] REST API for remote analysis
- [ ] Extended Unicode coverage
- [ ] Real-time text monitoring
- [ ] Batch file processing
- [ ] Export analysis reports (HTML, PDF, JSON)

---

**Made with ❤️ for security research and education**
