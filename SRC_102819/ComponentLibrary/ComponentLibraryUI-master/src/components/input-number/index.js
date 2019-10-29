import React from 'react';

function extractValue(value, allowDecimals) {
  const newValue = value && value.replace(allowDecimals ? /[^0-9.]/g : /[^0-9]/g,'');
  return newValue ? newValue : null;
}

export const InputNumber = (props) => <input type="text"
                                             name={props.name}
                                             id={props.id}
                                             className={props.className}
                                             value={props.value !== null && props.value !== undefined ? props.value : '' || ''}
                                             onChange={(e) => props.onChange(extractValue(e.target.value, props.allowDecimals))}/>;
InputNumber.defaultProps = {
  allowDecimals: true
};
