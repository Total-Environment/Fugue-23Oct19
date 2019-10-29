const csv = require('fast-csv');
const path = require('path');
const fsPath = require('fs-path');

let csvData = [];
let header = [];

Array.prototype.contains = function(obj) {
    var i = this.length;
    while (i--) {
        if (this[i] === obj) {
            return true;
        }
    }
    return false;
}

const materials = {
	'Aluminium & Copper': {
		'material_level_1': 'Primary',
		'definition_code': 'MAL',
		'material_name': 'Aluminium and Copper'
	},
	'Amenities': {
		'material_level_1': 'Secondary',
		'definition_code': 'MAM',
		'material_name': 'Amenities'
	},
	'Artefacts': {
		'material_level_1': 'Secondary',
		'definition_code': 'MAR',
		'material_name': 'Artefacts'
	},
	'Cement Products': {
		'material_level_1': 'Primary',
		'definition_code': 'MCC',
		'material_name': 'Cement Products'
	},
	'Clay': {
		'material_level_1': 'Primary',
		'definition_code': 'MCL',
		'material_name': 'Clay Material'
	},
	'Construction Chemical': {
		'material_level_1': 'Primary',
		'definition_code': 'MCH',
		'material_name': 'Construction Chemical'
	},
	'Electrical': {
		'material_level_1': 'Secondary',
		'definition_code': 'MEL',
		'material_name': 'Electrical'
	},
	'Fenestration': {
		'material_level_1': 'Secondary',
		'definition_code': 'MFN',
		'material_name': 'Fenestration'
	},
	'Fire Fighting System': {
		'material_level_1': 'Secondary',
		'definition_code': 'MFF',
		'material_name': 'Fire Fighting System'
	},
	'Flora': {
		'material_level_1': 'Primary',
		'definition_code': 'MFL',
		'material_name': 'Flora'
	},
	'Formwork': {
		'material_level_1': 'Secondary',
		'definition_code': 'MFW',
		'material_name': 'Formwork and Scaffolding'
	},
	'Furnishing': {
		'material_level_1': 'Secondary',
		'definition_code': 'MFS',
		'material_name': 'Furnishing'
	},
	'Furniture': {
		'material_level_1': 'Secondary',
		'definition_code': 'MFR',
		'material_name': 'Furniture'
	},
	'Glass Products': {
		'material_level_1': 'Primary',
		'definition_code': 'MGL',
		'material_name': 'Glass'
	},
	'Hardware': {
		'material_level_1': 'Secondary',
		'definition_code': 'MHD',
		'material_name': 'Tools | Hardware | Consumables'
	},
	'Home Automation': {
		'material_level_1': 'Secondary',
		'definition_code': 'MHA',
		'material_name': 'Home Automation'
	},
	'Home Entertainment': {
		'material_level_1': 'Secondary',
		'definition_code': 'MHE',
		'material_name': 'Home Entertainment'
	},
	'HVAC': {
		'material_level_1': 'Secondary',
		'definition_code': 'MHV',
		'material_name': 'HVAC'
	},
	'Steel Products': {
		'material_level_1': 'Primary',
		'definition_code': 'MIS',
		'material_name': 'Iron and Steel Products'
	},
	'IT': {
		'material_level_1': 'Secondary',
		'definition_code': 'MIT',
		'material_name': 'IT'
	},
	'Kitchen': {
		'material_level_1': 'Secondary',
		'definition_code': 'MKT',
		'material_name': 'Kitchen'
	},
	'Lights & Light Fixtures': {
		'material_level_1': 'Secondary',
		'definition_code': 'MLT',
		'material_name': 'Light and Light Fixture'
	},
	'Machines': {
		'material_level_1': 'Secondary',
		'definition_code': 'MMC',
		'material_name': 'Machines'
	},
	'Manufactured Board': {
		'material_level_1': 'Secondary',
		'definition_code': 'MMB',
		'material_name': 'Manufactured Board'
	},
	'Plastic & Polymer': {
		'material_level_1': 'Primary',
		'definition_code': 'MPP',
		'material_name': 'Plastics and Polymers'
	},
	'Fittings & Accessories': {
		'material_level_1': 'Secondary',
		'definition_code': 'MFT',
		'material_name': 'Plumbing Fixture & Fitting'
	},
	'Power Supply,Quality,Backup': {
		'material_level_1': 'Secondary',
		'definition_code': 'MPQ',
		'material_name': 'Power Supply, Quality & Backup'
	},
	'Reticulated Gas': {
		'material_level_1': 'Secondary',
		'definition_code': 'MRG',
		'material_name': 'Reticulated Gas'
	},
	'Safety': {
		'material_level_1': 'Secondary',
		'definition_code': 'MSA',
		'material_name': 'Safety'
	},
	'Sanitary Fixtures': {
		'material_level_1': 'Secondary',
		'definition_code': 'MFX',
		'material_name': 'Sanitary Fixtures'
	},
	'Stone': {
		'material_level_1': 'Primary',
		'definition_code': 'SNT',
		'material_name': 'Stone'
	},
	'Synthetic Flooring': {
		'material_level_1': 'Secondary',
		'definition_code': 'MSY',
		'material_name': 'Synthetic Flooring'
	},
	'Vehicles': {
		'material_level_1' : 'Secondary',
		'definition_code': 'MVH',
		'material_name': 'Vehicles'
	},
	'Vertical Transportation': {
		'material_level_1' : 'Secondary',
		'definition_code': 'MVT',
		'material_name': 'Vertical Transportation'
	},
	'Wood': {
		'material_level_1' : 'Primary',
		'definition_code': 'MWD',
		'material_name': 'Wood'
	},
	'Water Supply': {
		'material_level_1' : 'Secondary',
		'definition_code': 'MWS',
		'material_name': 'Water Supply'
	},
	'Generic Material': {
		'material_level_1' : 'Primary',
		'definition_code': 'GNR',
		'material_name': 'Generic Material'
	}
}

