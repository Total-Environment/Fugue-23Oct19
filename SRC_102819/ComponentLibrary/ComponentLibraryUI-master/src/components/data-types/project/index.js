import React from 'react';
import styles from './index.css';
import {helpText, helpTextError} from '../../../css-common/forms.css';

const renderOption = (project) => {
  return <option key={project.projectCode} value={project.projectCode}>{project.projectName}</option>;
};
const renderInput = (props) => {
  return <select
    className={styles.select}
    onChange={e => props.onChange(e.target.value)}
    value={props.columnValue.value}>
    <option key="select" value="">Global</option> {/* This bit is bad. Fix it. */}
    {props.columnValue.dataType.projects.map(renderOption)}
  </select>;
};
const renderData = (props) => {
  return <span>{props.columnValue.value}</span>;
};

export const Project = (props) => {
  return props.columnValue.editable ? renderInput(props) : renderData(props);
};
