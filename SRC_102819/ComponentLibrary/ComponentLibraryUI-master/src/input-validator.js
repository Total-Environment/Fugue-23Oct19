export const validateModel = (model) => {
  model.validity = validate(model.value, model.validators);
};


export const validate = (value, validators) => {
  const validatorMapping = {
    'Mandatory': isEmpty,
    'PositiveNumber': isPositiveNumber,
    'NaturalNumber': isNaturalNumber
  };

  return validators.reduce((finalValue, validator) => (
    finalValue.isValid ? validatorMapping[validator](value) : finalValue
  ), {isValid: true, message: ''});
}

const isEmpty = (value) =>{
  const isValid = value != undefined && value != null && value !== '';
  return {isValid, message: isValid ? '' : 'The field cannot be left blank.'};
}

const isPositiveNumber = (value) =>{
  const isValid = +value >= 0;
  return {isValid, message: isValid ? '' : 'Invalid value'};
}

const isNaturalNumber = (value) =>{
  const isValid = +value > 0;
  return {isValid, message: isValid ? '' : 'Invalid value'};
}