const arrayColumns = ['Approved Vendors'];
const moneyColumns = ['Wt. Avg. Purchase Rate', 'Last Purchase Rate'];
const checklistColumns = ['General PO Terms', 'Special PO Terms', 'Inspection Checklist', 'Maintenance Checklist', 'Specification Sheet'];
const staticFileColumns = ['Qty. Evaluation Method', 'Storage Method', 'Sampling Method', ];
const searchableColumns = ['Material Name', 'Material Code', 'Short Description', 'Shade Number', 'Shade Description', 'HSN Code', 'Associated Brands', 'Approved Vendors'];
const imageColumn = 'Image';
const brandColumn = 'Associated Brands';

const masterDataFileName = 'masterData.json';

const args = process.argv.slice(2);

const getArg = (attribute) => {
	for (var i = 0; i < args.length; i++) {
		if (args[i] == attribute) {
			return args[i+1];
		}
	}
}

const getBooleanArgs = (attribute) => {
	for (var i = 0; i < args.length; i++) {
		if(args[i] === attribute)
			return true;
	}
	return false;
}

const getArgs = (attribute) => {
	for (var i = 0; i < args.length; i++) {
		if (args[i] == attribute) {
			let value = [];
			for (var j = i + 1; j < args.length; j++) {
				if(args[j].includes('--'))
					return value;
				value.push(args[j]);
			}
			return value;
		}
	}
}

const columnListAttr = '--column-list';
const outputFolderArg = '--output-folder';
const extractMasterDataArg = '--extract-master-data';

const csvValueForCanbBeUsedAsAsset = 'Can be an Asset';
const csvFileMasterDataType = 'Master';
const csvFileAttachmentDataType = 'Attachment';
const csvFileSystemGeneratedDataType = '<Sytem Generated>';
const csvFileGlobalApplicableCatergory = 'Global';
const csvValueForSpecificationHeader = 'Specifications';
const csvValueForClassificationHeader = 'Classification';
const csvValueForMaintenanceHeader = 'Maintenance';

const heanderCsvIndex = 0;
const columnNameCsvIndex = 1;
const unitCsvIndex = 2;
const dataTypeCsvIndex = 3;
const isRequiredCsvIndex = 4;
const masterDataCountCsvIndex = 5;
const applicaleCategoriesCsvIndex = 6;
const masterDataStartCsvIndex = 7;

const headerNameAttr = 'Header Name';
const columnForHeaderCode = 'Header Code';
const columnNameAttr = 'Column Name';
const columnCodeAttr = 'Column Code';
const unitAttr = 'Unit';
const dataTypeAttr = 'Data Type';
const isRequiredAttr = 'Mandatory';
const applicableCategoriesAttr = 'Applicable Categories';
const masterDataCountAttr = 'Master Data count';
const canBeUsedAsAsset = 'Can be Used as an Asset';
const emptyString = '';
const jsonDelimiter = '  ';
const brandCode = 'approved_brands';

const arrayDataType = 'Array';
const masterDataDataType = 'MasterData';
const booleanDataType = 'Boolean';
const moneyDataType = 'Money';
const unitDataType = 'Unit';
const autoGeneratedDataType = 'Autogenerated';
const staticFileDataType = 'StaticFile';
const checklistDataType = 'CheckList';
const constantDataType = 'Constant';
const brandDataType = 'Brand';

const columnListFile = getArg(columnListAttr);
const outputFolder = getArg(outputFolderArg);

const needToExtractMasterData = getBooleanArgs(extractMasterDataArg);

