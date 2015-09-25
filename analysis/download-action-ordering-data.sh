#!/bin/bash

mkdir -p action-ordering-data;
cd action-ordering-data;
# Download and squash the files into one big file
aws s3 cp --recursive 's3://winthropstage/pokerhands/' .;
echo "Squashing files.."
for file in action_ordering_*; do echo "$file"; cat "$file" >> action_ordering.csv; rm "$file";  done;

# Redshift exports with xff characters in the files for some unknown reason
echo "removing xff characters" 
sed -i 's/\xff//g' action_ordering.csv;

# lets take a look and see if the output is right
head action_ordering.csv;
