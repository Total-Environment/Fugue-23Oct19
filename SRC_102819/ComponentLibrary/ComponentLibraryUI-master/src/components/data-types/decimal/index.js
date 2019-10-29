import React from 'react';
import {InputNumber} from '../../input-number';
import {helpText, helpTextError} from '../../../css-common/forms.css';
import classNames from 'classnames';
import styles from './index.css';

const renderInput = (props) => <span>
  <InputNumber className={classNames(props.className, styles.input)}
               value={props.columnValue.value || ''}
               allowDecimals={true}
               onChange={(value) => props.onChange(value)}
  />
</span>;
const renderData = (props) => <span>{props.columnValue.value}</span>;

export const Decimal = (props) => props.columnValue.editable ? renderInput(props) : renderData(props);
