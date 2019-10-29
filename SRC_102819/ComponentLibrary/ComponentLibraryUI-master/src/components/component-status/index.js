import React from 'react';
import styles from './index.css';
import {idFor} from '../../helpers';
import classNames from 'classnames';

export const ComponentStatus = (props) => <span className={classNames(props.className, styles[idFor(props.value)])}>
  {props.value}
</span>;
