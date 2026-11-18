param([string] $TemplateProject = '../UnityTemplates')

$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path -Parent $PSScriptRoot
$templateProjectRoot = if ([IO.Path]::IsPathRooted($TemplateProject)) {
    [IO.Path]::GetFullPath($TemplateProject)
} else { [IO.Path]::GetFullPath((Join-Path $projectRoot $TemplateProject)) }
$templateRoot = Join-Path $templateProjectRoot 'Assets/UnityTemplates'
$linkRoot = Join-Path $projectRoot 'Assets/TemplateSources'
$units = @('Foundation', 'Modules/Framework/DependencyInjection', 'Modules/Framework/Localization', 'Modules/Core/Save', 'Modules/Core/SceneFlow')

foreach ($unit in $units) {
    foreach ($part in @('Runtime', 'Editor')) {
        $target = Join-Path $templateRoot "$unit/$part"
        if (-not (Test-Path -LiteralPath $target -PathType Container)) {
            if ($part -eq 'Runtime') { throw "Required source is missing: $target" }
            continue
        }
        $link = Join-Path $linkRoot "$unit/$part"
        $existing = Get-Item -LiteralPath $link -Force -ErrorAction SilentlyContinue
        if ($existing) {
            if (($existing.Attributes -band [IO.FileAttributes]::ReparsePoint) -and
                ([IO.Path]::GetFullPath($existing.Target) -eq [IO.Path]::GetFullPath($target))) { continue }
            throw "Destination exists with a different target: $link"
        }
        New-Item -ItemType Directory -Force -Path (Split-Path -Parent $link) | Out-Null
        if (-not (Test-Path -LiteralPath "$link.meta")) {
            Copy-Item -LiteralPath "$target.meta" -Destination "$link.meta"
        }
        New-Item -ItemType Junction -Path $link -Target $target | Out-Null
    }
}
Write-Host 'RD Learn English template sources linked. Shared demos and tests are excluded.'
