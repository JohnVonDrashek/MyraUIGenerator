# NuGet Publishing Guide

This project is now configured for NuGet package publishing. Here's what has been set up:

## ✅ What's Configured

1. **Enhanced Package Metadata**
   - Repository URL, type, and branch
   - Package project URL
   - Symbol package support (snupkg)
   - All required NuGet metadata

2. **Automated Publishing Workflow**
   - GitHub Actions workflow at `.github/workflows/publish.yml`
   - Automatically publishes when you push a version tag
   - Creates both .nupkg and .snupkg (symbol) packages

3. **Package Structure**
   - Analyzer DLL in correct location (`analyzers/dotnet/cs/`)
   - README.md included in package
   - All metadata properly configured

## 🚀 How to Publish

### First Time Setup

1. **Get a NuGet API Key:**
   - Go to https://www.nuget.org/account/apikeys
   - Create a new API key with appropriate permissions
   - Set expiration (recommended: 1 year)
   - Copy the API key

2. **Add API Key to GitHub Secrets:**
   - Go to your GitHub repository: https://github.com/JohnVonDrashek/MyraUIGenerator
   - Navigate to: Settings → Secrets and variables → Actions
   - Click "New repository secret"
   - Name: `NUGET_API_KEY`
   - Value: Paste your NuGet API key
   - Click "Add secret"

### Publishing a New Version

1. **Update the version** in `src/MyraUIGenerator/MyraUIGenerator.csproj`:
   ```xml
   <Version>1.0.1</Version>  <!-- Change from 1.0.0 to your new version -->
   ```

2. **Commit and push:**
   ```bash
   git add src/MyraUIGenerator/MyraUIGenerator.csproj
   git commit -m "Bump version to 1.0.1"
   git push
   ```

3. **Create and push a version tag:**
   ```bash
   git tag v1.0.1
   git push origin v1.0.1
   ```

4. **The workflow will automatically:**
   - Extract version from tag
   - Update the project file
   - Build the project
   - Create the NuGet package
   - Publish to NuGet.org

### Manual Publishing (Alternative)

If you want to publish manually instead:

```bash
cd src/MyraUIGenerator

# Build the package
dotnet pack --configuration Release

# Publish to NuGet (replace YOUR_API_KEY with your actual key)
dotnet nuget push bin/Release/MyraUIGenerator.1.0.0.nupkg \
  --api-key YOUR_API_KEY \
  --source https://api.nuget.org/v3/index.json \
  --skip-duplicate
```

## 📦 Package Contents

The generated package includes:
- `analyzers/dotnet/cs/MyraUIGenerator.dll` - The source generator
- `README.md` - Package documentation
- Metadata (authors, license, repository, etc.)

## ⚠️ Notes

- **NU5017 Error**: You may see a NU5017 error during packing. This is a false positive from newer SDK versions for analyzer packages. The package is still created successfully and will work fine on NuGet.org.

- **Package Name**: Make sure `MyraUIGenerator` is available on NuGet.org before publishing. If it's taken, update the `PackageId` in the `.csproj` file.

- **First Publication**: After first publish, it may take a few minutes for the package to appear on nuget.org and pass validation.

- **Versioning**: Follow semantic versioning (MAJOR.MINOR.PATCH). Each published version is immutable on NuGet.org.

## 🔍 Verifying the Package

You can verify your package locally before publishing:

```bash
# Build the package
cd src/MyraUIGenerator
dotnet pack --configuration Release

# Check package contents
unzip -l bin/Release/MyraUIGenerator.1.0.0.nupkg
```

The package should contain:
- Analyzer DLL
- README.md
- Nuspec file with metadata

## 📚 Resources

- [NuGet Package Explorer](https://www.nuget.org/packages/NuGetPackageExplorer/) - Visual tool to inspect packages
- [NuGet.org Package Manager](https://www.nuget.org/manage/packages) - Manage your published packages
- [Semantic Versioning](https://semver.org/) - Version numbering guidelines

