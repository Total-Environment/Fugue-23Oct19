import React from 'react';
import {InputNumber} from '../../input-number';
import {helpText, helpTextError} from '../../../css-common/forms.css';
import classNames from 'classnames';
import styles from './index.css';

const renderHelpText = (validity) => {
  return <p className={classNames({
    [helpText]: true,
    [helpTextError]: !validity.isValid
  })}> {validity.msg}</p>;
};
const renderInput = (props) => <span>
  <InputNumber value={props.columnValue.value || ''}
               className={styles.input}
               allowDecimals={false}
               onChange={(value) => props.onChange(value)}
               />
  {renderHelpText(props.columnValue.validity)}
</span>;
const renderData = (props) => <span>{props.columnValue.value}</span>;

export const Int = (props) => props.columnValue.editable ? renderInput(props) : renderData(props);