csv.fromPath(columnListFile)
	.on("data", function(data){
	    if(header.length > 0)
	    	csvData.push(data.filter((d, i)=> i < 6 || !!d));

	    else
	    	header = data.filter(d=>!!d);
	})
	.on("end", function(){

		const columnObjects = csvData.reduce((acc, value) => acc.concat(buildObject(value)),[]);

		Object.keys(materials).forEach(material => {
			console.log('Extracting definition for: ', material);
			const def = getDefinition(material, columnObjects, material === 'Generic Material');
			writeFile(JSON.stringify(def, null, jsonDelimiter), `${material}.json`, outputFolder);
		});

		Object.keys(materials).forEach(material => {
			console.log('Extracting Asset definition for: ', material);
			const def = getMaintenance(material, columnObjects);
			writeFile(JSON.stringify(def, null, jsonDelimiter), `${material}.json`, `${outputFolder}\\Assets`);
		});

		if(needToExtractMasterData) {
			const masterDataJson = JSON.stringify(extractMasterData(columnObjects), null, jsonDelimiter);
			console.log('Extracting Master Data:')
			writeFile(masterDataJson, masterDataFileName, outputFolder);
		}
	});

const extractMasterData = (columns) => {
	return new MasterDataList(columns
		.filter(column=>column[masterDataCountAttr] > 0 || column[dataTypeAttr] === csvFileMasterDataType)
		.map(column => new MasterData(column[columnCodeAttr], column.Values))
	);
}

const writeFile = (text, fileName, outputPath) => {
	fsPath.writeFile(path.join(outputPath,fileName), text, function(err){
		if(err) {
			throw err;
		} else {
			console.log(`Extracted at: ${path.join(outputPath,fileName)}`);
		}
	});
}

const buildObject = (data) => {
	let object = {};
	object[headerNameAttr] = data[heanderCsvIndex];
	object[columnForHeaderCode] = getCodeFromName(data[heanderCsvIndex]);
	const finalColumnName = getColumnName(data);
	object[columnNameAttr] = finalColumnName;
	object[columnCodeAttr] = getCodeFromName(finalColumnName);
	object[unitAttr] = data[unitCsvIndex];
	object[dataTypeAttr] = data[dataTypeCsvIndex];
	object[isRequiredAttr] = data[isRequiredCsvIndex];
	object[masterDataCountAttr] = data[masterDataCountCsvIndex];
	object[applicableCategoriesAttr] = data[applicaleCategoriesCsvIndex]?data[applicaleCategoriesCsvIndex].split("|").filter(d => !!d).map(d => d.trim()):"";
	object.Values = data.splice(masterDataStartCsvIndex);
	return object;
}

const getColumnName = (data) => {
	if(data[columnNameCsvIndex] === csvValueForCanbBeUsedAsAsset)
		return canBeUsedAsAsset;
	return data[columnNameCsvIndex];
}

const getCodeFromName = (columnName) => {
	if(columnName === brandColumn)
		return brandCode;
	if(columnName === "Pur. Rate Threshold")
		return "purchase_rate_threshold";
	if(columnName === "Wt. Avg. Purchase Rate")
		return "weighted_average_purchase_rate";	
	return columnName.toLowerCase().replace(/ /g, '_').replace(/\W/g, emptyString).replace('__', '_');
}

const getMaintenance = (materialName, objects, isGeneric) => {
	return new Definition(getHeaders(objects, materialName, isGeneric, true), materials[materialName].material_name, materials[materialName].definition_code);
}

const getDefinition = (materialName, objects, isGeneric) => {
	return new Definition(getHeaders(objects, materialName, isGeneric, false), materials[materialName].material_name, materials[materialName].definition_code);
}

const getHeaders = (objects, materialName, isGeneric, forAsset) => {
	if(!forAsset) {
	return objects
		.map(object => object[headerNameAttr])
		.filter(m => !!m && m !== csvValueForMaintenanceHeader)
		.filter((x, i, a) => a.indexOf(x) == i)
		.map(headerName => {
			let columns = objects.filter(object => object[headerNameAttr] === headerName);
			if (headerName === csvValueForClassificationHeader)
				return new Header(getColumnGenerator(columns, materialName, isGeneric)(), headerName, columns[0][columnForHeaderCode], ["materialClassifications"]);
			else
				return new Header(getColumnGenerator(columns, materialName, isGeneric)(), headerName, columns[0][columnForHeaderCode], []);
		});
	}
	else {
	return objects
		.map(object => object[headerNameAttr])
		.filter(m => !!m && m === csvValueForMaintenanceHeader)
		.filter((x, i, a) => a.indexOf(x) == i)
		.map(headerName => {
			let columns = objects.filter(object => object[headerNameAttr] === headerName);
				return new Header(getColumnGenerator(columns, materialName, isGeneric)(), headerName, columns[0][columnForHeaderCode], []);
		});
	}
}

