    Param($DataContext)

    $Dir = get-childitem "c:\\test" -recurse
    # $Dir |get-member
    $files = $Dir | where {$_.extension -eq ".txt"}
    for ($i=0; $i -lt $files.Count; $i++) {
        $outfile = $files[$i].FullName + ".done" 
        $DataContext = Get-Content $files[$i].FullName
        rename-item -path $files[$i].FullName -newname $outfile

    }
