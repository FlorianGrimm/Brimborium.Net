function convert2CS{
    param($text)
        
    if ($null -eq $text){ $text = ""}
    if ($text.GetType() -eq [string]) {
        $text = ([string]$text).Split([System.Environment]::NewLine, [System.StringSplitOptions]::None)
    }
    
    [System.Text.RegularExpressions.Regex]$r = [System.Text.RegularExpressions.Regex]::new('^([ \t]*)([^\r\n]*)([\r\n]*)$', [System.Text.RegularExpressions.RegexOptions]::Singleline)
    
    $infos = $text | % {
        [string] $Line=$_
        $m=$r.Match($line)
        $indent = 0
        $indentText = ""
        if ($m.Success){
            $indentText = $m.Groups[1].Value
            $indent = [int]([System.Math]::Floor($m.Groups[1].Value.Length / 4))
            $line = $m.Groups[2].Value
        }
        [PSCustomObject]@{indent=$indent; line=$line; diffIndent=0; indentText=$indentText}
       
    }
    
    for($idx=1; $idx -lt $infos.Length; $idx++){
        $a=$infos[$idx-1]
        $b=$infos[$idx]
        $a.diffIndent = $b.indent - $a.indent
    } 
    $infos
    $infos | % {
        $info = $_
        
        [string] $indentText = $info.indentText
        [string] $inLine = $info.line
        [int] $diffIndent = $info.diffIndent
        
        [string] $outLine = '    ctxt'
        
        $inLine = $inLine.Replace('"', '\\"')
        
        #$outLine += '.Write("'

        $outLine += '.Write('
        $outLine += '/*' + $indentText + '*/ "'

        $outLine += $inLine
        $outLine += '");'
        
        if ($diffIndent -eq 0){
            $outLine += ".WriteLine();"
        } elseif ($diffIndent -lt 0){
            $outLine += ".WriteLine(indent: $($diffIndent));"
        } elseif ($info.diffIndent -gt 0){
            $outLine += ".WriteLine(indent: +$($diffIndent));"
        }
        $outLine
    }
}
<# 

$text = Get-Clipboard
$text.GetType().FullName

#>

convert2CS $text