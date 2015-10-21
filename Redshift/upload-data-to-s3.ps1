$BucketName = "winthroppoker"
$s3Directory = "C:\users\Administrator\Dropbox\data\PokerHands\nick.888.backlog.1935.raw"

foreach ($i in Get-ChildItem -Path $s3Directory -Filter *.csv -Recurse) {

  Write-Host "$($i.FullName)"
  # Write the file to S3 and add the filename to a collection.
  Write-S3Object -BucketName $BucketName -File $i.FullName -ProfileName nick

}