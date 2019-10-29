# ColumnListParser.js         
​
Reads the csv file specified and outputs the material definition jsons.
​
## How to run
```
node ColumnListParser.js --extract-master-data --columnlist <path of column csv file> --output-folder <folder to which all the definitions are extracted>
```

## Arguments

1. --extract-master-data: extracts the master data of the column where master data count is greater than one, if this flag is passed as argument.
2. --columnList <filePath>:  filePath specifies the path of the csv file which contains the column list.
3. --output-folder <folder path>: takes the folder path where all the json are extracted to. 
