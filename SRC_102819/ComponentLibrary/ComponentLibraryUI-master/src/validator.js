import * as R from 'ramda';

const dataTypes = { Unit, Range, Array, String, Boolean, MasterData, Default, Money, Int, Decimal, StaticFile };

export function validate(value, columnDefinition, columnName, options) {
  if (value === '-- NA --') {
    return { isValid: true, msg: '' };
  }
  const dataType = columnDefinition.dataType;
  if (R.isNil(value) || R.isEmpty(value)) {
    if (columnDefinition.isRequired) {
      return { isValid: false, msg: 'The field cannot be left blank.' };
    }
    return { isValid: true, msg: '' };
  }
  return (dataTypes[dataType.name] || Default)(value, dataType, columnDefinition, columnName, options);
}

export function isNumber(value) {
  return !isNaN(value) && isFinite(value);
}

function Int(value) {
  const isValid = Number.isInteger(+value);
  return { isValid: isValid, msg: isValid ? '' : 'Not a valid integer' };
}

function Decimal(value, dataType, columnDefinition, columnName, options) {
  const isValid = isNumber(value);
  const customValidate = (options && options.validate(value));
  if (!isValid) {
    return {
      isValid: false,
      msg: 'Not a valid number'
    };
  }
  if (customValidate && !customValidate.isValid) {
    return customValidate;
  }
  return {
    isValid: true,
    msg: ''
  };
}

function MasterData(value) {
  return { isValid: true, msg: '' };
}
function Unit(value) {
  const isValid = isNumber(value.value);
  return { isValid: isValid, msg: isValid ? '' : 'Not a valid number' };
}

function Range(value, dataType, columnDefinition, columnName) {
  if (columnDefinition.filterable) {
    const isValid = validate(value,
      Object.assign({}, columnDefinition, { dataType: { name: 'Decimal', subType: null } }), columnName);
    const msg = isValid.msg;
    return { isValid: isValid.isValid, msg: msg };
  }
  if (!isNumber(value.from)) {
    return { isValid: false, msg: 'Not a valid number' };
  }
  if (value.to !== null && !isNumber(value.to)) {
    return { isValid: false, msg: 'Not a valid number' };
  }
  return { isValid: true, msg: '' };
}

function String(value) {
  return { isValid: true, msg: '' };
}

function Boolean(value) {
  return { isValid: true, msg: '' };
}

function Default(value) {
  return { isValid: true, msg: '' };
}

function Money(value) {
  if (!isNumber(value.amount)) {
    return { isValid: false, msg: 'Not a valid number' }
  } else if (value.currency === '' && value.amount !== '') {
    return { isValid: false, msg: 'Currency is required' }
  }
  return { isValid: true, msg: '' };
}

function Array(values, dataType, columnDefinition, columnName) {
  if (columnDefinition.filterable) {
    const isValid = validate(values,
      Object.assign({}, columnDefinition, { dataType: dataType.subType }), columnName);
    const msg = isValid.msg;
    return isValid.isValid ? { isValid: isValid.isValid, msg: msg } : { isValid: isValid.isValid, msg: '' };
  }
  const msg = { StaticFile: 'Invalid file format.\nOnly ‘jpg, jpeg, png, PNG, JPG, JPEG’ files are allowed' };
  const subType = dataType.subType.name;
  const isValid = values.map(value => validate(value,
    Object.assign({}, columnDefinition, { dataType: dataType.subType }), columnName).isValid)
    .filter(x => !x).length === 0;
  if (isValid) {
    if (subType === "StaticFile") {
      let seen = new Set();
      var hasDuplicates = values.some(function (currentObject) {
        if (currentObject !== null && currentObject.name !== undefined)
          return seen.size === seen.add(currentObject.name).size;
        else
          return false;
      });
      if (hasDuplicates) {
        return { isValid: false, msg: 'Duplicate values.' };
      } else {
        return { isValid: true, msg: '' };
      }
    } else {
      return { isValid: true, msg: '' };
    }
  } else {
    return { isValid: false, msg: msg[subType] ? `${msg[subType]}` : 'One of the values is invalid.' };
  }
}

function StaticFile(value, dataType, columnDefinition, columnName) {
  if (columnName && typeof (value) === "number") {
    if (columnName.toLowerCase() === "image") {
      const isImage = (/\.(jpg|jpeg|png|PNG|JPG|JPEG)$/i).test(window.fileList[value][0].name);
      return isImage ? { isValid: true, msg: '' } : {
        isValid: false,
        msg: 'Invalid file format. \nPlease upload the following file formats only: jpg, jpeg, png, PNG, JPG, JPEG'
      };
    }
    const isStaticFile = (/\.(jpg|JPG|jpeg|png|pdf|PNG|PDF|JPEG)$/i).test(window.fileList[value][0].name);
    return isStaticFile ? { isValid: true, msg: '' } : {
      isValid: false,
      msg: 'Invalid file format. \nPlease upload the following file formats only: jpg, jpeg, png, PNG, JPG, pdf, PDF, JPEG'
    };
  }
  return { isValid: true, msg: '' };
}
