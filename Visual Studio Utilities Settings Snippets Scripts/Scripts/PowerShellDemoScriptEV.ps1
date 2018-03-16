Param($DataContext)
$DataContext = Get-Eventlog -log application -after ((get-date).AddSeconds(-60)) -EntryType Error | Where-Object {($_.Source -eq 'DEMO')}
