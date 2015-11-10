#!/bin/bash

mkdir -p datasets;
cd datasets;
# Download and squash the files into one big file
aws s3 cp --recursive 's3://winthropstage/pokerhands/' .;
echo "Squashing action ordering files.."
for file in action_ordering_*; do echo "$file"; cat "$file" >> action_ordering.csv; rm "$file";  done;

echo "Squashing holecard files.."
for file in holecards_by_actiontype_extract*; do echo "$file"; cat "$file" >> holecards_by_actiontype.csv; rm "$file";  done;
for file in holecards_by_actiontype_freq_extract*; do echo "$file"; cat "$file" >> holecards_by_actiontype_freq.csv; rm "$file";  done;

# Redshift exports with xff characters in the files for some unknown reason
echo "removing xff characters..." 
sed -i 's/\xff//g' action_ordering.csv;
sed -i 's/\xff//g' holecards_by_actiontype.csv;
sed -i 's/\xff//g' holecards_by_actiontype_freq.csv;

# lets take a look and see if the output is right
head *.csv
