$LicenseText = @"
/*
 * Copyright (c) $(Get-Date -Format "yyyy") Rikitav Tim4ik
 * * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */


"@

$CheckPhrase = "Permission is hereby granted, free of charge"
$Files = Get-ChildItem -Filter *.cs -Recurse

Write-Host "Started..." -ForegroundColor Cyan

foreach ($File in $Files) {
    $Content = Get-Content -Path $File.FullName -Raw

    if ([string]::IsNullOrWhiteSpace($Content)) {
        continue
    }

    if ($Content -match [regex]::Escape($CheckPhrase)) {
        Write-Host "Skipped: $($File.FullName)" -ForegroundColor DarkGray
        continue
    }

    $NewContent = $LicenseText + $Content
    [System.IO.File]::WriteAllText($File.FullName, $NewContent)
    Write-Host "Added : $($File.FullName)" -ForegroundColor Green
}

Write-Host "Done!" -ForegroundColor Cyan