const getColumnGenerator = (columns, materialName, isGeneric) => {
	const columnConstructor = (column) =>  new Column(
		getDataType(column, materialName),
		column[columnNameAttr],
		column[columnCodeAttr],
		isRequired(column),
		isSearchable(column)
	);

	if(isGeneric)
		return () => columns.map(columnConstructor);

	else
		return () => columns
				.filter(column => isValidRecord(materialName, column))
				.map(columnConstructor);
}

const isSearchable = (column) => {
	if(column[headerNameAttr] === csvValueForSpecificationHeader)
		return true;
	if(column[headerNameAttr] === csvValueForClassificationHeader)
		return true;
	if(searchableColumns.contains(column[columnNameAttr]))
		return true;
	return false;
}

const getDataType = (column, materialName) => {
	if(column[columnNameAttr] === brandColumn)
		return getType(arrayDataType, getType(brandDataType, materials[materialName].definition_code.replace(materials[materialName].definition_code.substring(0,1),'B')))

	if(!!column[unitAttr])
		return getType(unitDataType, column[unitAttr]);

	if(column[dataTypeAttr] === csvFileMasterDataType)
		return masterDataType(column, materialName);

	if(column[dataTypeAttr] === 'Number')
		return getType('Int', emptyString);

	if(imageColumn === column[columnNameAttr])
		return getType(arrayDataType, getType(staticFileDataType, emptyString))

	if(arrayColumns.contains(column[columnNameAttr]))
		return getType(arrayDataType, getType(column[dataTypeAttr], column[unitAttr]));

	if(moneyColumns.contains(column[columnNameAttr]))
		return getType(moneyDataType, emptyString);

	if(column[columnNameAttr] === 'Material Code')
		return getType(autoGeneratedDataType, 'Material Code');

	if(column[dataTypeAttr].match(csvFileAttachmentDataType))
		return attachmentDataType(column);

	if(column[dataTypeAttr] === csvFileSystemGeneratedDataType)
		return getType(column[dataTypeAttr], column[columnNameAttr]);

	return getType(column[dataTypeAttr], column[unitAttr]);
}

const attachmentDataType = (column) => {
	if(staticFileColumns.contains(column[columnNameAttr]))
		return getType(staticFileDataType, emptyString);

	if(checklistColumns.contains(column[columnNameAttr]))
		return getType(checklistDataType, emptyString);
}

const getType = (dataType, unit) => {
	let finalType = dataType;
	switch(dataType) {
		case csvFileMasterDataType:
			finalType = masterDataType;
		case csvFileSystemGeneratedDataType :
			finalType = autoGeneratedDataType;
	}
	return new DataType(finalType, unit);
}

const masterDataType = (column, materialName) => {
	if(column[columnCodeAttr] === 'material_level_1')
		return getType(constantDataType, materials[materialName].material_level_1)

	if(column[columnCodeAttr] === 'material_level_2')
		return getType(constantDataType, materials[materialName].material_name);

	if(column[columnCodeAttr] === 'material_status')
		return getType(masterDataDataType, 'status');

	if(column.Values.length == 2 && column.Values.includes('Yes') && column.Values.includes('No'))
		return getType(booleanDataType, emptyString);

	return getType(masterDataDataType, column[columnCodeAttr]);
}

const isValidRecord = (materialName, object) => {
	return object.hasOwnProperty(applicableCategoriesAttr)
		&& (object[applicableCategoriesAttr].contains(materialName)
		|| object[applicableCategoriesAttr].contains(csvFileGlobalApplicableCatergory));
}

const isRequired = (column) => {
	if(column[columnNameAttr] === 'Material Code')
		return false;
	return column[isRequiredAttr] == 'Yes';
}

const Header = function(columns, name, key, dependencies){
	this.columns = columns;
	this.name = name;
	this.key = key;
	if(dependencies.length !== 0)
		this.dependencies = dependencies;
}

const Definition = function(headers, name, code) {
	this.headers = headers;
	this.name = name;
	this.code = code;
}

const Column = function(dataType, name, key, isRequired, isSearchable) {
	this.dataType = dataType;
	this.name = name;
	this.key = key;
	this.isRequired = isRequired;
	this.isSearchable = isSearchable;
}

const DataType = function(name, subType) {
	this.name = name;
	this.subType = subType;
}

// master data

const MasterDataList = function(list) {
	this.list = list;
}

const MasterData = function(key, values) {
	if(key.toLowerCase() === 'material_status'.toLowerCase())
		this.key = "status"
	else	
		this.key = key;
	this.values = values;
}
